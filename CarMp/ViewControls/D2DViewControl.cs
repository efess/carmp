using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMP.Views;
using CarMP.Reactive.Touch;
using CarMP.Graphics.Geometry;
using CarMP.Reactive;
using CarMP.Reactive.KeyInput;
using CarMP.Reactive.Messaging;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls
{
    public abstract class D2DViewControl : IDisposable
    {
        public event EventHandler RenderStart;
        public event EventHandler RenderStop;

        internal static D2DViewControl HasInputControl { get;  set; }
        protected D2DViewControl _mouseDownControl;

        private List<D2DViewControl> _viewControls;
        public List<D2DViewControl> ViewControls { get { lock (_viewControls) return _viewControls; } }

        private bool _canRender = false;
        public void StartRender()
        {
            _canRender = true;
            OnRenderStart();
        }
        public void StopRenderer()
        {
            _canRender = false;
            OnRenderStop();
        }

        public D2DViewControl GetViewControlContainingPoint(Point pPoint)
        {
            D2DViewControl control = GetViewControlContainingPoint(this, pPoint);

            if (control == this && GetScreenBounds().Contains(pPoint))
                return this;

            return control == this ? null : control;
        }

        public D2DViewControl GetViewControlContainingPoint(D2DViewControl pViewControl, Point pPoint)
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

        public Point Location
        {
            get
            {
                return new Point(Bounds.Left, Bounds.Top);
            }
        }

        protected Rectangle _bounds;
        public Rectangle Bounds
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
            _canRender = true;
            _mouseDownControl = null;
            _viewControls = new List<D2DViewControl>();
        }

        protected virtual void PreRender() { }

        public void Render(IRenderer pRenderer)
        {
            if (_canRender)
            {
                PreRender();

                pRenderer.CurrentBounds = GetScreenBounds();
                //pRenderer.Renderer.PushAxisAlignedClip(GetAllowedScreenRenderingArea() , AntialiasMode.PerPrimitive);
                pRenderer.PushClip(GetAllowedScreenRenderingArea());

                OnRender(pRenderer);

                pRenderer.PopClip();

                lock(_viewControls)
                    for(int i = _viewControls.Count-1; i >= 0; i--)
                    {
                        if(!Bounds.Intersect(_viewControls[i].Bounds).IsEmpty())
                            _viewControls[i].Render(pRenderer);
                    }
            }
        }

        internal Rectangle GetAllowedScreenRenderingArea()
        {
            return GetAllowedScreenRenderingArea(GetScreenBounds());
        }

        private Rectangle GetAllowedScreenRenderingArea(Rectangle pOtherBounds)
        {
            if (Parent == null)
                return pOtherBounds;

            Rectangle newRectf = Parent.GetScreenBounds().Intersect(pOtherBounds);

            return newRectf.IsEmpty() ? newRectf : Parent.GetAllowedScreenRenderingArea(newRectf);
        }

        internal Rectangle GetScreenBounds()
        {
            return GetScreenBounds(Bounds);
        }

        private Rectangle GetScreenBounds(Rectangle pBounds)
        {
            Point screenPoint = GetScreenPoint();
            return new Rectangle(screenPoint.X, screenPoint.Y, pBounds.Width, pBounds.Height);
        }

        internal Point GetScreenPoint()
        {
            if (Parent == null)
                return Location;

            return GetScreenPoint(Location);
        }

        internal Point GetScreenPoint(Point pPointToAdd)
        {
            if (Parent == null)
                return pPointToAdd;

            return Parent.GetScreenPoint(
                new Point(
                    pPointToAdd.X + Parent.Bounds.Left, 
                    pPointToAdd.Y + Parent.Bounds.Top));
        }

        internal Point ConvertScreenToControlPoint(Point pPointToConvert)
        {
            Point newPoint = new Point(pPointToConvert.X - Bounds.Left, pPointToConvert.Y - Bounds.Top);
            if (Parent != null)
            {
                return Parent.ConvertScreenToControlPoint(newPoint);
            }
            return newPoint;
        }


        protected abstract void OnRender(IRenderer pRenderer);

        public event Action InputLeave;
        public Size Size { get { return new Size(_bounds.Width, _bounds.Height); } }
        public float Width { get { return _bounds.Width; } }
        public float Height { get { return _bounds.Height; } }
        public float X { get { return _bounds.Left; } }
        public float Y { get { return _bounds.Top; } }

        public void Clear()
        {
            //lock (_viewControls)
            //    foreach (D2DViewControl viewControl in _viewControls)
            //    {
            //        viewControl.Clear();
            //        if (viewControl is IMessageObserver)
            //            (viewControl as IMessageObserver).DisposeUnsubscriber.Dispose();
            //        viewControl.Dispose();
            //    }
            lock(_viewControls)
                _viewControls.Clear();
        }

        public void Remove(D2DViewControl pViewControl)
        {
            lock (_viewControls)
                _viewControls.Remove(pViewControl);
        }

        public virtual void SendUpdate(ReactiveUpdate pReactiveUpdate) 
        {
            if (pReactiveUpdate is Touch)
            {
                Point newPoint = ConvertScreenToControlPoint((pReactiveUpdate as Touch).Location);

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
        protected virtual void OnInputLeave() { if (InputLeave != null) InputLeave(); }
        
        private void SetInputControl()
        {
            if (HasInputControl != null)
                HasInputControl.OnInputLeave();

            HasInputControl = this;
        }

        public virtual void Dispose()
        {
        }

        public void AddViewControl(D2DViewControl pViewControl)
        {
            lock (_viewControls)
            {
                pViewControl.Parent = this;
                _viewControls.Add(pViewControl);
                if (pViewControl is IMessageObserver)
                    AppMain.Messanger.AddMessageObserver(pViewControl as IMessageObserver);
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
