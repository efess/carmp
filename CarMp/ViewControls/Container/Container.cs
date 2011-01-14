using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;

namespace CarMP.ViewControls
{
    public class Container : ViewControlCommonBase, IDisposable
    {
        private IBrush _borderBrush;
        public Container()
        {
            // Defaults
        }

        public override void Dispose()
        {
            if (_borderBrush != null)
                Helpers.GraphicsHelper.DisposeIfImplementsIDisposable(_borderBrush);
            base.Dispose();
        }

        public bool UseBorder { get; set; }

        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);

            if (UseBorder && _borderBrush == null)
                pRenderer.CreateBrush(Color.LightGray);

            if(UseBorder)
                pRenderer.DrawRectangle(_borderBrush, new Rectangle(this.Bounds.Left + 1, this.Bounds.Top + 1, this.Bounds.Width - 2, this.Bounds.Height - 2), 1);
        }        
    }
}
