using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct2D;

namespace CarMpControls
{
    public interface IView
    {
        void Render(RenderTarget pRenderTarget);
    }
}
