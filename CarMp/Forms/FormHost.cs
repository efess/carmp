using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using CarMP.Callbacks;
using CarMP.Reactive;
using CarMP.Reactive.KeyInput;
using CarMP.Reactive.Messaging;
using CarMP.Reactive.Touch;
using CarMP.ViewControls;
using CarMP.Views;
using CarMP.Win32;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;
using CarMP.Graphics.Geometry;

namespace CarMP.Forms
{
    public partial class FormHost : Form, IWin32MessageHookable, IMessageObserver
    {
        private ManualResetEvent _viewChanging;

        private DateTime _fpsCalcDate;
        private long _fpsCalcFramesTotal;
        private long _fpsCalcFramesCurrent;
        private Text _fpsControl;
        private D2DViewControl _mouseDownViewControl;

        private D2DView _overlayViewControls;
        private W32MessageToReactive _mouseEventProcessor;
        private Dictionary<string, D2DView> _loadedViews;
        private D2DView _currentView;
        private D2DViewFactory _viewFactory;

        //private Direct2D.RenderTargetWrapper _renderTarget;
        private IRenderer _renderer;


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (MessagePump != null)
                MessagePump(ref m);
            
            base.WndProc(ref m);
        }
        public Win32Messenger MessagePump { get; set; }

        //RenderTargetProperties _renderProps = new RenderTargetProperties
        //{
        //    PixelFormat = new PixelFormat(
        //        Microsoft.WindowsAPICodePack.DirectX.DXGI.Format.B8G8R8A8_UNORM,
        //        AlphaMode.Ignore),
        //    Usage = RenderTargetUsage.None,
        //    Type = RenderTargetType.Default // Software type is required to allow resource 
        //    // sharing between hardware (HwndRenderTarget) 
        //    // and software (WIC Bitmap render Target).
        //};
        

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

            _renderer = new RendererFactory().GetRenderer("opengl", this.Handle);
            if (_renderer is CarMP.Graphics.Implementation.OpenGL.OpenGLRenderer)
            {
                var oGL = (_renderer as CarMP.Graphics.Implementation.OpenGL.OpenGLRenderer);
                oGL.CreateWindow(new Graphics.Implementation.OpenGL.OpenGLRenderer.RenderEventHandler(DrawDirect2D));
            }
            this.ClientSizeChanged += (o, e) => { _renderer.Resize(new SizeI(ClientSize.Width, ClientSize.Height)); };
            
            InitializeComponent();

            this.Size = new System.Drawing.Size(Convert.ToInt32(AppMain.Settings.ScreenResolution.Width), Convert.ToInt32(AppMain.Settings.ScreenResolution.Height));
            this.Location = new System.Drawing.Point(Convert.ToInt32(AppMain.Settings.WindowLocation.X), Convert.ToInt32(AppMain.Settings.WindowLocation.Y));

            _viewFactory = new D2DViewFactory(new Size(ClientSize.Width, ClientSize.Height));
            _loadedViews = new Dictionary<string, D2DView>();

            _overlayViewControls = _viewFactory.CreateView(D2DViewFactory.OVERLAY);

            ApplySkin();

            //Action renderingLoop = new Action(() => RenderingLoop());
            //renderingLoop.BeginInvoke(null, null);
        }

        public void RouteKeyInputEvents(Key pKeyInput)
        {
            if (D2DViewControl.HasInputControl != null)
                D2DViewControl.HasInputControl.SendUpdate(pKeyInput);
        }

        public void RouteTouchEvents(Touch pTouchEvent)
        {
            D2DViewControl currentlySelected = null;
            //for (int i = _overlayViewControls.Count - 1;
            //    i >= 0;
            //    i--)
            //{
                currentlySelected = _overlayViewControls.GetViewControlContainingPoint(pTouchEvent.Location);
                if (currentlySelected != null
                    && currentlySelected != _overlayViewControls)
                {
                    currentlySelected.SendUpdate(pTouchEvent);
                    return;
                }
            //}
            
            currentlySelected = _currentView.GetViewControlContainingPoint(pTouchEvent.Location);
            if(currentlySelected != null)
                currentlySelected.SendUpdate(pTouchEvent);
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
                            (_currentView as ISkinable).ApplySkin(viewSkinNode, AppMain.Settings.CurrentSkin.CurrentSkinPath);
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
                        (view as ISkinable).ApplySkin(viewSkinNode, AppMain.Settings.CurrentSkin.CurrentSkinPath);
                }
            }

            //_overlayViewControls.ForEach((vc) => vc.Dispose());
            _overlayViewControls.Clear();

            foreach (XmlNode node in
                AppMain.Settings.CurrentSkin.OverlayNodes)
            {
                var viewControl = ViewControlFactory.GetViewControlAndApplySkin(
                    node.Name,
                    AppMain.Settings.CurrentSkin.CurrentSkinPath,
                    node);
                //if (viewControl is IMessageObserver)
                //    AppMain.Messanger.AddMessageObserver(viewControl as IMessageObserver);
                if(viewControl != null)
                    _overlayViewControls.AddViewControl(viewControl);
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
                    //_fpsControl.TextString = (_fpsCalcFramesTotal - _fpsCalcFramesCurrent).ToString(); ;
                    _fpsCalcFramesCurrent = _fpsCalcFramesTotal;
                    _fpsCalcDate = DateTime.Now;
                }

                System.Threading.Thread.Sleep(10);
            }
        }

        private void DrawDirect2D()
        {
            try
            {
                //if (!_renderTarget.IsOccluded)
                //{
                    if (_currentView == null) return;
                    _renderer.BeginDraw();
                    
                    _renderer.Clear(Color.Black);

                    _currentView.Render(_renderer);
                    _overlayViewControls.Render(_renderer);

                    //for (int i = _overlayViewControls.Count - 1;
                    //    i >= 0;
                    //    i--)
                    //{
                    //    _overlayViewControls[i].Render(_renderTarget);
                    //}

                    _renderer.EndDraw();

                    //this.Invalidate();
                //}
            }
            catch (Exception ex)
            {
                DebugHandler.HandleException(ex);
                _renderer.Flush();
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

        #region IMessageObserver Members

        public void ProcessMessage(Reactive.Messaging.Message pMessage)
        {
            switch (pMessage.Type)
            {
                case MessageType.SwitchView:
                    ShowView(pMessage.Data as String);
                    break;
            }
        }

        public IDisposable DisposeUnsubscriber { get; set; }

        #endregion
    }
}
