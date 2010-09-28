using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CarMP.Reactive.KeyInput
{
    public class Key : ReactiveUpdate
    {
        public Keys DotNetKeysValue { get; private set; }
        public char Character { get; private set; }
        public Key(char pCharacter, Keys pKeyValue)
        {
            DotNetKeysValue = pKeyValue;
            Character = pCharacter;
        }
    }
}
