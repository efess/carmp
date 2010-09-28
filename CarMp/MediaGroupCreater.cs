using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NHibernate.Criterion;
using NHibernate;
using System.Diagnostics;

namespace CarMP
{
    public class MediaGroupCreater
    {
        public event ProgressDelegate ProgressChanged;
        public event FinishHandler CreationFinished;

        public void ReCreateArtistAlbumGroupCreation()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            ISession dataSession = Database.GetSession();

            OnProgressChanged(0, "Deleting records");
            dataSession.CreateSQLQuery("DELETE FROM MediaGroup").ExecuteUpdate();
            OnProgressChanged(1, "Deleting records");
            dataSession.CreateSQLQuery("DELETE FROM MediaGroupItem").ExecuteUpdate();
            
            OnProgressChanged(2, "Creating root group");
            dataSession.CreateSQLQuery(
                @"insert into mediagroup (groupname, description, grouppath, GroupType)"
                + @" values ('AllArtists','','\ALLARTISTS\', 0)").ExecuteUpdate();
            
            OnProgressChanged(3, "Creating root group");
            dataSession.CreateSQLQuery(
                @"insert into mediagroup (groupname, description, grouppath, GroupType)"
                + @" values ('AllAlbums','','\ALLALBUMS\', 0)").ExecuteUpdate();
            
            OnProgressChanged(4, "Creating root group");
            dataSession.CreateSQLQuery(
                @"insert into mediagroup (groupname, description, grouppath, GroupType)"
                + @" values ('AllSongs','','\ALLSONGS\', 0)").ExecuteUpdate();
            
            OnProgressChanged(8, "Creating Artist groups");
            //Create artist groups
            dataSession.CreateSQLQuery(
                @"insert into mediagroup (groupname, description, grouppath, GroupType) "
                + @"select artist, '', '\ALLARTISTS\' || UPPER(artist) || '\', 1 "
                + @"from digitalmedialibrary "
                + @"group by UPPER(artist) COLLATE NOCASE").ExecuteUpdate();

            OnProgressChanged(8, "Creating \\Album groups");
            // Create Album groups
            dataSession.CreateSQLQuery(
                @"insert into mediagroup (groupname, description, grouppath, GroupType) "
                + @"select album, '', '\ALLALBUMS\' || UPPER(album) || '\',1 "
                + @"from digitalmedialibrary "
                + @"group by UPPER(album) COLLATE NOCASE").ExecuteUpdate();
            
            OnProgressChanged(8, "Creating Artist\\Album groups");
            // Create Artist\Album groups
            dataSession.CreateSQLQuery(
                @"insert into mediagroup (groupname, description, grouppath, GroupType) "
                + @"select album, '', '\ALLARTISTS\' || UPPER(artist) || '\' || UPPER(album) || '\',1 "
                + @"from digitalmedialibrary "
                + @"group by UPPER(artist),UPPER(album) COLLATE NOCASE").ExecuteUpdate();
            
            OnProgressChanged(15, "Creating group items");
            //-- Artist\Album to songs
            dataSession.CreateSQLQuery(
                @"insert into mediagroupitem(GroupId, ItemType, ItemName, LibraryId) "
                + @"select mg.groupid, 5, dl.title, dl.LibraryId "
                + @"from digitalmedialibrary as dl "
                + @"join mediagroup as mg "
                + @"on (mg.groupPath = '\ALLARTISTS\' || dl.artist || '\' || dl.album|| '\')").ExecuteUpdate();

            OnProgressChanged(15, "Creating group items");
            //-- Album to songs
            dataSession.CreateSQLQuery(
                @"insert into mediagroupitem(GroupId, ItemType, ItemName, LibraryId) "
                + @"select mg.groupid, 5, dl.title, dl.LibraryId "
                + @"from digitalmedialibrary as dl "
                + @"join mediagroup as mg "
                + @"on (mg.groupPath = '\ALLALBUMS\' || dl.album|| '\')").ExecuteUpdate();
            
            OnProgressChanged(32, "Creating group items");
            
            //-- Artist to Albums
            dataSession.CreateSQLQuery(
                @"insert into mediagroupitem(GroupId, ItemType, ItemName, NextGroupId) "
                + @"select mg.groupid,4, dl.album, mg2.groupid "
                + @"from mediagroup as mg "
                + @"join DigitalMediaLibrary as dl "
                + @"on (mg.GroupPath = '\ALLARTISTS\' || dl.Artist || '\') "
                + @"join mediagroup as mg2 on (mg2.grouppath = '\ALLARTISTS\' || dl.Artist || '\' || dl.Album || '\') "
                + @"group BY UPPER(dl.artist), UPPER(dl.album) COLLATE NOCASE").ExecuteUpdate();
            
            OnProgressChanged(49, "Creating group items");
            //-- AllArtists to Artist
            dataSession.CreateSQLQuery(
                @"insert into mediagroupitem(GroupId, ItemType, ItemName, NextGroupId) "
                + @"select mg1.groupid,3,dl.artist, mg2.groupid as nextgroup "
                + @"from digitalmedialibrary as dl "
                + @"join mediagroup as mg1 on (mg1.grouppath = '\ALLARTISTS\') "
                + @"join mediagroup as mg2 on (mg2.groupPath = '\ALLARTISTS\' || dl.artist || '\') "
                + @"group by UPPER(dl.artist) COLLATE NOCASE").ExecuteUpdate();

            OnProgressChanged(49, "Creating group items");
            //-- AllAlbums to Album
            dataSession.CreateSQLQuery(
                @"insert into mediagroupitem(GroupId, ItemType, ItemName, NextGroupId) "
                + @"select mg1.groupid,3,dl.album, mg2.groupid as nextgroup "
                + @"from digitalmedialibrary as dl "
                + @"join mediagroup as mg1 on (mg1.grouppath = '\ALLALBUMS\') "
                + @"join mediagroup as mg2 on (mg2.groupPath = '\ALLALBUMS\' || dl.album || '\') "
                + @"group by UPPER(dl.album) COLLATE NOCASE").ExecuteUpdate();
            
            OnProgressChanged(83, "Creating group items");
            //-- AllSongs to songs
            dataSession.CreateSQLQuery(
                @"insert into mediagroupitem(GroupId, ItemType, ItemName, LibraryId) "
                + @"select mg.groupid, 5, dl.title, dl.LibraryId "
                + @"from digitalmedialibrary as dl "
                + @"join mediagroup as mg "
                + @"on (mg.groupPath = '\ALLSONGS\')").ExecuteUpdate();

            dataSession.Close();
            OnProgressChanged(100, "Finished");

            sw.Stop();
            OnFinished(sw.Elapsed);
        }
        public void OnFinished(TimeSpan pTotalTime)
        {
            if (CreationFinished != null)
            {
                CreationFinished(this, new FinishEventArgs(pTotalTime, 0));
            }
        }
        public void OnProgressChanged(int pPercent, string pStatus)
        {
            if(ProgressChanged != null)
            {
                ProgressChanged(pPercent, new ProgressEventArgs(pPercent, pStatus));
            }
        }
    }

    /// <summary>
    /// Depricated - for reference ONLY.
    /// Use MediaGroupCreater which is faster - executes SQL statements to generate records.
    /// </summary>
    //public class OldMediaGroupCreater
    //{
    //    private const string ALL_ARTISTS_GROUP = "AllArtists";
    //    private const string ALL_ALBUMS_GROUP = "AllAlbums";
    //    private const string ALL_SONGS_GROUP = "AllSongs";

    //    private const string PATH_SEPARATOR = "\\";

    //    private Hashtable mediaGroupCache;
        
    //    public OldMediaGroupCreater()
    //    {
    //        mediaGroupCache = new Hashtable();
    //    }

    //    public void AddMediaItem(DigitalMediaLibrary pItem)
    //    {
            
    //        MediaGroup allSongsGroup =  AddUpdateCreateGroup(CreateSongItem(pItem), ALL_SONGS_GROUP, FormatPath(ALL_SONGS_GROUP), false);

    //        MediaGroup albumGroup = AddUpdateCreateGroup(CreateSongItem(pItem), pItem.Album, FormatPath(pItem.Artist, pItem.Album), false);

    //        ApplicationMain.DbSession.Save(albumGroup);
    //        MediaGroup artistGroup = AddUpdateCreateGroup(CreateAlbumItem(pItem, albumGroup.GroupId), pItem.Artist, FormatPath(pItem.Artist), true);

    //        ApplicationMain.DbSession.Save(artistGroup);
    //        MediaGroup allArtistGroup = AddUpdateCreateGroup(CreateArtistItem(pItem, artistGroup.GroupId), ALL_ARTISTS_GROUP, FormatPath(ALL_ARTISTS_GROUP), true);
            
    //        MediaGroup allAlbumsGroup = AddUpdateCreateGroup(CreateAlbumItem(pItem, albumGroup.GroupId), ALL_ALBUMS_GROUP, FormatPath(ALL_ALBUMS_GROUP), true);

    //        ApplicationMain.DbSession.Save(allSongsGroup);
    //        ApplicationMain.DbSession.Save(allArtistGroup);
    //        ApplicationMain.DbSession.Save(allAlbumsGroup);

    //    }

    //    private MediaGroupItem CreateArtistItem(DigitalMediaLibrary pItem, int pNextGroupId)
    //    {
    //        MediaGroupItem item = new MediaGroupItem();
    //        item.ItemType = (int)MediaItemType.Artist;
    //        item.ItemName = pItem.Artist;
    //        item.NextGroupId = pNextGroupId;

    //        return item;
    //    }

    //    private MediaGroupItem CreateAlbumItem(DigitalMediaLibrary pItem, int pNextGroupId)
    //    {
    //        MediaGroupItem item = new MediaGroupItem();
    //        item.ItemType = (int)MediaItemType.Album;
    //        item.ItemName = pItem.Album;
    //        item.NextGroupId = pNextGroupId;

    //        return item;
    //    }

    //    private MediaGroupItem CreateSongItem(DigitalMediaLibrary pItem)
    //    {
    //        //foreach (MediaGroupItem eItem in pGroup.GroupItem)
    //        //{
    //        //    if (eItem.ItemName == pItem.Title)
    //        //    {
    //        //        throw new Exception("Duplicate Song found: " + pItem.Title);
    //        //    }
    //        //}

    //        MediaGroupItem item = new MediaGroupItem();
    //        item.ItemType = (int)MediaItemType.Song;
    //        item.ItemName = pItem.Title;
    //        item.LibraryId = pItem.LibraryId;

    //        return item;
    //    }

    //    public MediaGroup GetGroupFromDb(string pPath, string pName)
    //    {
    //        IList<MediaGroup> mediaGroup = ApplicationMain.DbSession.CreateCriteria(typeof(MediaGroup)).Add(Expression.Eq("GroupPath", pPath)).List<MediaGroup>();

    //        if (mediaGroup.Count <= 0)
    //        {
    //            MediaGroup newGroup = new MediaGroup();
    //            newGroup.GroupName = pName;
    //            newGroup.GroupPath = pPath;

    //            return newGroup;
    //        }
    //        else if (mediaGroup.Count > 1)
    //        {
    //            throw new Exception("Duplicate MediaGroup found, GroupPath needs to be unique");
    //        }
    //        else
    //        {
    //            return mediaGroup[0];
    //        }
    //    }

    //    private MediaGroup AddUpdateCreateGroup(MediaGroupItem pItem, string pName, string pPath, bool pItemNameIsUnique)
    //    {
    //        MediaGroup mediaGroup = null;
    //        if (mediaGroupCache.ContainsKey(pPath))
    //            mediaGroup = mediaGroupCache[pPath] as MediaGroup;
    //        else
    //        {
    //            mediaGroup = GetGroupFromDb(pPath, pName);
    //            mediaGroupCache.Add(pPath, mediaGroup);
    //        }

    //        bool found = false;

    //        if (pItemNameIsUnique)
    //        {
    //            foreach (MediaGroupItem eItem in mediaGroup.GroupItem)
    //            {
    //                if (eItem.ItemName == pItem.ItemName)
    //                {
    //                    found = true;
    //                    break;
    //                }
    //            }
    //        }

    //        if (!found)
    //        {
    //            pItem.Group = mediaGroup;
    //            mediaGroup.AddGroupItem(pItem);
    //        }

    //        return mediaGroup;
    //    }

    //    public string FormatPath(params string[] pPathElements)
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append(PATH_SEPARATOR);

    //        foreach (string str in pPathElements)
    //        {
    //            sb.Append(str).Append(PATH_SEPARATOR);
    //        }

    //        return sb.ToString();
    //    }
    //}
   
}
