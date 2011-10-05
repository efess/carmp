using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using CarMP.MediaEntities;

namespace CarMP
{
    public class MediaHistory : IMediaSelection
    {
        public virtual int HistoryId { get; set; }
        public virtual int ListIndex { get; set; }
        public virtual string DisplayString { get; set; }
        public virtual MediaListItemType MediaType { get { return (MediaListItemType)MediaTypeInt; } set { MediaTypeInt = (int)value; ; } }
        public virtual string Key { get; set; }
        public virtual int ItemSpecificType { get; set; }
        public virtual string ObjectType { get; set; }

        public virtual int MediaTypeInt { get; set; }
    }
}
