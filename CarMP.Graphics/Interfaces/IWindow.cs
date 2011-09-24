using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Interfaces
{
    public interface IWindow
    {
        void SetProcessKeyPress(Action<char, Keys> pCallback);
        void SetProcessMouseMove(Action<Point> pCallback);
        void SetProcessMouseDown(Action<Point> pCallback);
        void SetProcessMouseUp(Action<Point> pCallback);

        void CreateWindow(Point pWindowLocation, Size pWindowSize);
        IRenderer Renderer { get; }
    }
}
