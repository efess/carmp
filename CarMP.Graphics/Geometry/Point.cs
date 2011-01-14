using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public struct PointI
    {
        public int X { get; set; }
        public int Y { get; set; }

        public PointI(int pX, int pY)
            : this()
        {
            X = pX;
            Y = pY;
        }
    }
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float pX, float pY)
            : this()
        {
            X = pX;
            Y = pY;
        }
    }
}
