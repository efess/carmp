using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMp
{
    public class MediaHistoryMapping : ClassMap<MediaHistory>
    {
        public MediaHistoryMapping()
        {
            Id(x => x.ListIndex);
            Map(x => x.DisplayString);
            Map(x => x.MediaType);
            Map(x => x.Key);
            Map(x => x.ItemSpecificType);
            Map(x => x.ObjectType);
        }
    }
}

