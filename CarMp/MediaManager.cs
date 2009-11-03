using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataObjectLayer;
using CarMpMediaInfo;
using System.Collections;
using NHibernate.Criterion;
using System.IO;

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

        public static void Close()
        {
            WinampController.StopPlayback();
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

        public static List<FileSystemItem> GetFSRootLevelItems()
        {
            List<FileSystemItem> fileSystemItems = new List<FileSystemItem>();
            foreach (DriveInfo drives in FileSystem.GetDrives())
            {
                fileSystemItems.Add(new FileSystemItem(drives.Name , FileSystemItemType.HardDrive, drives.RootDirectory.FullName));
            }
            return fileSystemItems;
        }

        public static List<MediaListItem> GetMLRootLevelItems()
        {
            IList<MediaGroup> groups = ApplicationMain.DbSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupType", (int)MediaGroupType.Root)).List<MediaGroup>();
            List<MediaListItem> items = new List<MediaListItem>();
            for(int i = 0; i < groups.Count; i++)
            {
                items.Add(new MediaListItem(groups[i].GroupName, MediaItemType.Root, groups[i].GroupId));
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

        /// <summary>
        /// Plays a file from the media library
        /// </summary>
        /// <param name="pLibraryId"></param>
        public static void StartPlayback(int pLibraryId)
        {
            DigitalMediaLibrary item =GetDigitalMedia(pLibraryId);
            
            if (item == null)
                return;

            WinampController.Playfile(item.Path);
        }

        /// <summary>
        /// Plays a file from the media library
        /// </summary>
        /// <param name="pLibraryId"></param>
        public static void StartPlayback(string pFullPath)
        {
            WinampController.Playfile(pFullPath);
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

        private static DigitalMediaLibrary GetDigitalMedia(int pLibraryId)
        {
            IList<DigitalMediaLibrary> digitalMedia = ApplicationMain.DbSession.CreateCriteria(typeof(DigitalMediaLibrary)).Add(Expression.Eq("LibraryId", pLibraryId)).List<DigitalMediaLibrary>();
            if (digitalMedia.Count > 0)
            {
                return digitalMedia[0] as DigitalMediaLibrary;
            }
            else
            {
                return null;
            }
        }

        public static List<FileSystemItem> GetNewFSMediaList(string pPath)
        {
            List<FileSystemItem> fileSystemItems = new List<FileSystemItem>();

            List<string> directories = FileSystem.GetDirectories(pPath);
            fileSystemItems.AddRange(
                directories.ConvertAll<FileSystemItem>(new Converter<string, FileSystemItem>((str) => (new FileSystemItem(FileSystem.TopDirectory(str), FileSystemItemType.Directory, str))))
                );

            List<FileInfo> files = FileSystem.GetFiles(pPath, new List<string>() { "MP3" });
            fileSystemItems.AddRange(
                files.ConvertAll<FileSystemItem>(new Converter<FileInfo, FileSystemItem>((fileInfo) => (new FileSystemItem(fileInfo.Name, FileSystemItemType.AudioFile, fileInfo.FullName))))
                );

            return fileSystemItems;
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
