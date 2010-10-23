using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.DataObjects
{
    public class Art
    {
        public virtual string Key { get; set; }
        public virtual int ArtType { get; set; }
        public virtual string Path { get; set; }
    }

    public enum ArtType
    {
        AlbumArtSmall,
        AlbumArtLarge
    }
}
