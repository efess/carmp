using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Reactive.Touch
{
    public class TouchMoveObservable : DefaultObservable<TouchMove>
    {
        public void PushTouchMove(TouchMove pTouchMove)
        {
            _observerList.ForEach(obs => obs.OnNext(pTouchMove));
        }
    }
}
