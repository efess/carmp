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
                    MediaItemType.Root),
                    new MediaListItem(
                    "Albums",
                    MediaItemSpecialTarget.AllAlbums,
                    MediaItemType.Root),
                    new MediaListItem(
                    "Playlists",
                    MediaItemSpecialTarget.AllPlaylists,
                    MediaItemType.Root),
                    new MediaListItem(
                    "AllSongs",
                    MediaItemSpecialTarget.AllSongs,
                    MediaItemType.Root),
                    new MediaListItem(
                    "Directory",
                    MediaItemSpecialTarget.RootDirectories,
                    MediaItemType.Root)
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
                    mli = new MediaListItem(history.DisplayString, (MediaItemSpecialTarget)history.ItemSpecialTarget, (MediaItemType)history.ItemType);
                }
                else
                {
                    mli = new MediaListItem(history.DisplayString, history.ItemTarget, (MediaItemType)history.ItemType);
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

        private static List<MediaListItem> GetNewMediaList()
        {

        }
    }
}
