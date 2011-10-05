using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CarMP.ViewControls;

namespace CarMP.MediaEntities
{
    [Serializable]
    public enum MediaListItemType
    {
        Group,
        Song
    }

    public abstract class MediaListItem : DragableListTextItem, IMediaSelection
    {
        public string ObjectType { get { return this.GetType().Name; } }
        public abstract int ItemSpecificType { get; }
        public abstract string Key { get; }
        public MediaListItemType MediaType { get; private set; }
        protected MediaListItem(string pDisplayString, MediaListItemType pListItemType)
            : base(pDisplayString)
        {
            MediaType = pListItemType;
        }
    }
}
