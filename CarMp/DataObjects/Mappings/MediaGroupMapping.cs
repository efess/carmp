using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMP
{
    public class MediaGroupMapping : ClassMap<MediaGroup>
    {
        public MediaGroupMapping()
        {
            Id(x => x.GroupId);
            Map(x => x.GroupName);
            Map(x => x.Description);
            Map(x => x.GroupPath);
            Map(x => x.GroupType);
            HasMany(x => x.GroupItem)
                .KeyColumn("GroupId")
              .Inverse()
              .Cascade.All();
        }
    }
}
