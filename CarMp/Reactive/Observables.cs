using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMP.Reactive.Touch;
using CarMP.Reactive.KeyInput;

namespace CarMP.Reactive
{
    public class Observables
    {
        public readonly TouchGestureObservable ObsTouchGesture = new TouchGestureObservable();
        public readonly TouchMoveObservable ObsTouchMove = new TouchMoveObservable();
        public readonly KeyboardObservable ObsKeyInput = new KeyboardObservable();

        internal Observables()
        {
        }
    }
}
