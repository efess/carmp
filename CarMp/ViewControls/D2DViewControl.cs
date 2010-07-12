using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using CarMp.Views;
using CarMp.Reactive.Touch;

namespace CarMp.ViewControls
{
    public abstract class D2DViewControl : IDisposable
    {
        public event EventHandler RenderStart;
        public event EventHandler RenderStop;

        protected D2DViewControl _mouseDownControl;
        private List<D2DViewControl> _viewControls;

//        private static TouchObservables _touchObs;

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

        //internal static void SetTouchObservables(TouchObservables pTouchObs)
        //{
        //    _touchObs = pTouchObs;
        //}

        //public virtual void SuscribeTouchMove(Action<TouchMove> pTouchMoveAction)
        //{
        //    InternalSubscribeTouchMove(pTouchMoveAction);
        //}

        //public virtual void SuscribeTouchGesture(Action<TouchGesture> pTouchGestureAction)
        //{
        //    InternalSubscribeTouchGesture(pTouchGestureAction);
        //}

        //private void InternalSubscribeTouchMove(Action<TouchMove> pTouchMoveAction)
        //{
        //    _touchObs.ObsTouchMove.Where<TouchMove>(tm =>
        //        {
        //            return _visible && GetScreenBounds().Contains(tm.Location);
        //        })
        //        .Select<TouchMove, TouchMove>(tm => 
        //        {
        //            return new TouchMove(GetControlPointFromScreen(tm.Location), tm.TouchDown, tm.Velocity);
        //        })
        //        .Subscribe(pTouchMoveAction);            
        //}

        //private void InternalSubscribeTouchGesture(Action<TouchGesture> pTouchGestureAction)
        //{
        //    _touchObs.ObsTouchGesture.Where<TouchGesture>(tm =>
        //        {
        //            return _visible && GetScreenBounds().Contains(tm.Location);
        //        })
        //        .Select<TouchGesture, TouchGesture>(tm =>
        //        {
        //            return new TouchGesture(tm.Gesture, GetControlPointFromScreen(tm.Location));
        //        })
        //        .Subscribe(pTouchGestureAction);
        //}

        public D2DViewControl GetViewControlContainingPoint(PointF pPoint)
        {
            D2DViewControl control = GetViewControlContainingPoint(this, pPoint);

            if (control == this && GetScreenBounds().Contains(pPoint))
                return this;

            return control == this ? null : control;
        }

        public D2DViewControl GetViewControlContainingPoint(D2DViewControl pViewControl, PointF pPoint)
        {
            if(pViewControl != null)
            {
                lock (pViewControl._viewControls)
                {
                    for (int i = pViewControl._viewControls.Count - 1;
                        i >= 0;
                        i--)
                    {
                        if (pViewControl._viewControls[i].GetScreenBounds().Contains(pPoint))
                            return GetViewControlContainingPoint(pViewControl._viewControls[i], pPoint);
                    }
                }
            }
            return pViewControl;
        }

        protected RectangleF _bounds;
        public RectangleF Bounds
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

                pRenderTarget.Renderer.PushAxisAlignedClip(GetAllowedRenderingArea(pRenderTarget.CurrentBounds) , SlimDX.Direct2D.AntialiasMode.Aliased);

                OnRender(pRenderTarget);

                pRenderTarget.Renderer.PopAxisAlignedClip();
                
                for(int i = _viewControls.Count-1; i >= 0; i--)
                {
                    _viewControls[i].Render(pRenderTarget);
                }
            }
        }

        internal RectangleF GetAllowedRenderingArea(RectangleF pChildBounds)
        {
            if (Parent == null)
                return pChildBounds;
            
            RectangleF newRect = pChildBounds;
            newRect.Intersect(Parent.Bounds);
            return newRect;
        }

        internal RectangleF GetScreenBounds()
        {
            return new RectangleF(GetScreenPoint(), Bounds.Size);
        }

        internal PointF GetScreenPoint()
        {
            if (Parent == null)
                return Bounds.Location;

            return GetScreenPoint(Bounds.Location);
        }

        internal PointF GetScreenPoint(PointF pPointToAdd)
        {
            if (Parent == null)
                return pPointToAdd;

            return Parent.GetScreenPoint(
                new PointF(
                    pPointToAdd.X + Parent.Bounds.X, 
                    pPointToAdd.Y + Parent.Bounds.Y));
        }

        internal PointF GetControlPointFromScreen(PointF pPointToSubtract)
        {
            if (Parent != null)
            {
                return Parent.GetControlPointFromScreen(new PointF(pPointToSubtract.X - Parent.Bounds.X, pPointToSubtract.Y - Parent.Bounds.Y));
            }
            return pPointToSubtract;
        }

        internal PointF GetControlPointFromScreen()
        {
            if (Parent == null)
                return Bounds.Location;

            return GetControlPointFromScreen(Bounds.Location);
        }

        internal PointF ConvertScreenToControlPoint(PointF pPointToConvert)
        {
            PointF newPoint = new PointF(pPointToConvert.X - Bounds.X, pPointToConvert.Y - Bounds.Y);
            if (Parent != null)
            {
                return Parent.ConvertScreenToControlPoint(newPoint);
            }
            return newPoint;
        }


        protected abstract void OnRender(Direct2D.RenderTargetWrapper pRenderTarget);

        public SizeF Size { get { return _bounds.Size; } }
        public PointF Location { get { return _bounds.Location; } }
        public float Width { get { return _bounds.Width; } }
        public float Height { get { return _bounds.Height; } }
        public float X { get { return _bounds.X; } }
        public float Y { get { return _bounds.Y; } }

        public void Clear()
        {
            lock (_viewControls)
            {
                _viewControls.Clear();
            }
        }

        public virtual void SendTouch(Touch pTouch) 
        {
            PointF newPoint = ConvertScreenToControlPoint(pTouch.Location);

            if (pTouch is TouchMove)
            {
                OnTouchMove(new TouchMove(newPoint, (pTouch as TouchMove).TouchDown,(pTouch as TouchMove).Velocity));
            }
            else if (pTouch is TouchGesture)
            {
                OnTouchGesture(new TouchGesture((pTouch as TouchGesture).Gesture,newPoint));
            }
        }

        protected virtual void OnTouchGesture(TouchGesture pTouchGesture) { }
        protected virtual void OnTouchMove(TouchMove pTouchMove) { }

        public virtual void Dispose() { }

        public void AddViewControl(D2DViewControl pViewControl)
        {
            pViewControl.Parent = this;
            _viewControls.Add(pViewControl);
        }

        public int IndexOf(D2DViewControl pViewControl)
        {
            return _viewControls.IndexOf(pViewControl);
        }

        private MouseEventArgs TransformMouseEventArgs(MouseEventArgs e)
        {
            PointF newPoint = GetControlPointFromScreen(e.Location);
            return new MouseEventArgs(e.Button, e.Clicks, Convert.ToInt32(newPoint.X), Convert.ToInt32(newPoint.Y), e.Delta);
        }

        protected virtual void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            //for (int i = _viewControls.Count - 1; i >= 0; i--)
            //{
            //    if (_viewControls[i].Bounds.Contains(e.Location))
            //    {
            //        _viewControls[i].OnMouseDown(e);
            //        _mouseDownControl = _viewControls[i];
            //        break;
            //    }
            //}
        }

        protected virtual void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            //if (_mouseDownControl != null)
            //    _mouseDownControl.OnMouseMove(e);

            //for (int i = _viewControls.Count - 1; i >= 0; i--)
            //{
            //    if (_mouseDownControl != _viewControls[i]
            //        && _viewControls[i].Bounds.Contains(e.Location))
            //    {
            //        _viewControls[i].OnMouseMove(e);
            //        break;
            //    }
            //}
        }

        protected virtual void OnMouseUp(System.Windows.Forms.MouseEventArgs e) { }

        internal void MouseMove(MouseEventArgs e)
        {
            OnMouseMove(TransformMouseEventArgs(e));
        }
        internal void MouseDown(MouseEventArgs e)
        {
            OnMouseDown(TransformMouseEventArgs(e));
        }
        internal void MouseUp(MouseEventArgs e)
        {
            OnMouseUp(TransformMouseEventArgs(e));
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
