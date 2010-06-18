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

    public class FileSystemItem : MediaListItem
    {
        /// <summary>
        /// Id representing next item id when clicked.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public FileSystemItemType ItemType;

        /// <summary>
        /// Representing a unique ID to define this object
        /// </summary>
        public override object Key
        {
            get { return (object)FullPath; }
        }

        public FileSystemItem(string pDisplayString, FileSystemItemType pItemType, string pPath)
            : base(pDisplayString, TranslateMediaEnum(pItemType))
        {
            ItemType = pItemType; 
            FullPath = pPath;
        }
        public FileSystemItem(string pDisplayString, MediaListItemType pMediaItemType, string pKey, int pSpecificType)
            : base(pDisplayString, pMediaItemType)
        {
            ItemType = (FileSystemItemType)pSpecificType;
            FullPath = pKey;
        }

        private static MediaListItemType TranslateMediaEnum(FileSystemItemType pFsItemType)
        {
            switch (pFsItemType)
            {
                case FileSystemItemType.Directory:
                case FileSystemItemType.HardDrive:
                case FileSystemItemType.MemoryCard:
                case FileSystemItemType.CdDrive:
                    return MediaListItemType.Group;
                default:
                    return MediaListItemType.Song;
            }
        }
    }
}
