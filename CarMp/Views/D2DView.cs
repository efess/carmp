using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.ViewControls;
using CarMP.Graphics.Geometry;

namespace CarMP.Views
{
    public abstract class D2DView : ViewControlCommonBase
    {
        public abstract string Name { get; }
        protected D2DView(Size pWindowSize)
        {
            Bounds = new Rectangle(0, 0, pWindowSize.Width, pWindowSize.Height);
        }
    }
}
