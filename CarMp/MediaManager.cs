using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Criterion;
using System.Threading;
using System.IO;
using CarMP.MediaEntities;
using CarMP.IO;
using CarMP.MediaInfo;

namespace CarMP
{
    public class MediaManager
    {
        private const long TIMER_DEFAULT_INTERVAL = 1000;

        public event ChangeMediaListHandler ListChangeRequest;
        public event ChangeMediaListHandler ListChanged;
        public event MediaChangedHandler MediaChanged;
        public event MediaProgressChangedHandler MediaProgressChanged;

        public MediaHistoryManager MediaListHistory { private set; get; }

        public MediaState CurrentState { get; private set; }

        private int _currnetSongLength;
        private int _lastSongPosition;
        private Timer _progressTimer;
        private bool _timerHit;
        private Dictionary<int, NHibernate.ISession> _DataSessions;
        private IMediaController _audioController;

        private MediaListItem _currentPlayingItem;
        private List<MediaListItem> _currentViewedList;
        private List<MediaListItem> _currentPlayList;

        // This may be dangerous... Throughout the lifetime of the application 
        // datasessions will be created for each threadid, and never closed.
        // Will have to see what happens...

        // This started out as bad design, and this is mostly a bandaid.

        private NHibernate.ISession DataSession
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

        public MediaManager(IMediaController pAudioController)
        {
            _audioController = pAudioController;
            _DataSessions = new Dictionary<int, NHibernate.ISession>();
            MediaListHistory = new MediaHistoryManager(new MediaListItemFactory());
            GetListHistory();

            _progressTimer = new System.Threading.Timer(
                (i) => OnTimerTick(),
                null,
                0,
                TIMER_DEFAULT_INTERVAL
            );
        }

        private void ProcessSongPosition()
        {
            if(_currnetSongLength <= 0)
                _currnetSongLength = GetSongLength();

            int songPosition = GetCurrentPosition();

            if (CurrentState == MediaState.Playing
                && (songPosition >= _currnetSongLength
                || songPosition <= 0))
            {
                MediaNext();

                // Sleep, give mediacontroller time to do its thing.
                Thread.Sleep(100);

                songPosition = GetCurrentPosition();
                _currnetSongLength = GetSongLength();
            }

            _lastSongPosition = songPosition;

            OnMediaProgressChanged(songPosition);
        }

        public void SetList(int pListIndex)
        {
            OnListChangeRequest(pListIndex);
        }
        private void OnListChangeRequest(int pListIndex)
        {
            if(ListChangeRequest != null)
                ListChangeRequest(this, new ChangeMediaListArgs(pListIndex));
        }

        public void Close()
        {
            _audioController.StopPlayback();
        }

        private void GetListHistory()
        {
            IList<MediaHistory> lHistories = DataSession.
                CreateCriteria(typeof(MediaHistory))
                .AddOrder(new Order("ListIndex", true))
                .List<MediaHistory>();

            foreach (MediaHistory history in lHistories)
            {
                MediaListHistory.AddHistoryItem(history);
            }

        }

        private List<MediaListItem> GetFSRootLevelItems()
        {
            List<MediaListItem> fileSystemItems = new List<MediaListItem>();
            foreach (DriveInfo drives in FileSystem.GetDrives())
            {
                fileSystemItems.Add(new FileSystemItem(drives.Name , FileSystemItemType.HardDrive, drives.RootDirectory.FullName));
            }
            return fileSystemItems;
        }

        private List<MediaListItem> GetMLRootLevelItems()
        {
            IList<MediaGroup> groups = DataSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupType", (int)MediaGroupType.Root)).List<MediaGroup>();
            List<MediaListItem> items = new List<MediaListItem>();
            for(int i = 0; i < groups.Count; i++)
            {
                items.Add(new DigitalMediaItem(groups[i].GroupName, DigitalMediaItemType.Root, groups[i].GroupId));
            }
            return items;
        }

        public void SetMediaHistory(int pListIndex, MediaListItem pMediaListItem)
        {
            MediaListHistory.AddHistoryItem(pMediaListItem, pListIndex);
        }

        public void ClearMediaLibrary()
        {
            DataSession.CreateSQLQuery("DELETE FROM DigitalMediaLibrary").ExecuteUpdate();
        }

        public void SaveMediaToLibrary(List<MediaItem> pMediaItems)
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
        public void StopPlayback()
        {
            CurrentState = MediaState.Stopped;
            _audioController.StopPlayback();
        }

        /// <summary>
        /// Starts Playing a stopped or paused song
        /// </summary>
        public void StartPlayback()
        {
            if(_currentPlayingItem != null)
            {
                _audioController.StartPlayback();
                _progressTimer.Change(0, TIMER_DEFAULT_INTERVAL);

                CurrentState = MediaState.Playing;
            }
        }

        /// <summary>
        /// Pauses playback of a song
        /// </summary>
        public void PausePlayback()
        {
            _audioController.PausePlayback();
        }

        public void PlayMediaListItem(MediaListItem pListItem)
        {
            PlayMediaListItemInternal(pListItem);
            SetPlayList();
        }

        private void PlayMediaListItemInternal(MediaListItem pListItem)
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
        }

        private void PlayFromFile(string pPath)
        {
            StartPlayback(pPath);
            MediaItem mediaItem = FileMediaInfo.GetInfo(new System.IO.FileInfo(pPath));
            mediaItem.Length = GetSongLength();
            OnMediaChanged(mediaItem);
        }

        private void PlayFromMediaLibrary(int pTargetId)
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

        private void SetPlayList()
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
        private void StartPlayback(string pFullPath)
        {
            _audioController.PlayFile(pFullPath);
            _progressTimer.Change(0, TIMER_DEFAULT_INTERVAL);
            CurrentState = MediaState.Playing;
        }

        public int GetCurrentPosition()
        {
            return _audioController.GetCurrentPos();
        }

        public int GetSongLength()
        {
            return _audioController.GetSongLength();
        }

        public void SetCurrentPos(int pos)
        {
            _audioController.SetCurrentPos(pos);
        }
        private List<MediaListItem> GetNewMediaList(int pGroupId)
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

        private DigitalMediaLibrary GetDigitalMedia(int pLibraryId)
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

        private List<MediaListItem> GetNewFSMediaList(string pPath)
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

        public void MediaNext()
        {
            if (_currentPlayingItem == null) return;

            int i = _currentPlayList.IndexOf(_currentPlayingItem);
            if (i < _currentPlayList.Count - 1)
                PlayMediaListItemInternal(_currentPlayList[i + 1]);
            else
                PlayMediaListItemInternal(_currentPlayList[0]);
        }


        public void MediaPrevious()
        {
            if (_currentPlayingItem == null) return;

            int i = _currentPlayList.IndexOf(_currentPlayingItem);
            if (i > 0)
                PlayMediaListItemInternal(_currentPlayList[i - 1]);
            else
                PlayMediaListItemInternal(_currentPlayList[_currentPlayList.Count - 1]);
        }

        public List<MediaListItem> GetNewList(MediaListItem pGroupItem)
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

        public void ExecuteListChanged(int pListIndex) { OnListChanged(pListIndex); }
        private void OnListChanged(int pListIndex)
        {
            if (ListChanged != null)
                ListChanged(this, new ChangeMediaListArgs(pListIndex));
        }

        private void OnMediaChanged(MediaItem pMediaItem)
        {
            if (MediaChanged != null)
            {
                MediaChanged(null, new MediaChangedArgs(pMediaItem));
            }
        }

        private void OnTimerTick()
        {
            if(_timerHit)
                return;

            if (_audioController != null)
            {
                _timerHit = true;
                ProcessSongPosition();
                
                Thread.Sleep(700);

                _timerHit = false;
            }
        }

        private void OnMediaProgressChanged(int pSongPosition)
        {
            if (MediaProgressChanged != null )
            {
                MediaProgressChanged(null, new MediaProgressChangedArgs(pSongPosition));
             
            }
        }
        
        //private List<MediaListItem> GetNewMediaList(int pListHistoryIndex)
        //{
        //    if (pListHistoryIndex == 0)
        //    {
        //        return RootLevelItems;
        //    }


        //}

        //private DoQuery GetQueryConstraint(MediaListItem pItem)
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
