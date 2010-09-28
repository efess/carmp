using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP
{
    /// <summary>
    /// TODO: Change this to only include a generic MediaItem object
    /// </summary>
    public class MediaChangedArgs : EventArgs
    {
        public MediaItem MediaDetail { get; private set;}
        
        public MediaChangedArgs(MediaItem pMediaDetail)
        {
            MediaDetail = pMediaDetail;
        }
    }
}
