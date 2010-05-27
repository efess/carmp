using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CarMpControls
{
    public abstract class ViewControlBase
    {
        private RectangleF _bounds;
        public RectangleF Bounds
        {
            get { return _bounds; }
            set { 
                _bounds = value; 
                OnSizeChanged(this, new EventArgs());
            }
        }

        public SizeF Size { get { return _bounds.Size; } }
        public PointF Location { get { return _bounds.Location; } }
        public float Width { get { return _bounds.Width; } }
        public float Height { get { return _bounds.Height; } }
        public float X { get { return _bounds.X; } }
        public float Y { get { return _bounds.Y; } }

        public virtual void OnMouseMove(MouseEventArgs e) {}
        public virtual void OnMouseUp(MouseEventArgs e) { }
        public virtual void OnMouseDown(MouseEventArgs e) { }
        public virtual void Invalidate() { }
        public virtual void OnSizeChanged(object sender, EventArgs e) { }
    }
}
