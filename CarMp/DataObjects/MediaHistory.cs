using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;

namespace CarMp
{
    public class MediaHistory
    {
        public virtual int HistoryId { get; set; }
        public virtual int ListIndex { get; set; }
        public virtual string DisplayString { get; set; }
        public virtual int MediaType { get; set; }
        public virtual string Key { get; set; }
        public virtual int ItemSpecificType { get; set; }
        public virtual string ObjectType { get; set; }
    }
}
