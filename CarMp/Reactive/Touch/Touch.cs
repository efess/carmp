using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CarMp.Reactive.Touch
{
    public class Touch
    {
        public PointF Location { get; private set;}
        public float X { get { return Location.X;}}
        public float Y { get { return Location.Y;}}

        public Touch(PointF pLocation)
        {
            Location = pLocation;
        }
    }
}
