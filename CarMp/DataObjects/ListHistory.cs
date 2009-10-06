using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Criterion;

namespace CarMp
{
    public class ListHistory
    {
        public virtual int HistoryId {get;set;}
        public virtual int Index { get; set; }
        public virtual string DisplayString { get; set; }
        public virtual MediaItemType ItemType { get; set; }
        public virtual int TargetId { get; set; }
    }
}
