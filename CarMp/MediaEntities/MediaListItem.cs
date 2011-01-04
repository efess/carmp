using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CarMP.ViewControls;

namespace CarMP.MediaEntities
{
    public enum MediaListItemType
    {
        Group,
        Song
    }

    public abstract class MediaListItem : DragableListTextItem
    {
        public abstract int ItemSpecificType { get; }
        public abstract string Key { get; }
        internal MediaListItemType MediaType { get; private set; }
        protected MediaListItem(string pDisplayString, MediaListItemType pListItemType)
            : base(pDisplayString)
        {
            MediaType = pListItemType;
        }
    }
}
