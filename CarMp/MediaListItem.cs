using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using CarMp.ViewControls;

namespace CarMp
{
    public enum MediaItemType
    {
        /// <summary>
        /// Determines what type of item this is
        /// </summary>
        Root = 0,
        Playlist = 2,
        Artist = 3,
        Album = 4,
        Song = 5
    }

    public class MediaListItem : DragableListTextItem
    {
        /// <summary>
        /// Id representing next item id when clicked.
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public MediaItemType ItemType;

        /// <summary>
        /// Creates a list item from a Group Item object
         /// </summary>
        /// <param name="pGroupItem"></param>
        public MediaListItem(MediaGroupItem pGroupItem)
        {
            this.DisplayString = pGroupItem.ItemName;
            this.ItemType = (MediaItemType)pGroupItem.ItemType;

            if (ItemType == MediaItemType.Song)
            {
                if (pGroupItem.LibraryId == 0)
                    throw new Exception("Library Id must not be 0");
                TargetId = pGroupItem.LibraryId;
            }
            else
            {
                if(pGroupItem.NextGroupId == 0)
                    throw new Exception("Next Group must not be 0");
                TargetId = pGroupItem.NextGroupId;
            }
        }

        public MediaListItem(string pDisplayString, MediaItemType pItemType, int pItemTargetId)
        {
            DisplayString = pDisplayString;
            ItemType = pItemType;
            TargetId = pItemTargetId;
        }
    }
}
