using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjectLayer;
using CarMpMediaInfo;
using System.Collections;
using NHibernate.Criterion;

namespace CarMp
{
    public static class MediaManager
    {
        public static DragableListSelectHistory MediaListHistory;

        public static void Initialize()
        {
            MediaListHistory = new DragableListSelectHistory();
            GetListHistory();
        }

        private static void GetListHistory()
        {
            IList<ListHistory> lHistories = ApplicationMain.DbSession.CreateCriteria(typeof(ListHistory)).List<ListHistory>();

            //lHistories.Sort(new Comparison<ListHistory>(delegate(ListHistory lh, ListHistory lh2)
            //    {
            //        return lh.Index.CompareTo(lh2.Index);
            //    }
            //    ));

            foreach (ListHistory history in lHistories)
            {
                MediaListHistory.Push(new MediaListItem(history.DisplayString, history.ItemType, history.TargetId));
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

        public static MediaListItem[] GetRootLevelItems()
        {
            IList<MediaGroup> groups = ApplicationMain.DbSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupType", (int)MediaGroupType.Root)).List<MediaGroup>();
            MediaListItem[] items = new MediaListItem[groups.Count];
            for(int i = 0; i < groups.Count; i++)
            {
                items[i] = new MediaListItem(groups[i].GroupName, MediaItemType.Root, groups[i].GroupId);
            }
            return items;
        }

        public static void ClearMediaLibrary()
        {
            ApplicationMain.DbSession.CreateSQLQuery("DELETE FROM DigitalMediaLibrary").ExecuteUpdate();
        }

        public static void SaveMediaToLibrary(List<MediaItem> pMediaItems)
        {
            NHibernate.ISession dbSession = ApplicationMain.DbSession;

            DigitalMediaLibrarys dmls = new DigitalMediaLibrarys();
            // MediaGroupCreater mediaGroupCreater = new MediaGroupCreater();

            // retreive all stores and display them
            using (dbSession.BeginTransaction())
            {
                foreach (MediaItem item in pMediaItems)
                {
                    DigitalMediaLibrary dml = new DigitalMediaLibrary();

                    dml.Artist = item.Artist;
                    dml.Album = item.Album;
                    dml.FileName = item.FileName;
                    dml.Frequency = item.Frequency;
                    dml.Genre = item.Genre;
                    dml.Kbps = item.Kbps;
                    dml.Path = item.Path;
                    dml.Title = item.Title;
                    dml.Track = item.Track;
                    dml.DeviceId = item.DeviceId;

                    try
                    {
                        ApplicationMain.DbSession.Save(dml);
                    }
                    catch
                    {
                    }
                    //mediaGroupCreater.AddMediaItem(dml);
                }
                try
                {
                    ApplicationMain.DbSession.Transaction.Commit();
                }
                catch (Exception ex) { DebugHandler.HandleException(ex); }
                
                //{
                //    DebugHandler.DebugPrint("Cannot save Media list: " + dmls.ErrorString);
                //}
            }
        }

        public static void StartPlayback(int pLibraryId)
        {
            // Play song.
        }

        public static List<MediaListItem> GetNewMediaList(int pGroupId)
        {
            
            List<MediaListItem> listOfItems = new List<MediaListItem>();
            IList<MediaGroup> mediaGroup = ApplicationMain.DbSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupId", pGroupId)).List<MediaGroup>();
            if (mediaGroup.Count == 0)
                return listOfItems;
            else
            {
                foreach (MediaGroupItem item in mediaGroup[0].GroupItem)
                {
                    listOfItems.Add(new MediaListItem(item));
                }
            }

            return listOfItems;
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

    public struct MediaItem
    {
        public string DeviceId;
        public string Path;
        public string FileName;
        public string Title;
        public string Artist;
        public string Album;
        public int Length;
        public int Kbps;
        public int Channels;
        public int Frequency;
        public string Genre;
        public string Track;
    }
}
