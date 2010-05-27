using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct2D;

namespace CarMpControls
{
    public interface IViewControl
    {
        void Render(RenderTarget pRenderTarget, System.Drawing.RectangleF pDrawingBounds);
    }
}
