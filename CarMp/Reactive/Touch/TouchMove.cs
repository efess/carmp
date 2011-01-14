using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;

namespace CarMP.Reactive.Touch
{
    public class TouchMove : Touch
    {
        public bool TouchDown { get; private set; }
        public Velocity Velocity { get; private set; }

        public TouchMove(Point pLocation, bool pTouchDown, Velocity pVelocity)
            : base(pLocation)
        {
            Velocity = pVelocity;
            TouchDown = pTouchDown;
        }
    }
}
