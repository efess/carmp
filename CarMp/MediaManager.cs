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
        private const long TIMER_DEFAULT_INTERVAL = 1000;

        public static event MediaChangedHandler MediaChanged;
        public static event MediaProgressChangedHandler MediaProgressChanged;

        private static MediaListHistory MediaHistory;
        private static Dictionary<int, NHibernate.ISession> _DataSessions;
        private static IAudioController _audioController;

        public static MediaState CurrentState { get; private set; }

        private static System.Threading.Timer _progressTimer = new System.Threading.Timer((i) =>
            OnTimerTick(),
            null,
            0,
            TIMER_DEFAULT_INTERVAL
            );

        private static MediaListItem _currentPlayingItem;
        private static List<MediaListItem> _currentViewedList;
        private static List<MediaListItem> _currentPlayList;

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
            MediaHistory = new MediaListHistory();
            GetListHistory();
        }

        public static void Close()
        {
            _audioController.StopPlayback();
        }

        private static void GetListHistory()
        {
            IList<MediaHistory> lHistories = DataSession.
                CreateCriteria(typeof(MediaHistory))
                .List<MediaHistory>();

            //lHistories.Sort(new Comparison<ListHistory>(delegate(ListHistory lh, ListHistory lh2)
            //    {
            //        return lh.Index.CompareTo(lh2.Index);
            //    }
            //    ));

            foreach (MediaHistory history in lHistories)
            {
                MediaHistory.Push(new MediaListItem(history.DisplayString, history.ItemType, history.TargetId));
            }
        }

        private static void SaveListHistory()
        {
            for (int i = 0; i < MediaListHistory.Count; i++)
            {
                
            }
        }

        private static List<MediaListItem> GetFSRootLevelItems()
        {
            List<MediaListItem> fileSystemItems = new List<MediaListItem>();
            foreach (DriveInfo drives in FileSystem.GetDrives())
            {
                fileSystemItems.Add(new FileSystemItem(drives.Name , FileSystemItemType.HardDrive, drives.RootDirectory.FullName));
            }
            return fileSystemItems;
        }

        private static List<MediaListItem> GetMLRootLevelItems()
        {
            IList<MediaGroup> groups = DataSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupType", (int)MediaGroupType.Root)).List<MediaGroup>();
            List<MediaListItem> items = new List<MediaListItem>();
            for(int i = 0; i < groups.Count; i++)
            {
                items.Add(new DigitalMediaItem(groups[i].GroupName, DigitalMediaItemType.Root, groups[i].GroupId));
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
            CurrentState = MediaState.Stopped;
            _audioController.StopPlayback();
        }

        /// <summary>
        /// Starts Playing a stopped or paused song
        /// </summary>
        public static void StartPlayback()
        {
            if(_currentPlayingItem != null)
            {
                CurrentState = MediaState.Playing;
                _audioController.StartPlayback();
                _progressTimer.Change(0, TIMER_DEFAULT_INTERVAL);
            }
        }

        /// <summary>
        /// Pauses playback of a song
        /// </summary>
        public static void PausePlayback()
        {
            _audioController.PausePlayback();
        }

        public static void PlayMediaListItem(MediaListItem pListItem)
        {
            _currentPlayingItem = pListItem;
            if (pListItem is FileSystemItem)
            {
                var fileSystemItem = pListItem as FileSystemItem;
                PlayFromFile(fileSystemItem.FullPath);
            }
            else if (pListItem is DigitalMediaItem)
            {
                var mediaItem = pListItem as DigitalMediaItem;
                PlayFromMediaLibrary(mediaItem.TargetId);
            }
            SetPlayList();
        }

        private static void PlayFromFile(string pPath)
        {
            StartPlayback(pPath);
            MediaItem mediaItem = FileMediaInfo.GetInfo(new System.IO.FileInfo(pPath));
            mediaItem.Length = GetSongLength();
            OnMediaChanged(mediaItem);
        }

        private static void PlayFromMediaLibrary(int pTargetId)
        {
            DigitalMediaLibrary item = GetDigitalMedia(pTargetId);

            if (item == null)
                return;

            StartPlayback(item.Path);
            
            MediaItem mediaItem = new MediaItem();
            mediaItem.Artist = item.Artist;
            mediaItem.Album = item.Album;
            mediaItem.Title = item.Title;
            mediaItem.Length = GetSongLength();

            OnMediaChanged(mediaItem);
        }

        private static void SetPlayList()
        {
            List<MediaListItem> mediaListItems = new List<MediaListItem>();

            for (int i = 0; i < _currentViewedList.Count; i++)
            {
                mediaListItems.Add(_currentViewedList[i]);
            }
            _currentPlayList = mediaListItems;
        }


        /// <summary>
        /// Plays a file from the media library
        /// </summary>
        /// <param name="pLibraryId"></param>
        private static void StartPlayback(string pFullPath)
        {
            CurrentState = MediaState.Playing;
            _audioController.PlayFile(pFullPath);
            _progressTimer.Change(0, TIMER_DEFAULT_INTERVAL);
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
        private static List<MediaListItem> GetNewMediaList(int pGroupId)
        {
            List<MediaListItem> listOfItems = new List<MediaListItem>();
            IList<MediaGroup> mediaGroup = DataSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupId", pGroupId)).List<MediaGroup>();
            if (mediaGroup.Count == 0)
                return listOfItems;
            else
            {
                foreach (MediaGroupItem item in mediaGroup[0].GroupItem)
                {
                    listOfItems.Add(new DigitalMediaItem(item));
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

        private static List<MediaListItem> GetNewFSMediaList(string pPath)
        {
            List<MediaListItem> fileSystemItems = new List<MediaListItem>();

            List<string> directories = FileSystem.GetDirectories(pPath);
            fileSystemItems.AddRange(
                directories.ConvertAll<MediaListItem>(new Converter<string, MediaListItem>((str) => (new FileSystemItem(FileSystem.TopDirectory(str), FileSystemItemType.Directory, str))))
                );

            List<FileInfo> files = FileSystem.GetFiles(pPath, new List<string>() { "MP3" });
            fileSystemItems.AddRange(
                files.ConvertAll<MediaListItem>(new Converter<FileInfo, MediaListItem>((fileInfo) => (new FileSystemItem(fileInfo.Name, FileSystemItemType.AudioFile, fileInfo.FullName))))
                );

            return fileSystemItems;
        }

        public static void MediaNext()
        {
            if (_currentPlayingItem == null) return;

            int i = _currentPlayList.IndexOf(_currentPlayingItem);
            if (i < _currentPlayList.Count - 1)
                PlayMediaListItem(_currentPlayList[i + 1]);
            else
                PlayMediaListItem(_currentPlayList[0]);
        }


        public static void MediaPrevious()
        {
            if (_currentPlayingItem == null) return;

            int i = _currentPlayList.IndexOf(_currentPlayingItem);
            if (i > 0)
                PlayMediaListItem(_currentPlayList[i - 1]);
            else
                PlayMediaListItem(_currentPlayList[_currentPlayList.Count - 1]);
        }

        public static List<MediaListItem> GetNewList(MediaListItem pGroupItem)
        {
            List<MediaListItem> returnList = new List<MediaListItem>();
            if (pGroupItem is FileSystemItem)
            {
                var fileSystemItem = pGroupItem as FileSystemItem;
                returnList.AddRange(GetNewFSMediaList(fileSystemItem.FullPath));
            }
            else if (pGroupItem is DigitalMediaItem)
            {
                var mediaItem = pGroupItem as DigitalMediaItem;
                returnList.AddRange(GetNewMediaList(mediaItem.TargetId));
            }
            else if (pGroupItem is RootItem)
            {
                var rootItem = pGroupItem as RootItem;
                if (rootItem.ItemType == RootItemType.DigitalMediaLibrary)
                {
                    returnList.AddRange(GetMLRootLevelItems());
                }
                else if (rootItem.ItemType == RootItemType.FileSystem)
                {
                    returnList.AddRange(GetFSRootLevelItems());
                }
            }

            List<MediaListItem> songList = new List<MediaListItem>();
            for (int i = 0; i < returnList.Count; i++)
            {
                if(returnList[i].MediaType == MediaListItemType.Song)
                    songList.Add(returnList[i]);
            }
            _currentViewedList = songList;
            return returnList;
        }

        private static void OnMediaChanged(MediaItem pMediaItem)
        {
            if (MediaChanged != null)
            {
                MediaChanged(null, new MediaChangedArgs(pMediaItem));
            }
        }

        private static void OnTimerTick()
        {
            if (_audioController != null)
                OnMediaProgressChanged(GetCurrentPosition());
        }

        private static void OnMediaProgressChanged(int pSongPosition)
        {
            if (MediaProgressChanged != null)
                MediaProgressChanged(null, new MediaProgressChangedArgs(pSongPosition));
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
