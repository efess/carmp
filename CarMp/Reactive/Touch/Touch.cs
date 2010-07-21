using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMp.Reactive.Touch
{
    public class Touch
    {
        public Point2F Location { get; private set;}
        public float X { get { return Location.X;}}
        public float Y { get { return Location.Y;}}

        public Touch(Point2F pLocation)
        {
            Location = pLocation;
        }
    }
}
