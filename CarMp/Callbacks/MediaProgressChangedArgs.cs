using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp
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
