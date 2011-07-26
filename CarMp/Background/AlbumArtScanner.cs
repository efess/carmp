using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.IO;
using CarMP.DataObjects.View;
using CarMP.DataObjects;
using NHibernate.Criterion;
using System.IO;
using Microsoft.MediaPlayer.Interop;

namespace CarMP.Background
{
    public class AlbumArtScanner  : ScannerBase
    {
        private string WMP_LARGE_GUID = "AlbumArt_{0}_Large.jpg";
        private string WMP_SMALL_GUID = "AlbumArt_{0}_Small.jpg";

        protected override void  Scan()
        {
            
            NHibernate.ISession session = DatabaseInterface.DataSession;
            try
            {
                // Try to pull out links for Guid art
                SaveWMPLibraryData(session);
            }
            catch { }// Ignore this.

            IList<AlbumArtPath> artPaths = session.
                CreateCriteria(typeof(AlbumArtPath))
                .AddOrder(new Order("Album", true))
                .List<AlbumArtPath>();

            foreach(AlbumArtPath path in artPaths)
            {
                foreach(FileInfo file in FileSystem.GetFiles(path.Path, new List<string>{"JPG", "JPEG"}))
                {
                    switch (file.Name.Substring(0,file.Name.Length - file.Extension.Length).ToUpper())
                    {
                        case "FOLDER":
                            CreateAndSaveLargeArt(path.Artist, path.Album, file.FullName, session);
                            break;
                        case "ALBUMARTSMALL":
                            CreateAndSaveLargeArt(path.Artist, path.Album, file.FullName, session);
                            break;
                        case "COVER":
                            CreateAndSaveLargeArt(path.Artist, path.Album, file.FullName, session);
                            break;
                    }
                }
            }

            session.Flush();

        }
        private void CreateAndSaveLargeArt(string pArtist, string pAlbum, string pPath, NHibernate.ISession pDataSession)
        {
            CreateAndSaveArt(pArtist, pAlbum, pPath, pDataSession, ArtType.AlbumArtLarge);
        }


        private void CreateAndSaveSmallArt(string pArtist, string pAlbum, string pPath, NHibernate.ISession pDataSession)
        {
            CreateAndSaveArt(pArtist, pAlbum, pPath, pDataSession, ArtType.AlbumArtSmall);
        }
        private void CreateAndSaveArt(string pArtist, string pAlbum, string pPath, NHibernate.ISession pDataSession, ArtType pType)
        {
            var key = pArtist + "|" + pAlbum;
            
            var art = new Art();
            art.ArtType = (int)pType;
            art.Key = key;
            art.Path = pPath;
            try
            {
                pDataSession.SaveOrUpdate(art);
            }
            catch { } // ignore.
        }



        private void SaveWMPLibraryData(NHibernate.ISession pDataSession)
        {
            // Hip hip hooray for procedural......
            WindowsMediaPlayerClass WMP = new WindowsMediaPlayerClass();
            IWMPMediaCollection mc = WMP.mediaCollection;
            IWMPStringCollection strings = mc.getAttributeStringCollection("AlbumID", "audio");

            var albumArtistLookup = new System.Collections.Hashtable();

            int WmpCount = strings.count;
            if (WmpCount > 0)
            {
                for (int i = 0; i < WmpCount; i++)
                {
                    string item = strings.Item(i);
                    IWMPPlaylist playlist = mc.getByAttribute("AlbumID", item);
                    playlist.setItemInfo("SortAttribute", "OriginalIndex");
                    int playlistCount = playlist.count;
                    if (playlistCount > 0)
                    {
                        for (int ic = 0; ic < playlistCount; ic++)
                        {
                            IWMPMedia mMedia = playlist.get_Item(ic);

                            string guid = mMedia.getItemInfo("WM/WMCollectionID");
                            string albumTitle = mMedia.getItemInfo("WM/AlbumTitle");
                            string albumArtist = mMedia.getItemInfo("WM/AlbumArtist");

                            FileInfo file = new FileInfo(mMedia.sourceURL);

                            //Add to array if not found
                            if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(albumTitle)
                                && !albumArtistLookup.ContainsKey(file.DirectoryName))
                            {
                                albumArtistLookup[file.DirectoryName] = true;

                                string filePathLarge = System.IO.Path.Combine(file.DirectoryName, string.Format(WMP_LARGE_GUID, guid));
                                string filePathSmall = System.IO.Path.Combine(file.DirectoryName, string.Format(WMP_SMALL_GUID, guid));

                                if (File.Exists(filePathLarge))
                                    CreateAndSaveLargeArt(albumArtist, albumTitle, filePathLarge, pDataSession);
                                if (File.Exists(filePathSmall))
                                    CreateAndSaveSmallArt(albumArtist, albumTitle, filePathSmall, pDataSession);
                            }
                        }
                    } 
                }
            }
        }

        //Get Album art
                //bool foundlarge = false;
                //bool foundsmall = false;
                //foreach(ArtItem ai in stuff)
                //{
                //    if (ai.album == tempalbum)
                //    {
                //        guid = ai.guid;
                //        stuff.Remove(ai);
                        
                //        foreach (string jpg in jpglist)
                //        {
                //            if(jpg == "AlbumArt_" + guid + "_Small.jpg")
                //            {
                //                    foundsmall = true;  
                //                    ab.picturesmallpath = path + "\\"+jpg;
                //            }
                //            if(jpg == "AlbumArt_" + guid + "_Large.jpg")
                //            {
                //                    foundlarge = true;  //Solid find, do not search further.
                //                    ab.picturepath = path + "\\" + jpg;
                //            }
                //            if(jpg == "Folder.jpg" && !foundlarge)
                //            {
                //                ab.picturepath = path + "\\" + jpg;
                //            }

                //            if(jpg == "AlbumArtSmall.jpg" && !foundsmall)
                //            {
                //                ab.picturesmallpath = path + "\\" + jpg;
                //            }
                //            if(jpg == "Front.jpg" && !foundlarge)
                //            {
                //                ab.picturepath = path + "\\" + jpg;
                //            }
                //            if (foundlarge && foundsmall)
                //            {
                //                break;
                //            }
                //        }


                //        break;
                //    }
                //}

                //if (!foundlarge)
                //{

                //    foreach (string jpg in jpglist)
                //    {
                //        if (jpg.IndexOf(tempalbum, StringComparison.CurrentCultureIgnoreCase) > 0)
                //        {
                //            ab.picturepath = path + "\\" + jpg;
                //        }
                //        else
                //        {
                //            if (jpg.IndexOf(tempalbum.Replace(" ", "_"), StringComparison.CurrentCultureIgnoreCase) > 0)
                //            {
                //                ab.picturepath = path + "\\" + jpg;
                //            }
                //        }
                //    }
                //}

                

        protected override void ScannedFile(System.IO.FileInfo pFileInfo)
        {
            throw new NotImplementedException();
        }
    }
}
