using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMP
{
    public class MediaGroupItemMap : ClassMap<MediaGroupItem>
    {
        public MediaGroupItemMap()
        {
            Id(x => x.GroupItemId);
            //Map(x => x.GroupId);
            Map(x => x.ItemName);
            Map(x => x.ItemType);
            Map(x => x.LibraryId);
            Map(x => x.NextGroupId);
            References(x => x.Group, "GroupId");
                
        }
    }
}
