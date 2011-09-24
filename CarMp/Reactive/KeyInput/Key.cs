using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics;

namespace CarMP.Reactive.KeyInput
{
    public class Key : ReactiveUpdate
    {
        public Keys KeysValue { get; private set; }
        public char Character { get; private set; }
        public Key(char pCharacter, Keys pKeyValue)
        {
            KeysValue = pKeyValue;
            Character = pCharacter;
        }   
    }
}
