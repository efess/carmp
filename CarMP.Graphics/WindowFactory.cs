using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Implementation.OpenGL;
using CarMP.Graphics.Implementation.OpenTk;
using CarMP.Graphics.Implementation.Direct2D;
using CarMP.Implementation.WinFormsWindow;

namespace CarMP.Graphics
{
    public class WindowFactory
    {
        public IWindow GetWindow(string pRenderer)
        {
            switch (pRenderer.ToUpper())
            {
                case "OPENGL":
                    return new OpenGLRenderer();


                case "OPENTK":
                    return new FormHost(typeof(OpenTkRenderer));
                case "DIRECT2D":
                    return new FormHost(typeof(Direct2DRenderer));
                default:
                    throw new NotSupportedException("Window not supported.");
            }
        }
    }
}
