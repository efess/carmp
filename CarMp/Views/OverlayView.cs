using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;

namespace CarMP.Views
{
    public class OverlayView : D2DView
    {
        public OverlayView(Size pWindowSize)
            : base(pWindowSize)
        {
        }

        public override string Name
        {
            get { return "Overlay"; }
        }
    }
}
