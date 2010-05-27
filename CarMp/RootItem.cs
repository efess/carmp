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

    public class RootItem : DragableListTextItem
    {
        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public RootItemType ItemType;

        public RootItem(string pDisplayString, RootItemType pItemType)
        {
            DisplayString = pDisplayString;
            ItemType = pItemType;
        }
    }
}
