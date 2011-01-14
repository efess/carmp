using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Geometry;

namespace CarMP.Reactive.Touch
{
    public class Touch : ReactiveUpdate
    {
        public Point Location { get; private set;}
        public float X { get { return Location.X;}}
        public float Y { get { return Location.Y;}}

        public Touch(Point pLocation)
        {
            Location = pLocation;
        }
    }
}
