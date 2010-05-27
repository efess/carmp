using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMp.ViewControls;

namespace CarMp
{
    public enum FileSystemItemType
    {
        HardDrive,
        CdDrive,
        MemoryCard,
        Directory,
        AudioFile,
        CdTrack
    }

    public class FileSystemItem : DragableListTextItem
    {
        /// <summary>
        /// Id representing next item id when clicked.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public FileSystemItemType ItemType;

        public FileSystemItem(string pDisplayString, FileSystemItemType pItemType, string pPath)
        {
            DisplayString = pDisplayString;
            ItemType = pItemType;
            FullPath = pPath;
        }
    }
}
