using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMp.ViewControls;

namespace CarMp
{
    public enum RootItemType
    {
        DigitalMediaLibrary,
        FileSystem
    }

    public class RootItem : MediaListItem
    {
        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public RootItemType ItemType;

        public RootItem(string pDisplayString, RootItemType pItemType)
            : base(pDisplayString, MediaListItemType.Group)
        {
            ItemType = pItemType;
        }

        /// <summary>
        /// Representing a unique ID to define this object
        /// </summary>
        public override string Key
        {
            get { return ItemType.ToString(); }
        }

        public override int ItemSpecificType
        {
            get { return (int)ItemType; }
        }
    }
}
