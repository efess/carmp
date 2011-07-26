using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMP.DataObjects.View.Mappings
{
    public class AlbumArtPathMapping : ClassMap<AlbumArtPath>
    {
        public AlbumArtPathMapping()
        {
            ReadOnly();
            Table("AlbumArtPath");
            Id(x => x.Path);
            Map(x => x.Album);
            Map(x => x.Artist);
        }
    }
}
    