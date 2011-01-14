using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public struct EllipseI
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int RadiusX { get; set; }
        public int RadiusY { get; set; }
        public PointI Point
        {
            get
            {
                return new PointI(X, Y);
            }
        }
        public EllipseI(int pX, int pY, int pRadiusX, int pRadiusY)
            : this()
        {
            X = pX;
            Y = pY;
            RadiusX = pRadiusX;
            RadiusY = pRadiusY;
        }
        public EllipseI(PointI pPoint, int pRadiusX, int pRadiusY)
            : this()
        {
            X = pPoint.X;
            Y = pPoint.Y;
            RadiusX = pRadiusX;
            RadiusY = pRadiusY;
        }
    }
    public struct  Ellipse
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
            : this()
        {
            X = pX;
            Y = pY;
            RadiusX = pRadiusX;
            RadiusY = pRadiusY;
        }
        public Ellipse(Point pPoint, float pRadiusX, float pRadiusY)
            : this()
        {
            X = pPoint.X;
            Y = pPoint.Y;
            RadiusX = pRadiusX;
            RadiusY = pRadiusY;
        }
    }
}
