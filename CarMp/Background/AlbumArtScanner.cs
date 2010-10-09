using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.IO;
using CarMP.DataObjects.View;
using CarMP.DataObjects;
using NHibernate.Criterion;
using System.IO;

namespace CarMP.Background
{
    public class AlbumArtScanner  : ScannerBase
    {
        protected override void  Scan()
        {
            NHibernate.ISession session = DatabaseInterface.DataSession;
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
                    }
                }
            }

            session.Flush();

            // OR Maybe an even better idea - search through media library for songs, search for album art
            // based on their path? Easily identifiy unique paths der.
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
            var art = new Art();
            art.ArtType = pType;
            art.Key = pArtist + "|" + pAlbum;
            art.Path = pPath;
            pDataSession.Save(art);
        }

        

        //public void GetWMPData()
        //{
        //    stuff.Clear();
        //    WMP = new WindowsMediaPlayerClass();
        //    IWMPMediaCollection mc = WMP.mediaCollection;
        //    IWMPStringCollection strings = mc.getAttributeStringCollection("AlbumID", "audio");

        //    int WmpCount = strings.count;
        //    if (WmpCount > 0)
        //    {
        //        for (int i = 0; i < WmpCount; i++)
        //        {
        //            string item = strings.Item(i);
        //            IWMPPlaylist playlist = mc.getByAttribute("AlbumID", item);
        //            playlist.setItemInfo("SortAttribute", "OriginalIndex");
        //            int playlistCount = playlist.count;
        //            if (playlistCount > 0)
        //            {
        //                for (int ic = 0; ic < playlistCount; ic++)
        //                {
        //                    IWMPMedia mMedia = playlist.get_Item(ic);
        //                    string guid = mMedia.getItemInfo("WM/WMCollectionID");
        //                    string albumtitle = mMedia.getItemInfo("WM/AlbumTitle");
        //                    //Add to array if not found
        //                    if (guid != "")
        //                    {
        //                        Output("Found WMP album art for: " + albumtitle);
        //                        bool found = false;
        //                        foreach (ArtItem ai in stuff)
        //                        {
        //                            if (ai.album.ToLower() == albumtitle.ToLower())
        //                            {
        //                                found = true;
        //                                break;
        //                            }
        //                        }
        //                        if (found == false)
        //                        {
        //                            stuff.Add(new ArtItem(albumtitle, guid));
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //    }
        //}

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
