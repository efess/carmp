using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Implementation.Direct2D;
using CarMP.Graphics.Implementation.OpenTk;

namespace CarMP.Graphics
{
    public class RendererFactory
    {
        public virtual IRenderer GetRenderer(string pRenderer, IntPtr pWindowHandle)
        {
            switch (pRenderer.ToUpper())
            {
                case "DIRECT2D":
                    return new Direct2DRenderer(pWindowHandle);
                case "CLUTTER":
                    return null;
                case "OPENTK":
                case "OPENGL":
                    return new OpenTkRenderer(pWindowHandle);
                default:
                    return null;
            }
        }
    }
}
