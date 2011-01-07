using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public class Ellipse
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float RadiusX { get; set; }
        public float RadiusY { get; set; }
        public Point Point
        {
            get
            {
                return new Point(X, Y);
            }
        }
        public Ellipse(float pX, float pY, float pRadiusX, float pRadiusY)
        {
            X = pX;
            Y = pY;
            RadiusX = pRadiusX;
            RadiusY = pRadiusY;
        }
    }
}
