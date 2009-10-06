using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMp
{
    public class ListHistoryMapping : ClassMap<ListHistory>
    {
        public ListHistoryMapping()
        {
            Id(x => x.HistoryId);
            Map(x => x.Index, "ListIndex");
            Map(x => x.DisplayString);
            Map(x => x.ItemType);
            Map(x => x.TargetId);
        }
    }
}

