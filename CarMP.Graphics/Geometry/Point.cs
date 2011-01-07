using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float pX, float pY)
        {
            X = pX;
            Y = pY;
        }
    }
}
