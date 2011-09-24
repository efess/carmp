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
using CarMP.Callbacks;
using CarMP.DataObjects;
using CarMP.Reactive.Messaging;

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
        private IMediaController _audioController;
        
        private List<MediaListItem> _currentViewedList;

        private int _currentMediaIndex;
        private List<MediaItem> _currentPlayList;

        public MediaItem? CurrentMediaItem
        {
            get{
                if (_currentPlayList != null
                    && _currentMediaIndex >= 0 
                    && _currentMediaIndex < _currentPlayList.Count)
                    return _currentPlayList[_currentMediaIndex];
                return null;
            }
        }

        public string GetCurrentLargeAlbumArt()
        {
            if(CurrentMediaItem != null)
            {
                var mediaItem = CurrentMediaItem.Value;
                var key = mediaItem.Artist + "|" + mediaItem.Album;
                var listItems = DatabaseInterface.DataSession
                    .CreateCriteria<Art>()
                    .Add(Expression.Eq("Key", key))
                    .Add(Expression.Eq("ArtType", ArtType.AlbumArtLarge))
                    .List<Art>();

                if( listItems.Count > 0) 
                    return listItems[0].Path;
            }
            return null;
        }

        public string GetCurrentSmallAlbumArt()
        {
            if (CurrentMediaItem != null)
            {
                var mediaItem = CurrentMediaItem.Value;
                var key = mediaItem.Artist + "|" + mediaItem.Album;
                var listItems = DatabaseInterface.DataSession
                    .CreateCriteria<Art>()
                    .Add(Expression.Eq("Key", key))
                    .List<Art>();

                if (listItems.Count > 0)
                    return listItems[0].Path;
            }
            return null;
        }


        public MediaManager(IMediaController pAudioController)
        {
            _audioController = pAudioController;
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
            if (_currnetSongLength <= 0)
                _currnetSongLength = GetSongLength();

            int songPosition = GetCurrentPosition();

            if (CurrentState == MediaState.Playing
                && (songPosition >= _currnetSongLength
                || songPosition <= 0))
            {
                MediaNext();

                songPosition = GetCurrentPosition();
            }

            OnMediaProgressChanged(songPosition);
        }
    
        public void SetList(int pListIndex)
        {
             OnListChangeRequest(pListIndex);
        }
        private void OnListChangeRequest(int pListIndex)
        {
            if (ListChangeRequest != null)
                ListChangeRequest(this, new ChangeMediaListArgs(pListIndex));
        }

        public void Close()
        {
            _audioController.StopPlayback();
        }

        private void GetListHistory()
        {
            return;
            IList<MediaHistory> lHistories = DatabaseInterface.DataSession.
                CreateCriteria(typeof(MediaHistory))
                .AddOrder(new Order("ListIndex", true))
                .List<MediaHistory>();

            lock (MediaListHistory)
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
                fileSystemItems.Add(new FileSystemItem(drives.Name, FileSystemItemType.HardDrive, drives.RootDirectory.FullName));
            }
            return fileSystemItems;
        }

        private List<MediaListItem> GetMLRootLevelItems()
        {
            IList<MediaGroup> groups = DatabaseInterface.DataSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupType", (int)MediaGroupType.Root)).List<MediaGroup>();
            List<MediaListItem> items = new List<MediaListItem>();
            for (int i = 0; i < groups.Count; i++)
            {
                items.Add(new DigitalMediaItem(groups[i].GroupName, DigitalMediaItemType.Root, groups[i].GroupId));
            }
            return items;
        }

        public void SetMediaHistory(int pListIndex, MediaListItem pMediaListItem)
        {
            lock(MediaListHistory)
                MediaListHistory.AddHistoryItem(pMediaListItem, pListIndex);
            
            OnHistoryChanged();
        }

        public void ClearMediaLibrary()
        {
            NHibernate.ISession dbSession = DatabaseInterface.DataSession;

            dbSession.CreateSQLQuery("DELETE FROM Art").ExecuteUpdate();
            dbSession.CreateSQLQuery("DELETE FROM DigitalMediaLibrary").ExecuteUpdate();
            dbSession.CreateSQLQuery("DELETE FROM MediaGroup").ExecuteUpdate();
            dbSession.CreateSQLQuery("DELETE FROM MediaGroupItem").ExecuteUpdate();
        }

        public void SaveMediaToLibrary(List<MediaItem> pMediaItems)
        {
            NHibernate.ISession dbSession = DatabaseInterface.DataSession;

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
                        dbSession.Save(dml);
                    }
                    catch
                    {
                    }
                    //mediaGroupCreater.AddMediaItem(dml);
                }
                try
                {
                    dbSession.Transaction.Commit();
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
            if (CurrentMediaItem != null)
            {
                _audioController.StartPlayback();

                // Sleep, give mediacontroller time to do its thing.
                Thread.Sleep(100);
                _currnetSongLength = GetSongLength();
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

        public void PlayMediaListItem(MediaListItem pMediaListItem)
        {
            SetPlayList();
            PlayMediaListItemInternal(pMediaListItem);
        }

        private void PlayMediaListItemInternal(MediaListItem pMediaListItem)
        {
            MediaItem? mediaItem = _currentPlayList.FirstOrDefault(item => item.InternalKey == pMediaListItem.Key);
            if (mediaItem != null)
            {
                var index = _currentPlayList.IndexOf(mediaItem.Value);

                _currentMediaIndex = index;
                PlayCurrentMediaItem();
            }
        }

        private void PlayCurrentMediaItem()
        {
            MediaItem? currentItem = CurrentMediaItem;
            if (currentItem == null)
                return;

            StartPlayback(currentItem.Value.Path);

            OnMediaChanged(currentItem.Value);
        }

        private MediaItem GetMediaItem(MediaListItem pMediaListItem)
        {
            if (pMediaListItem is FileSystemItem)
            {
                return GetMediaItemFromFile(pMediaListItem as FileSystemItem);
            }
            else if (pMediaListItem is DigitalMediaItem)
            {
                return GetMediaItemFromMediaLibrary(pMediaListItem as DigitalMediaItem);
            }
            
            throw new Exception("MediaListItem type can not be converted to MediaItem");
        }

        private MediaItem GetMediaItemFromFile(FileSystemItem pFileItem)
        {
            return FileMediaInfo.GetInfo(new System.IO.FileInfo(pFileItem.FullPath));
        }

        private MediaItem GetMediaItemFromMediaLibrary(DigitalMediaItem pMediaItem)
        {
            DigitalMediaLibrary item = null;
            if (pMediaItem.LibraryItem != null)
                item = pMediaItem.LibraryItem;
            else
                item = GetDigitalMedia(pMediaItem.TargetId);
            MediaItem mediaItem = new MediaItem();
            mediaItem.Artist = item.Artist;
            mediaItem.Album = item.Album;
            mediaItem.Title = item.Title;
            mediaItem.Path = item.Path;
            mediaItem.Track = item.Track;
            mediaItem.Genre = item.Genre;
            
            return mediaItem;
        }

        private void PlayFromFile(string pPath)
        {
            
            StartPlayback(pPath);
            MediaItem mediaItem = FileMediaInfo.GetInfo(new System.IO.FileInfo(pPath));
            OnMediaChanged(mediaItem);
        }

        private void SetPlayList()
        {
            List<MediaItem> mediaListItems = new List<MediaItem>();

            for (int i = 0; i < _currentViewedList.Count; i++)
            {
                var item = GetMediaItem(_currentViewedList[i]);
                item.InternalKey = _currentViewedList[i].Key;
                item.DisplayName = _currentViewedList[i].DisplayString;

                mediaListItems.Add(item);
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

            // Sleep, give mediacontroller time to do its thing.
            Thread.Sleep(100);
            _currnetSongLength = GetSongLength();

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
            
            // Manually building out "Hql" here to eager load objects in one transaction
            string hql = @"from MediaGroup mg " +
                " inner join fetch mg.GroupItem as gi" +
                " left outer join fetch gi.LibraryEntry as le" +
                " where mg.GroupId = :groupId";

            //switch (AppMain.Settings.SortMedia)
            //{
            //    case MediaSort.FileName:
            //        hql += " order by le.FileName";
            //        break;
            //    case MediaSort.Title:
            //        hql += " order by le.Title";
            //        break;
            //    case MediaSort.Track:
            //        hql += " order by le.Track";
            //        break;
            //}
        
            IList<MediaGroup> mediaGroup = DatabaseInterface
                .DataSession
                .CreateQuery(hql)
                .SetParameter("groupId", pGroupId)
                .List<MediaGroup>();
            
            if (mediaGroup.Count == 0)
                return listOfItems;

            IEnumerable<MediaGroupItem> list = mediaGroup[0].GroupItem;

            switch (AppMain.Settings.SortMedia)
            {
                case MediaSort.FileName:
                    list = list.OrderBy(a => a.LibraryEntry != null ? a.LibraryEntry.FileName : string.Empty);
                    break;
                case MediaSort.Title:
                    list = list.OrderBy(a => a.LibraryEntry != null ? a.LibraryEntry.Title : string.Empty);
                    break;
                case MediaSort.Track:
                    list = list.OrderBy(a => a.LibraryEntry != null
                        && a.LibraryEntry.Track != null 
                        ? a.LibraryEntry.Track.PadLeft(4, '0') 
                        : "0000");
                    break;
            }

            foreach (MediaGroupItem item in list)
            {
                listOfItems.Add(new DigitalMediaItem(item));
            }

            return listOfItems;
        }

        private DigitalMediaLibrary GetDigitalMedia(int pLibraryId)
        {
            IList<DigitalMediaLibrary> digitalMedia = DatabaseInterface.DataSession.CreateCriteria(typeof(DigitalMediaLibrary)).Add(Expression.Eq("LibraryId", pLibraryId)).List<DigitalMediaLibrary>();
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
            if (CurrentMediaItem == null) return;

            if (_currentMediaIndex < _currentPlayList.Count - 1)
            {
                _currentMediaIndex++;
                PlayCurrentMediaItem();
            }
            else
            {
                _currentMediaIndex = 0;
                PlayCurrentMediaItem();
            }
        }


        public void MediaPrevious()
        {
            if (CurrentMediaItem == null) return;

            if (_currentMediaIndex > 0)
            {
                _currentMediaIndex--;
                PlayCurrentMediaItem();
            }
            else
            {
                _currentMediaIndex = _currentPlayList.Count - 1;
                PlayCurrentMediaItem();
            }
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
                if (returnList[i].MediaType == MediaListItemType.Song)
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
            AppMain.Messanger.SendMessage(new Message(null, MessageType.MediaChange, pMediaItem));
            if (MediaChanged != null)
            {
                MediaChanged(null, new MediaChangedArgs(pMediaItem));
            }
        }

        private void OnHistoryChanged()
        {
            AppMain.Messanger.SendMessage(new Message(null, MessageType.MediaHistoryChange, null));
        }

        private void OnTimerTick()
        {
            if (_timerHit)
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
            AppMain.Messanger.SendMessage(new Message(null, MessageType.MediaProgress, pSongPosition));
            if (MediaProgressChanged != null)
            {
                MediaProgressChanged(null, new MediaProgressChangedArgs(pSongPosition));
            }
        }
    }

    public struct MediaItem
    {
        public string DisplayName { get; set; }
        public string DeviceId { get; set;}
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public int Length { get; set; }
        public int Kbps { get; set; }
        public int Channels { get; set; }
        public int Frequency { get; set; }
        public string Genre { get; set; }
        public string Track { get; set; }
        public string InternalKey { get; set; }
    }
}
