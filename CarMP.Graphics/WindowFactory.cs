using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Implementation.OpenGL;

namespace CarMP.Graphics
{
    public class WindowFactory
    {
        public IWindow GetWindow(string pWindowType)
        {
            switch (pWindowType.ToUpper())
            {
                case "OPENGL":
                    return new OpenGLRenderer();

                default:
                    throw new NotSupportedException("Window not supported.");
            }
        }
    }
}
