using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Reactive.KeyInput
{
    public class KeyboardObservable : DefaultObservable<Key>
    {
        public void PushKeyInput(Key pKeyInput)
        {
            _observerList.ForEach(obs => obs.OnNext(pKeyInput));
        }
    }
}
