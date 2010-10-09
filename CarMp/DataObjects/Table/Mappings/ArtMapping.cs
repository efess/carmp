using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMP.DataObjects.Mappings
{
    public class ArtMapping : ClassMap<Art>
    {
        public ArtMapping()
        {
            Id(x => x.Path);
            Map(x => x.ArtType);
            Map(x => x.Key);
        }
    }
}
