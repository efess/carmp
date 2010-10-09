using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.DataObjects.View
{
    public class AlbumArtPath
    {
        public virtual string Album { get; set; }
        public virtual string Artist { get; set; }
        public virtual string Path { get; set; }
    }
}
