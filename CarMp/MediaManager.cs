using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMpMediaInfo;
using System.Collections;
using NHibernate.Criterion;
using System.IO;

namespace CarMp
{
    public static class MediaManager
    {
        public static DragableListSelectHistory MediaListHistory;
        private static Dictionary<int, NHibernate.ISession> _DataSessions;
        private static IAudioController _audioController;
        public static MediaState CurrentState { get; private set; }

        // This may be dangerous... Throughout the lifetime of the application 
        // datasessions will be created for each threadid, and never closed.
        // Will have to see what happens...

        // This started out as bad design, and this is mostly a bandaid.

        private static NHibernate.ISession DataSession
        {
            get
            {
                NHibernate.ISession dataSession;
                int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (!_DataSessions.ContainsKey(threadId))
                {
                    dataSession = Database.GetSession();
                    _DataSessions.Add(threadId, dataSession);
                }
                else
                    dataSession = _DataSessions[threadId];

                return dataSession;
            }
        }

        public static void Initialize(IAudioController pAudioController)
        {
            _audioController = pAudioController;
            _DataSessions = new Dictionary<int, NHibernate.ISession>();
            MediaListHistory = new DragableListSelectHistory();
            GetListHistory();
        }

        public static void Close()
        {
            _audioController.StopPlayback();
        }

        private static void GetListHistory()
        {
            IList<ListHistory> lHistories = DataSession.CreateCriteria(typeof(ListHistory)).List<ListHistory>();

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
            IList<MediaGroup> groups = DataSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupType", (int)MediaGroupType.Root)).List<MediaGroup>();
            List<MediaListItem> items = new List<MediaListItem>();
            for(int i = 0; i < groups.Count; i++)
            {
                items.Add(new MediaListItem(groups[i].GroupName, MediaItemType.Root, groups[i].GroupId));
            }
            return items;
        }

        public static void ClearMediaLibrary()
        {
            DataSession.CreateSQLQuery("DELETE FROM DigitalMediaLibrary").ExecuteUpdate();
        }

        public static void SaveMediaToLibrary(List<MediaItem> pMediaItems)
        {
            NHibernate.ISession dbSession = DataSession;

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
                        DataSession.Save(dml);
                    }
                    catch
                    {
                    }
                    //mediaGroupCreater.AddMediaItem(dml);
                }
                try
                {
                    DataSession.Transaction.Commit();
                }
                catch (Exception ex) { DebugHandler.HandleException(ex); }
                
                //{
                //    DebugHandler.DebugPrint("Cannot save Media list: " + dmls.ErrorString);
                //}
            }
        }

        /// <summary>
        /// Stop a song
        /// </summary>
        public static void StopPlayback()
        {
            _audioController.StopPlayback();
        }

        /// <summary>
        /// Starts Playing a stopped or paused song
        /// </summary>
        public static void StartPlayback()
        {
            _audioController.StartPlayback();
        }

        /// <summary>
        /// Pauses playback of a song
        /// </summary>
        public static void PausePlayback()
        {
            _audioController.PausePlayback();
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

            StartPlayback(item.Path);
        }

        /// <summary>
        /// Plays a file from the media library
        /// </summary>
        /// <param name="pLibraryId"></param>
        public static void StartPlayback(string pFullPath)
        {
            CurrentState = MediaState.Playing;
            _audioController.PlayFile(pFullPath);
        }

        public static int GetCurrentPosition()
        {
            return _audioController.GetCurrentPos();
        }

        public static int GetSongLength()
        {
            return _audioController.GetSongLength();
        }

        public static void SetCurrentPos(int pos)
        {
            _audioController.SetCurrentPos(pos);
        }
        public static List<MediaListItem> GetNewMediaList(int pGroupId)
        {
            
            List<MediaListItem> listOfItems = new List<MediaListItem>();
            IList<MediaGroup> mediaGroup = DataSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupId", pGroupId)).List<MediaGroup>();
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
            IList<DigitalMediaLibrary> digitalMedia = DataSession.CreateCriteria(typeof(DigitalMediaLibrary)).Add(Expression.Eq("LibraryId", pLibraryId)).List<DigitalMediaLibrary>();
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
