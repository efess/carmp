using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP
{
    public class MediaGroupItem
    {
        public virtual int GroupId { get; set; }
        public virtual int GroupItemId { get; private set; }
        public virtual int ItemType { get; set; }
        public virtual string ItemName { get; set; }
       // public virtual int LibraryId { get; set; }
        public virtual int NextGroupId { get; set; }
        public virtual MediaGroup Group { get; set; }
        public virtual DigitalMediaLibrary LibraryEntry { get; set; }
    }
}
