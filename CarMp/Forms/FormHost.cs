using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMp.Views;
using System.Threading;
using CarMp.ViewControls;
using System.Xml;
using CarMp.Reactive.Touch;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMp.Forms
{
    public partial class FormHost : Form
    {
        private ManualResetEvent _viewChanging;

        private DateTime _fpsCalcDate;
        private long _fpsCalcFramesTotal;
        private long _fpsCalcFramesCurrent;
        private Text _fpsControl;
        private D2DViewControl _mouseDownViewControl;

        private List<D2DViewControl> _overlayViewControls;

        private EventToTouchBridge _mouseEventProcessor;
        private Dictionary<string, D2DView> _loadedViews;
        private D2DView _currentView;

        private D2DViewFactory _viewFactory;

        private Direct2D.RenderTargetWrapper _renderTarget;

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
            _mouseEventProcessor = new EventToTouchBridge(this);
            _mouseEventProcessor.ObservablTouchActions.ObsTouchGesture.Subscribe((tg) => RouteTouchEvents(tg));
            _mouseEventProcessor.ObservablTouchActions.ObsTouchMove.Subscribe((tm) => RouteTouchEvents(tm));
            //D2DViewControl.SetTouchObservables(_mouseEventProcessor.ObservablTouchActions);

            _viewChanging = new ManualResetEvent(false);
            
            _fpsCalcFramesTotal = 0;
            _fpsCalcDate = DateTime.Now;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            _renderTarget = new Direct2D.RenderTargetWrapper(
                Direct2D.D2DFactory.CreateHwndRenderTarget(
                _renderProps,
                new HwndRenderTargetProperties(this.Handle, new SizeU(Convert.ToUInt32(ClientSize.Width), Convert.ToUInt32(ClientSize.Height)), PresentOptions.Immediately)));
            

            this.ClientSizeChanged += (o, e) => { _renderTarget.Resize(new SizeU(Convert.ToUInt32(ClientSize.Width), Convert.ToUInt32(ClientSize.Height))); };
            
            InitializeComponent();

            this.Size = SessionSettings.ScreenResolution;
            this.Location = SessionSettings.WindowLocation;

            _overlayViewControls = new List<D2DViewControl>();
            _viewFactory = new D2DViewFactory(new SizeF(ClientSize.Width, ClientSize.Height));
            _loadedViews = new Dictionary<string, D2DView>();

            InitializeOverlayControls();

            Action renderingLoop = new Action(() => RenderingLoop());
            renderingLoop.BeginInvoke(null, null);
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
                    currentlySelected.SendTouch(pTouchEvent);
                    return;
                }
            }

            
            currentlySelected = _currentView.GetViewControlContainingPoint(pTouchEvent.Location);
            if(currentlySelected != null)
                currentlySelected.SendTouch(pTouchEvent);
        }

        private void InitializeOverlayControls()
        {
            MediaControlBar controlBar = null;
            MediaInfoBar infoBar = null;
            XmlNode infoBarNode = SessionSettings.CurrentSkin.GetOverlayNodeSkin("MediaInfoBar");
            XmlNode controlBarNode = SessionSettings.CurrentSkin.GetOverlayNodeSkin("MediaControlBar");

            _fpsControl = new ViewControls.Text();
            _fpsControl.Bounds = new RectF(this.Width - 40, 0, this.Width, 40);
            _overlayViewControls.Add(_fpsControl);

            if (controlBarNode != null)
            {
                controlBar = new MediaControlBar();
                controlBar.ApplySkin(controlBarNode, SessionSettings.SkinPath);
                _overlayViewControls.Add(controlBar);
                controlBar.StartRender();
            }

            if (infoBarNode != null)
            {
                infoBar = new MediaInfoBar();
                infoBar.ApplySkin(infoBarNode, SessionSettings.SkinPath);
                _overlayViewControls.Add(infoBar);
                infoBar.StartRender();
            }

            GraphicalButton toggleAnimation = new GraphicalButton();
            toggleAnimation.Bounds = new RectF(450, 30, 514, 94);
            toggleAnimation.SetButtonUpBitmapData(@"C:\source\CarMp\trunk\Images\Skins\BMW\Box.bmp");
            int i = 0;

            toggleAnimation.Click += (sender, e) =>
                {
                    controlBar.SetAnimation(i);
                    controlBar.StartAnimation();
                    infoBar.SetAnimation(i);
                    infoBar.StartAnimation();
                    if(i > 0)
                        i = -1 ;
                    else i = 1;
                };
            _fpsControl.StartRender();

            toggleAnimation.StartRender();
            _overlayViewControls.Add(toggleAnimation);
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
                    _currentView = _viewFactory.CreateView(pViewName);

                    _loadedViews.Add(pViewName, _currentView);
                    if (_currentView is ISkinable)
                    {
                        System.Xml.XmlNode viewSkinNode = SessionSettings.CurrentSkin.GetViewNodeSkin(pViewName);

                        if (viewSkinNode != null)
                            (_currentView as ISkinable).ApplySkin(viewSkinNode, SessionSettings.SkinPath);
                    }

                }
                _currentView.StartRender();

            }
            finally
            {
                _viewChanging.Set();
            }

            
            return _currentView;
        }

        public void ApplySkin()
        {
            foreach (KeyValuePair<string, D2DView> kv in _loadedViews)
            {
                D2DView view = kv.Value;
                if (view is ISkinable)
                {
                    System.Xml.XmlNode viewSkinNode = SessionSettings.CurrentSkin.GetViewNodeSkin(view.Name);

                    if (viewSkinNode != null)
                        (view as ISkinable).ApplySkin(viewSkinNode, SessionSettings.SkinPath);
                }
            }
        }

        // OnPaint implmentation
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    DrawDirect2D();
        //    base.OnPaint(e);
        //}

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

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    for (int i = _overlayViewControls.Count - 1;
        //        i >= 0;
        //        i--)
        //    {
        //        _mouseDownViewControl = _overlayViewControls[i].GetViewControlContainingPoint(e.Location);
        //        if (_mouseDownViewControl != null)
        //        {
        //            _mouseDownViewControl.MouseDown(e);
        //            return;
        //        }
        //    }
        //    if (_currentView is D2DView)
        //    {
        //        _mouseDownViewControl = _currentView.GetViewControlContainingPoint(e.Location);
        //        if(_mouseDownViewControl != null)
        //            _mouseDownViewControl.MouseDown(e);
        //    }
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    return;
        //    EventQueue eventQueue = new EventQueue();

        //    if (_mouseDownViewControl != null)
        //        eventQueue.AddToQueue(new EventQueueEntry(new EventQueueDelegate<MouseEventArgs>(_mouseDownViewControl.MouseMove), e));

        //    for (int i = _overlayViewControls.Count - 1;
        //        i >= 0;
        //        i--)
        //    {
        //        if (_mouseDownViewControl != null &&
        //            _overlayViewControls[i] == _mouseDownViewControl)
        //            continue;

        //        if (_overlayViewControls[i].Bounds.Contains(e.Location))
        //        {
        //            eventQueue.AddToQueue(new EventQueueEntry(new EventQueueDelegate<MouseEventArgs>(_overlayViewControls[i].MouseMove), e));
        //            return;
        //        }
        //    }
        //    if (_currentView is D2DView)
        //    {
        //        D2DViewControl control = _currentView.GetViewControlContainingPoint(e.Location);
        //        if (control != null && _mouseDownViewControl != control)
        //            eventQueue.AddToQueue(new EventQueueEntry(new EventQueueDelegate<MouseEventArgs>(control.MouseMove), e));
        //    }

        //    eventQueue.ProcessQueue();
        //}

        //protected override void OnMouseUp(MouseEventArgs e)
        //{
        //    EventQueue queue = new EventQueue();

        //    if (_mouseDownViewControl != null)
        //        queue.AddToQueue(new EventQueueEntry(new EventQueueDelegate<MouseEventArgs>(_mouseDownViewControl.MouseUp), e));
            
        //    for (int i = _overlayViewControls.Count - 1;
        //        i >= 0;
        //        i--)
        //    {

        //        if (_overlayViewControls[i].Bounds.Contains(e.Location))
        //        {
        //            if (_mouseDownViewControl != null
        //                && _overlayViewControls[i] == _mouseDownViewControl)
        //            {
        //                continue;
        //            }

        //            int j = i;
        //            queue.AddToQueue(new EventQueueEntry(new EventQueueDelegate<MouseEventArgs>(_overlayViewControls[j].MouseUp), e));
        //            break;
        //        }
        //    }
        //    if (_currentView is D2DView)
        //    {
        //        D2DViewControl control = _currentView.GetViewControlContainingPoint(e.Location);
        //        if (control != null && _mouseDownViewControl != control)
        //            queue.AddToQueue(new EventQueueEntry(new EventQueueDelegate<MouseEventArgs>(control.MouseUp), e));
        //    }
        //    _mouseDownViewControl = null;

        //    queue.ProcessQueue();
        //}

        protected override void OnSizeChanged(EventArgs e)
        {
            if (_currentView is D2DView)
            {
                (_currentView as D2DView).OnSizeChanged(this, e);
            }
        }
    }
}
