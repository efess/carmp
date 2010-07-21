using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMp.Reactive.Touch
{
    public class TouchMove : Touch
    {
        public bool TouchDown { get; private set; }
        public float Velocity { get; private set; }

        public TouchMove(Point2F pLocation, bool pTouchDown, float pVelocity)
            : base(pLocation)
        {
            Velocity = pVelocity;
            TouchDown = pTouchDown;
        }
    }
}
