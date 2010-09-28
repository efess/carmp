using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.ViewControls;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.Views
{
    public abstract class D2DView : D2DViewControl
    {
        public abstract string Name { get; }
        protected D2DView(SizeF pWindowSize)
        {
            Bounds = new RectF(0, 0, pWindowSize.Width, pWindowSize.Height);
        }
    }
}
