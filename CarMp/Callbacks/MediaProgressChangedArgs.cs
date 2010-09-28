using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP
{
    public class MediaProgressChangedArgs : EventArgs
    {
        public int MediaPosition { get; private set; }
        public MediaProgressChangedArgs(int pMediaPosition)
        {
            MediaPosition = pMediaPosition;
        }
    }
}
