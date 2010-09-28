using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMP.Views;
using CarMP.Reactive.Touch;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Reactive;
using CarMP.Reactive.KeyInput;

namespace CarMP.ViewControls
{
    public abstract class D2DViewControl : IDisposable
    {
        public event EventHandler RenderStart;
        public event EventHandler RenderStop;

        internal static D2DViewControl HasInputControl { get;  set; }
        protected D2DViewControl _mouseDownControl;

        private List<D2DViewControl> _viewControls;

        private bool _renderOk = false;
        public void StartRender()
        {
            _renderOk = true;
            OnRenderStart();
        }
        public void StopRenderer()
        {
            _renderOk = false;
            OnRenderStop();
        }

        public D2DViewControl GetViewControlContainingPoint(Point2F pPoint)
        {
            D2DViewControl control = GetViewControlContainingPoint(this, pPoint);

            if (control == this && GetScreenBounds().Contains(pPoint))
                return this;

            return control == this ? null : control;
        }

        public D2DViewControl GetViewControlContainingPoint(D2DViewControl pViewControl, Point2F pPoint)
        {
            if(pViewControl != null)
            {
                lock (pViewControl._viewControls)
                {
                    int c = pViewControl._viewControls.Count;
                    for (int i = pViewControl._viewControls.Count - 1;
                        i >= 0;
                        i--)
                    {
                        if (c != pViewControl._viewControls.Count) System.Diagnostics.Debugger.Break();
                        if (pViewControl._viewControls[i].GetScreenBounds().Contains(pPoint))
                            return GetViewControlContainingPoint(pViewControl._viewControls[i], pPoint);
                    }
                }
            }
            return pViewControl;
        }

        public Point2F Location
        {
            get
            {
                return new Point2F(Bounds.Left, Bounds.Top);
            }
        }

        protected RectF _bounds;
        public RectF Bounds
        {
            get { return _bounds; }
            set { 
                _bounds = value; 
                OnSizeChanged(this, new EventArgs());
            }
        }

        public D2DViewControl Parent { private set; get; }

        public D2DViewControl()
        {
            _mouseDownControl = null;
            _viewControls = new List<D2DViewControl>();
        }

        protected virtual void PreRender() { }

        public void Render(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (_renderOk)
            {
                PreRender();

                pRenderTarget.CurrentBounds = GetScreenBounds();
                pRenderTarget.Renderer.PushAxisAlignedClip(GetAllowedRenderingArea(pRenderTarget.CurrentBounds) , AntialiasMode.PerPrimitive);

                OnRender(pRenderTarget);

                pRenderTarget.Renderer.PopAxisAlignedClip();
                
                for(int i = _viewControls.Count-1; i >= 0; i--)
                {
                    _viewControls[i].Render(pRenderTarget);
                }
            }
        }

        internal RectF GetAllowedRenderingArea(RectF pChildBounds)
            {
            if (Parent == null)
                return pChildBounds;
            
            return pChildBounds.Intersect(Parent.GetScreenBounds()); ;
        }

        internal RectF GetScreenBounds()
        {
            Point2F screenPoint = GetScreenPoint();
            return new RectF(screenPoint.X, screenPoint.Y, screenPoint.X + Bounds.Width, screenPoint.Y + Bounds.Height);
        }

        internal Point2F GetScreenPoint()
        {
            if (Parent == null)
                return Location;

            return GetScreenPoint(Location);
        }

        internal Point2F GetScreenPoint(Point2F pPointToAdd)
        {
            if (Parent == null)
                return pPointToAdd;

            return Parent.GetScreenPoint(
                new Point2F(
                    pPointToAdd.X + Parent.Bounds.Left, 
                    pPointToAdd.Y + Parent.Bounds.Top));
        }

        internal Point2F GetControlPointFromScreen(Point2F pPointToSubtract)
        {
            if (Parent != null)
            {
                return Parent.GetControlPointFromScreen(new Point2F(pPointToSubtract.X - Parent.Bounds.Left, pPointToSubtract.Y - Parent.Bounds.Top));
            }
            return pPointToSubtract;
        }

        internal Point2F GetControlPointFromScreen()
        {
            if (Parent == null)
                return new Point2F(Bounds.Left, Bounds.Height);

            return GetControlPointFromScreen(new Point2F(Bounds.Left, Bounds.Top));
        }

        internal Point2F ConvertScreenToControlPoint(Point2F pPointToConvert)
        {
            Point2F newPoint = new Point2F(pPointToConvert.X - Bounds.Left, pPointToConvert.Y - Bounds.Top);
            if (Parent != null)
            {
                return Parent.ConvertScreenToControlPoint(newPoint);
            }
            return newPoint;
        }


        protected abstract void OnRender(Direct2D.RenderTargetWrapper pRenderTarget);

        public SizeF Size { get { return new SizeF(_bounds.Width, _bounds.Height); } }
        public float Width { get { return _bounds.Width; } }
        public float Height { get { return _bounds.Height; } }
        public float X { get { return _bounds.Left; } }
        public float Y { get { return _bounds.Top; } }

        public void Clear()
        {
            lock (_viewControls)
            {
                _viewControls.Clear();
            }
        }

        public virtual void SendUpdate(ReactiveUpdate pReactiveUpdate) 
        {
            if (pReactiveUpdate is Touch)
            {
                Point2F newPoint = ConvertScreenToControlPoint((pReactiveUpdate as Touch).Location);

                if (pReactiveUpdate is TouchMove)
                {
                    OnTouchMove(new TouchMove(newPoint, (pReactiveUpdate as TouchMove).TouchDown, (pReactiveUpdate as TouchMove).Velocity));
                }
                else if (pReactiveUpdate is TouchGesture)
                {
                    if((pReactiveUpdate as TouchGesture).Gesture == GestureType.Click)
                        SetInputControl();
                    OnTouchGesture(new TouchGesture((pReactiveUpdate as TouchGesture).Gesture, newPoint));
                }
            }
            else if (pReactiveUpdate is Key)
            {
                OnKeyPressed(pReactiveUpdate as Key);
            }
        }

        protected virtual void OnKeyPressed(Key pKey) { }
        protected virtual void OnTouchGesture(TouchGesture pTouchGesture) { }
        protected virtual void OnTouchMove(TouchMove pTouchMove) { }
        protected virtual void OnInputLeave() { }

        private void SetInputControl()
        {
            if (HasInputControl != null)
                HasInputControl.OnInputLeave();

            HasInputControl = this;
        }

        public virtual void Dispose() { }

        public void AddViewControl(D2DViewControl pViewControl)
        {
            lock (_viewControls)
            {
                pViewControl.Parent = this;
                _viewControls.Add(pViewControl);
            }
        }

        public int IndexOf(D2DViewControl pViewControl)
        {
            return _viewControls.IndexOf(pViewControl);
        }

        protected virtual void OnRenderStart()
        {
            if (RenderStart != null)
                RenderStart(this, new EventArgs());
        }
        protected virtual void OnRenderStop()
        {
            if (RenderStop != null)
                RenderStop(this, new EventArgs());
        }

        public virtual void OnSizeChanged(object sender, EventArgs e) { }
    }
}
