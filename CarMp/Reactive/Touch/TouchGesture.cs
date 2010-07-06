using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CarMp.Reactive.Touch
{
    public enum GestureType
    {
        SwipeUp,
        SwipeDown,
        SwipeLeft,
        SwipeRight,
        Click,
    }

    public class TouchGesture : Touch
    {
        public GestureType Gesture { get; private set; }

        public TouchGesture(GestureType pGesture, PointF pLocation)
            : base(pLocation)
        {
            Gesture = pGesture;
        }
    }
}
