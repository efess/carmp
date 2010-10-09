using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;

namespace CarMP
{
    public enum MediaGroupType
    {
        Root,
        ArtistAlbum
    }

    public class MediaGroup
    {
        public virtual int GroupId { get; set; }
        public virtual string GroupName { get; set; }
        public virtual string Description {get;set;}
        public virtual string GroupPath { get; set; }
        public virtual int GroupType { get; set; }
        public virtual IList<MediaGroupItem> GroupItem { get; set; }

        public MediaGroup()
        {
            GroupItem = new List<MediaGroupItem>();
        }
        
        public virtual void AddGroupItem(MediaGroupItem pGroupItem)
        {
            GroupItem.Add(pGroupItem);
        }
    }
}
