using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;

namespace CarMP.Reactive.Touch
{
    public enum GestureType
    {
        SwipeUp,
        SwipeDown,
        SwipeLeft,
        SwipeRight,
        Click,
        Hold
    }

    public class TouchGesture : Touch
    {
        public GestureType Gesture { get; private set; }

        public TouchGesture(GestureType pGesture, Point pLocation)
            : base(pLocation)
        {
            Gesture = pGesture;
        }
    }
}
