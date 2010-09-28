using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMP
{
    public class MediaHistoryMapping : ClassMap<MediaHistory>
    {
        public MediaHistoryMapping()
        {
            Id(x => x.HistoryId);
            Map(x => x.ListIndex);
            Map(x => x.DisplayString);
            Map(x => x.MediaType);
            Map(x => x.Key).Column("ObjectKey");
            Map(x => x.ItemSpecificType).Column("ObjectSpecificType");
            Map(x => x.ObjectType);
        }
    }
}

