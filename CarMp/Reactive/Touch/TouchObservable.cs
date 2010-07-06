using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CarMp.Reactive.Touch
{
    public class TouchObservables
    {
        public readonly TouchGestureObservable ObsTouchGesture = new TouchGestureObservable();
        public readonly TouchMoveObservable ObsTouchMove = new TouchMoveObservable();

        internal TouchObservables()
        {
        }
    }
}
