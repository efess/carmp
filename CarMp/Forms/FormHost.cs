using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Threading;
using CarMP.Views;
using CarMP.ViewControls;
using CarMP.Reactive.Touch;
using CarMP.Reactive.KeyInput;
using CarMP.Reactive;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;
using CarMP.Direct2D;
using CarMP.Callbacks;

namespace CarMP.Forms
{
    public partial class FormHost : Form, IMessageHookable
    {
        private ManualResetEvent _viewChanging;

        private DateTime _fpsCalcDate;
        private long _fpsCalcFramesTotal;
        private long _fpsCalcFramesCurrent;
        private Text _fpsControl;
        private D2DViewControl _mouseDownViewControl;

        private List<D2DViewControl> _overlayViewControls;
        private W32MessageToReactive _mouseEventProcessor;
        private Dictionary<string, D2DView> _loadedViews;
        private D2DView _currentView;
        private D2DViewFactory _viewFactory;

        private Direct2D.RenderTargetWrapper _renderTarget;
        protected override void WndProc(ref Message m)
        {
            if (MessagePump != null)
                MessagePump(ref m);

            base.WndProc(ref m);
        }
        public Messenger MessagePump { get; set; }
        RenderTargetProperties _renderProps = new RenderTargetProperties
        {
            PixelFormat = new PixelFormat(
                Microsoft.WindowsAPICodePack.DirectX.DXGI.Format.B8G8R8A8_UNORM,
                AlphaMode.Ignore),
            Usage = RenderTargetUsage.None,
            Type = RenderTargetType.Default // Software type is required to allow resource 
            // sharing between hardware (HwndRenderTarget) 
            // and software (WIC Bitmap render Target).
        };
        

        public FormHost()
        {
            // Initialize & set Touch Observable
            _mouseEventProcessor = new W32MessageToReactive(this);
            _mouseEventProcessor.ObservableActions.ObsTouchGesture.Subscribe((tg) => RouteTouchEvents(tg));
            _mouseEventProcessor.ObservableActions.ObsTouchMove.Subscribe((tm) => RouteTouchEvents(tm));
            _mouseEventProcessor.ObservableActions.ObsKeyInput.Subscribe((ki) => RouteKeyInputEvents(ki));

            //D2DViewControl.SetTouchObservables(_mouseEventProcessor.ObservablTouchActions);

            _viewChanging = new ManualResetEvent(false);
            
            _fpsCalcFramesTotal = 0;
            _fpsCalcDate = DateTime.Now;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            _renderTarget = new Direct2D.RenderTargetWrapper(
                D2DStatic.D2DFactory.CreateHwndRenderTarget(
                _renderProps,
                new HwndRenderTargetProperties(this.Handle, new SizeU(Convert.ToUInt32(ClientSize.Width), Convert.ToUInt32(ClientSize.Height)), PresentOptions.Immediately)));
            
            this.ClientSizeChanged += (o, e) => { _renderTarget.Resize(new SizeU(Convert.ToUInt32(ClientSize.Width), Convert.ToUInt32(ClientSize.Height))); };
            
            InitializeComponent();

            this.Size = new System.Drawing.Size(Convert.ToInt32(AppMain.Settings.ScreenResolution.Width), Convert.ToInt32(AppMain.Settings.ScreenResolution.Height));
            this.Location = new System.Drawing.Point(Convert.ToInt32(AppMain.Settings.WindowLocation.X), Convert.ToInt32(AppMain.Settings.WindowLocation.Y)); 

            _overlayViewControls = new List<D2DViewControl>();
            _viewFactory = new D2DViewFactory(new SizeF(ClientSize.Width, ClientSize.Height));
            _loadedViews = new Dictionary<string, D2DView>();

            InitializeOverlayControls();
            
            ApplySkin();

            Action renderingLoop = new Action(() => RenderingLoop());
            renderingLoop.BeginInvoke(null, null);
        }

        public void RouteKeyInputEvents(Key pKeyInput)
        {
            if (D2DViewControl.HasInputControl != null)
                D2DViewControl.HasInputControl.SendUpdate(pKeyInput);
        }

        public void RouteTouchEvents(Touch pTouchEvent)
        {
            D2DViewControl currentlySelected = null;
            for (int i = _overlayViewControls.Count - 1;
                i >= 0;
                i--)
            {
                currentlySelected = _overlayViewControls[i].GetViewControlContainingPoint(pTouchEvent.Location);
                if (currentlySelected != null)
                {
                    currentlySelected.SendUpdate(pTouchEvent);
                    return;
                }
            }
            
            currentlySelected = _currentView.GetViewControlContainingPoint(pTouchEvent.Location);
            if(currentlySelected != null)
                currentlySelected.SendUpdate(pTouchEvent);
        }

        private void InitializeOverlayControls()
        {
            _fpsControl = new ViewControls.Text();
            _fpsControl.Bounds = new RectF(this.Width - 40, 0, this.Width, 40);
            _overlayViewControls.Add(_fpsControl);
                        
            //GraphicalButton toggleAnimation = new GraphicalButton();
            //toggleAnimation.Bounds = new RectF(450, 30, 514, 94);
            //toggleAnimation.SetButtonUpBitmapData(@"C:\source\CarMP\trunk\Images\Skins\BMW\Box.bmp");
            //int i = 0;

            //toggleAnimation.Click += (sender, e) =>
            //    {
            //        controlBar.SetAnimation(i);
            //        controlBar.StartAnimation();
            //        infoBar.SetAnimation(i);
            //        infoBar.StartAnimation();
            //        if(i > 0)
            //            i = -1 ;
            //        else i = 1;
            //    };
        }

        public D2DView ShowView(string pViewName)
        {
            _viewChanging.Reset();
            try
            {
                if (_currentView != null)
                {
                    // View is showing, hide this view
                }

                // Create view
                if (_loadedViews.ContainsKey(pViewName))
                    _currentView = _loadedViews[pViewName];
                else
                {
                    // Create view
                    var view = _viewFactory.CreateView(pViewName);

                    if (view == null)
                    {
                        DebugHandler.DebugPrint("Tried to create view, but view not found " + pViewName);
                        return null;
                    }

                    _currentView = view;

                    _loadedViews.Add(pViewName, _currentView);
                    if (_currentView is ISkinable)
                    {
                        System.Xml.XmlNode viewSkinNode = AppMain.Settings.CurrentSkin.GetViewNodeSkin(pViewName);

                        if (viewSkinNode != null)
                            (_currentView as ISkinable).ApplySkin(viewSkinNode, AppMain.Settings.CurrentSkinPath);
                    }

                }
                //_currentView.StartRender();

            }
            finally
            {
                _viewChanging.Set();
            }

            
            return _currentView;
        }

        public void ApplySkin()
        {
            AppMain.Settings.CurrentSkin.ReloadCurrent();

            foreach (KeyValuePair<string, D2DView> kv in _loadedViews)
            {
                D2DView view = kv.Value;
                if (view is ISkinable)
                {
                    System.Xml.XmlNode viewSkinNode = AppMain.Settings.CurrentSkin.GetViewNodeSkin(view.Name);

                    if (viewSkinNode != null)
                        (view as ISkinable).ApplySkin(viewSkinNode, AppMain.Settings.CurrentSkinPath);
                }
            }

            _overlayViewControls.ForEach((vc) => vc.Dispose());
            _overlayViewControls.Clear();

            foreach (XmlNode node in
                AppMain.Settings.CurrentSkin.OverlayNodes)
            {
                var viewControl = ViewControlFactory.GetViewControlAndApplySkin(
                    node.Name,
                    AppMain.Settings.CurrentSkinPath,
                    node);
                if(viewControl != null)
                    _overlayViewControls.Add(viewControl);
            }    
        }

        private void RenderingLoop()
        {
            System.Threading.Thread.CurrentThread.Name = "Rendering Loop";
            while (true)
            {
                _viewChanging.WaitOne();

                if(_currentView != null)
                    DrawDirect2D();
                
                _fpsCalcFramesTotal++;
                if ( DateTime.Now.AddSeconds(-1) > _fpsCalcDate)
                {
                    _fpsControl.TextString = (_fpsCalcFramesTotal - _fpsCalcFramesCurrent).ToString(); ;
                    _fpsCalcFramesCurrent = _fpsCalcFramesTotal;
                    _fpsCalcDate = DateTime.Now;
                }

                System.Threading.Thread.Sleep(5);
            }
        }

        private void DrawDirect2D()
        {
            try
            {
                if (!_renderTarget.IsOccluded)
                {
                    _renderTarget.BeginDraw();
                    _renderTarget.Transform = Matrix3x2F.Identity;
                    _renderTarget.Clear(new ColorF(Colors.Black, 1f));

                    _currentView.Render(_renderTarget);

                    for (int i = _overlayViewControls.Count - 1;
                        i >= 0;
                        i--)
                    {
                        _overlayViewControls[i].Render(_renderTarget);
                    }

                    _renderTarget.EndDraw();

                    this.Invalidate();
                }
            }
            catch (Exception ex)
            {
                DebugHandler.HandleException(ex);
                _renderTarget.Renderer.Flush();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (_currentView is D2DView)
            {
                (_currentView as D2DView).OnSizeChanged(this, e);
            }
        }
    }
}
