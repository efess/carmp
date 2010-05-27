using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMp.ViewControls;

namespace CarMp.Views
{
    public abstract class D2DView : D2DViewControl
    {
        protected D2DView(System.Drawing.Size pWindowSize)
        {
            Bounds = new System.Drawing.RectangleF(0, 0, pWindowSize.Width, pWindowSize.Height);
 
        }

    }
}
