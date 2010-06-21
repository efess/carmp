using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp
{
    public class MediaListItemFactory
    {
        public MediaListItem CreateListItem(
            string pMediaListItemType,
            object pKey,
            int pMediaItemType,
            int pSpecificItemType,
            string pDisplayString)
        {
            switch (pMediaListItemType.ToUpper())
            {
                case "FILESYSTEMITEM":
                    return new FileSystemItem(pDisplayString, (FileSystemItemType)pSpecificItemType, (string)pKey);
                case "DIGITALMEDIAITEM":
                    return new DigitalMediaItem(pDisplayString, (DigitalMediaItemType)pSpecificItemType, Convert.ToInt32(pKey));
                case "ROOTITEM":
                    return new RootItem(pDisplayString, (RootItemType)pSpecificItemType);
                default:
                    break;
            }
            return null;
        }
    }
}
