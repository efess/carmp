using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.Views
{
    public class OverlayView : D2DView
    {
        public OverlayView(SizeF pWindowSize)
            : base(pWindowSize)
        {
        }

        public override string Name
        {
            get { return "Overlay"; }
        }
    }
}
