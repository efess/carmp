using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using CarMp.Views;

namespace CarMp.ViewControls
{
    public abstract class D2DViewControl
    {
        protected D2DViewControl _mouseDownControl;
        protected List<D2DViewControl> _viewControls;

        private bool _renderOk = false;
        public void StartRender()
        {
            _renderOk = true;
        }

        public D2DViewControl GetViewControlContainingPoint(PointF pPoint)
        {

            for (int i = _viewControls.Count - 1;
                i >= 0;
                i--)
            {
                if(_viewControls[i].GetScreenBounds().Contains(pPoint))
                    return _viewControls[i];
            }

            if (GetScreenBounds().Contains(pPoint))
                return this;

            return null;
        }

        private RectangleF _bounds;
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

        public void Render(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (_renderOk)
            {
                pRenderTarget.CurrentBounds = GetScreenBounds();
                
                pRenderTarget.Renderer.PushAxisAlignedClip(pRenderTarget.CurrentBounds, SlimDX.Direct2D.AntialiasMode.Aliased);

                OnRender(pRenderTarget);
                pRenderTarget.Renderer.PopAxisAlignedClip();

                for(int i = _viewControls.Count-1; i >= 0; i--)
                {
                    _viewControls[i].Render(pRenderTarget);
                }
            }
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
            if (Parent != null)
            {
                return Parent.GetScreenPoint(new PointF(pPointToAdd.X + Parent.Bounds.X, pPointToAdd.Y + Parent.Bounds.Y));
            }
            return pPointToAdd;
        }

        protected abstract void OnRender(Direct2D.RenderTargetWrapper pRenderTarget);
        
        public SizeF Size { get { return _bounds.Size; } }
        public PointF Location { get { return _bounds.Location; } }
        public float Width { get { return _bounds.Width; } }
        public float Height { get { return _bounds.Height; } }
        public float X { get { return _bounds.X; } }
        public float Y { get { return _bounds.Y; } }

        public void AddViewControl(D2DViewControl pViewControl)
        {
            pViewControl.Parent = this;
            _viewControls.Add(pViewControl);
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

        protected virtual void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            //if (_mouseDownControl != null)
            //{
            //    _mouseDownControl.OnMouseUp(e);
            //    _mouseDownControl = null;
            //}

        }

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

        public virtual void OnSizeChanged(object sender, EventArgs e) { }

        private MouseEventArgs TransformMouseEventArgs(MouseEventArgs e)
        {
            PointF newPoint = GetScreenPoint(e.Location);
            return new MouseEventArgs(e.Button, e.Clicks, Convert.ToInt32(newPoint.X), Convert.ToInt32(newPoint.Y), e.Delta);
        }
    }
}
