using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp
{
    public enum DigitalMediaItemType
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

    public class DigitalMediaItem : MediaListItem
    {
        /// <summary>
        /// Id representing next item id when clicked.
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Representing a unique ID to define this object
        /// </summary>
        public override object Key
        {
            get { return (object)TargetId; }
        }

        public override int ItemSpecificType
        {
            get { return (int)ItemType; }
        }
        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public DigitalMediaItemType ItemType;

        /// <summary>
        /// Creates a list item from a Group Item object
         /// </summary>
        /// <param name="pGroupItem"></param>
        public DigitalMediaItem(MediaGroupItem pGroupItem)
            : base(pGroupItem.ItemName, TranslateMediaEnum((DigitalMediaItemType)pGroupItem.ItemType))
        {
            this.ItemType = (DigitalMediaItemType)pGroupItem.ItemType;

            if (ItemType == DigitalMediaItemType.Song)
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

        public DigitalMediaItem(string pDisplayString, DigitalMediaItemType pItemType, int pItemTargetId)
            : base(pDisplayString, TranslateMediaEnum(pItemType))
        {
            ItemType = pItemType;
            TargetId = pItemTargetId;
        }

        public DigitalMediaItem(string pDisplayString, MediaListItemType pMediaItemType, string pKey, int pSpecificType)
            : base(pDisplayString, pMediaItemType)
        {
            ItemType = (DigitalMediaItemType)pSpecificType;
            TargetId = Convert.ToInt32(pKey);
        }

        private static MediaListItemType TranslateMediaEnum(DigitalMediaItemType pDigitalItemType)
        {
            switch (pDigitalItemType)
            {
                case DigitalMediaItemType.Song:
                    return MediaListItemType.Song;
                default:
                    return MediaListItemType.Group;
            }
        }
    }
}
