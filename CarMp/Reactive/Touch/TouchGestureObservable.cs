using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Reactive.Touch
{
    public class TouchGestureObservable : DefaultObservable<TouchGesture>
    {
        public void PushTouchGesture(TouchGesture pTouchGesture)
        {

            PushToObservers(pTouchGesture);
        }
    }
}
