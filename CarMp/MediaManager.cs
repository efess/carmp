using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjectLayer;

namespace CarMp
{
    public static class MediaManager
    {
        public static DragableListSelectHistory MediaListHistory;
        public static MediaListItem[] RootLevelItems =
            new MediaListItem[]
                {
                    new MediaListItem(
                    "Artists",
                    MediaItemSpecialTarget.AllArtists,
                    MediaItemType.Root,
                    MediaItemType.Artist),
                    new MediaListItem(
                    "Albums",
                    MediaItemSpecialTarget.AllAlbums,
                    MediaItemType.Root,
                    MediaItemType.Album),
                    new MediaListItem(
                    "Playlists",
                    MediaItemSpecialTarget.AllPlaylists,
                    MediaItemType.Root,
                    MediaItemType.Playlist),
                    new MediaListItem(
                    "AllSongs",
                    MediaItemSpecialTarget.AllSongs,
                    MediaItemType.Root,
                    MediaItemType.Song),
                    new MediaListItem(
                    "Directory",
                    MediaItemSpecialTarget.RootDirectories,
                    MediaItemType.Root,
                    MediaItemType.Directory)
                };

        public static void Initialize()
        {
            GetListHistory();
        }

        private static void GetListHistory()
        {
            ListHistorys lHistories = new ListHistorys();
            lHistories.Read();
            lHistories.Sort(new Comparison<ListHistory>(delegate(ListHistory lh, ListHistory lh2)
                {
                    return lh.ListIndex.CompareTo(lh2.ListIndex);
                }
                ));

            foreach (ListHistory history in lHistories)
            {
                MediaListItem mli = null;
                if (history.ItemSpecialTarget > 0)
                {
                    mli = new MediaListItem(history.DisplayString, (MediaItemSpecialTarget)history.ItemSpecialTarget, (MediaItemType)history.ItemType, (MediaItemType)history.ItemTargetType);
                }
                else
                {
                    mli = new MediaListItem(history.DisplayString, history.ItemTarget, (MediaItemType)history.ItemType, (MediaItemType)history.ItemTargetType);
                }
                MediaListHistory.Push(mli);
            }
        }

        private static void SaveListHistory()
        {
            ListHistorys lHistories = new ListHistorys();
            lHistories.Delete();

            for (int i = 0; i < MediaListHistory.Count; i++)
            {
                
            }
        }

        //private static List<MediaListItem> GetNewMediaList(int pListHistoryIndex)
        //{
        //    if (pListHistoryIndex == 0)
        //    {
        //        return RootLevelItems;
        //    }


        //}

        //private static DoQuery GetQueryConstraint(MediaListItem pItem)
        //{
        //    DoQuery query = new DoQuery();
        //    DoQueryConstraint constraint = new DoQueryConstraint()
        //    {
        //        Predicate = QueryPredicate.Equal
        //    };

        //    if (pItem.ItemSpecialTarget == MediaItemSpecialTarget.StringDefined)
        //    {
        //        switch (pItem.ItemType)
        //        {
        //            case MediaItemType.Album:
        //                constraint.Field = "Album";
        //                break;
        //            case MediaItemType.Album:
        //                constraint.Field = "Album";
        //                break;
        //            case MediaItemType.Directory:
        //                constraint.Field = "Directory";
        //                break;
        //            case MediaItemType.Playlist:
        //                constraint.Field = "Playlist";
        //                break;
        //            // This case doesn't exist - Root items have special target types
        //            //case MediaItemType.Root:
        //            //    constraint.Field = "Album";
        //            //    break;
        //            case MediaItemType.Song:
        //                constraint.Field = "Album";
        //                break;
        //        }
        //    }
        //    else
        //    {
        //    }
        //}
    }
}
