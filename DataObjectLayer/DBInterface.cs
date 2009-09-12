using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//using id3Reader;
using System.Threading;
using System.Collections;
//using WMPLib;


namespace dbNUcar
{
    public class DBInterface
    {
    //    private DateTime ProcessDuration;
    //    public static string SkinDirectory;
    //    public static DBInterface Db = null;
    //    public delegate void UpdateStatusHandler(string status);
    //    public event UpdateStatusHandler StatusUpdated;  // StatusUpdated is the event name
    //    public int SongIndex;
    //    private ArrayList directories = new ArrayList();
    //    private string MediaDir;
    //    public WindowsMediaPlayer WMP;
    //    public ArrayList stuff = new ArrayList();
        

    //    public void Output(string message)
    //    {
    //        if (StatusUpdated != null)
    //        {
    //            StatusUpdated(message);
    //        }
    //    }


    //    public void CreateDatabase(string dirstart)
    //    {
            
    //        directories.Clear();
    //        Librarys ls = new Librarys();
    //        ls.Truncate();
    //        Albums abs = new Albums();
    //        abs.Truncate();
    //        Artists arts = new Artists();
    //        arts.Truncate();
    //        SongIndex = 0;
    //        MediaDir = dirstart;
    //        Thread td = new Thread(new ThreadStart(CreateNewDatabaseThread));
    //        td.Start();
    //    }

    //    public void GetWMPData()
    //    {
    //        stuff.Clear();
    //        WMP = new WindowsMediaPlayerClass();
    //        IWMPMediaCollection mc = WMP.mediaCollection;
    //        IWMPStringCollection strings = mc.getAttributeStringCollection("AlbumID", "audio");
            
    //        int WmpCount = strings.count;
    //        if (WmpCount > 0)
    //        {
    //            for (int i = 0; i < WmpCount; i++)
    //            {
    //                string item = strings.Item(i);
    //                IWMPPlaylist playlist = mc.getByAttribute("AlbumID", item);
    //                playlist.setItemInfo("SortAttribute","OriginalIndex");
    //                int playlistCount = playlist.count;
    //                if(playlistCount > 0)
    //                {
    //                    for(int ic = 0; ic < playlistCount; ic++)
    //                    {
    //                        IWMPMedia mMedia = playlist.get_Item(ic);
    //                        string guid = mMedia.getItemInfo("WM/WMCollectionID");
    //                        string albumtitle = mMedia.getItemInfo("WM/AlbumTitle");
    //                        //Add to array if not found
    //                        if(guid != "")
    //                        {
    //                            Output("Found WMP album art for: " + albumtitle);
    //                            bool found = false;
    //                            foreach (ArtItem ai in stuff)
    //                            {
    //                                if (ai.album.ToLower() == albumtitle.ToLower())
    //                                {
    //                                    found = true;
    //                                    break;
    //                                }
    //                            }
    //                            if (found == false)
    //                            {
    //                                stuff.Add(new ArtItem(albumtitle, guid));
    //                            }
    //                        }

    //                    }
    //                }
    //            }
    //        }
    //    }

    //    public string GetKeyValue(string pkey)
    //    {
    //        Options opt = new Options();
    //        opt.key = pkey;
    //        opt.Load();
    //        if (opt.Count == 1)
    //        {
    //            return ((Option)opt[0]).keyvalue;
    //        }
    //        return "";
    //    }


    //    public int GetKeyIntValue(string pkey,int pdefault)
    //    {

    //        Options opt = new Options();
    //        opt.key = pkey;
    //        opt.Load();
    //        int temp;
    //        if (opt.Count == 1)
    //        {
    //            if (int.TryParse(((Option)opt[0]).keyvalue, out temp))
    //            {
    //                return temp;
    //            }
    //            else
    //            {
    //                return pdefault;
    //            }
    //        }
    //        return pdefault;
    //    }

    //    public void SetKeyValue(string pkey, int pkeyvalue)
    //    {
    //        SetKeyValue(pkey, pkeyvalue.ToString());
    //    }

    //    public void SetKeyValue(string pkey, string pkeyvalue)
    //    {
    //        Options opt = new Options();
    //        opt.key = pkey;
    //        opt.Load();
    //        if (opt.Count == 1)
    //        {
    //            ((Option)opt[0]).keyvalue = pkeyvalue;
    //            opt.Save();
    //        }
    //        else
    //        {
    //            Option option = new Option();
    //            option.key = pkey;
    //            option.keyvalue = pkeyvalue;
    //            option.New();
    //            option.Save();
    //        }
    //    }

    //    public void AddMp3ToDatabase(string file)
    //    {
    //        ProcessDuration = DateTime.Now;
    //        SongIndex++;
    //        Id3Read id3;// = new Id3Read();
    //        Output(SongIndex + "  -  " + file);

    //        string relativepath = file.Substring(MediaDir.Length,file.LastIndexOf("\\")-MediaDir.Length);
    //        relativepath = (relativepath == "") ? "\\" : relativepath;

    //        id3 = new Id3Read(file);
            
    //        #region Build Library Record
    //        Library l = new Library();
    //        l.New();
    //        l.index = SongIndex;
    //        l.filename = file.Substring(file.LastIndexOf("\\")+1, file.Length - file.LastIndexOf("\\")-1);
    //        if (id3.id3v1.exists || id3.id3v2.exists)
    //        {
    //            if (id3.id3v2.Album == "" || id3.id3v2.Album == null)
    //            {
    //                if (id3.id3v1.Album == "" || id3.id3v1.Album == null)
    //                {
    //                    l.album = "";
    //                }
    //                else
    //                {
    //                    l.album = id3.id3v1.Album;
    //                }
    //            }
    //            else
    //            {
    //                l.album = id3.id3v2.Album;

    //            }
    //            if (id3.id3v2.Artist == "" || id3.id3v2.Artist == null)
    //            {
    //                if (id3.id3v1.Artist == "" || id3.id3v1.Artist == null)
    //                {
    //                    l.artist = "";
    //                }
    //                else
    //                {
    //                    l.artist = id3.id3v1.Artist;
    //                }
    //            }
    //            else
    //            {
    //                l.artist = id3.id3v2.Artist;
    //            }

    //            //track                
    //            if (id3.id3v2.TrackNum == null || id3.id3v2.TrackNum == "")
    //            {
    //                if (id3.id3v1.Track != null)
    //                {
    //                    l.track = id3.id3v1.Track;
    //                }
    //            }
    //            else
    //            {
    //                l.track = id3.id3v2.TrackNum;
    //            }


    //            //If there is no ID3 tag for Title, just insert a space
    //            if (id3.id3v2.Title == "" || id3.id3v2.Title == null)
    //            {
    //                if (id3.id3v1.Title == "" || id3.id3v1.Title == null)
    //                {
    //                    l.title = "";
    //                }
    //                else
    //                {
    //                    l.title = id3.id3v1.Title;
    //                }
    //            }
    //            else
    //            {
    //                l.title = id3.id3v2.Title;
    //            }
    //            if (id3.id3v2.Genre == "" || id3.id3v2.Genre == null)
    //            {
    //                if (id3.id3v1.Genre == "" || id3.id3v1.Genre == null)
    //                {
    //                    l.genre = "";
    //                }
    //                else
    //                {
    //                    l.genre = id3.id3v1.Genre;
    //                }
    //            }
    //            else
    //            {
    //                l.genre = id3.id3v2.Genre;
    //            }
    //        }

    //        //Attempt to parse Title/Artist/Track from filename
    //        if(l.title == "" && l.artist == "")
    //        {
    //            int result;
    //            try
    //            {
    //                if (Int32.TryParse(l.filename.Substring(0, 1), out result))
    //                {
    //                    if (l.filename.IndexOf(" - ") > 0)
    //                    {
    //                        l.track = (Int32.TryParse(l.filename.Substring(0, l.filename.IndexOf("-")), out result)) ? l.filename.Substring(0, l.filename.IndexOf("-")) : "";
    //                        if (l.filename.IndexOf(" - ") == l.filename.LastIndexOf(" - "))
    //                        {
    //                            l.title = l.filename.Substring(l.filename.IndexOf(" - ") + 2, l.filename.Length - l.filename.IndexOf(" - ") - 2);
    //                        }
    //                        else
    //                        {
    //                            string[] temp;
    //                            temp = l.filename .Substring(0,l.filename.Length-4).Split(new char[] { '-' });
    //                            l.track = temp[0];
    //                            l.artist = temp[1].Trim();
    //                            l.title = temp[2].Trim();
    //                        }
    //                    }
    //                    else
    //                        l.title = l.filename.Substring(0, l.filename.Length - 4);
    //                }
    //                else
    //                {
    //                    if (l.filename.IndexOf(" - ") > 0)
    //                    {
    //                        l.artist = l.filename.Substring(0, l.filename.IndexOf(" - "));
    //                        l.title = l.filename.Substring(l.filename.LastIndexOf(" - ") + 3, l.filename.Length - l.filename.LastIndexOf(" - ") - 7);
    //                    }
    //                    else
    //                        if (l.filename.IndexOf("-") > 0)
    //                        {
    //                            l.artist = l.filename.Substring(0, l.filename.IndexOf("-"));
    //                            l.title = l.filename.Substring(l.filename.LastIndexOf("-") + 1, l.filename.Length - l.filename.LastIndexOf("-") - 5);
    //                        }
    //                        else
    //                            l.title = l.filename.Substring(0, l.filename.Length - 4);

    //                }
    //            }
    //            catch(Exception e)
    //            {
    //                throw(e);
    //            }
    //        }
    //        l.fullpath = file;
    //        l.root = MediaDir;
    //        l.filetype = "MP3";
    //        l.relativepath = relativepath;
    //        //l.relativepath = file;
    //        l.kbps = id3.mp3Header.Bitrate;//TestUltraID3.FirstMPEGFrameInfo.Bitrate;
    //        TimeSpan duration = DateTime.Now.Subtract(ProcessDuration);
    //        l.length = Convert.ToString((duration.Seconds * 1000) + duration.Milliseconds);//new DateTime(1990, 1, 1, TestUltraID3.FirstMPEGFrameInfo.Duration.Hours, TestUltraID3.FirstMPEGFrameInfo.Duration.Minutes, TestUltraID3.FirstMPEGFrameInfo.Duration.Seconds); ;
    //        l.monostereo = id3.mp3Header.Mode;//TestUltraID3.FirstMPEGFrameInfo.Mode.ToString();
    //        l.frequency = id3.mp3Header.Frequency;
    //        l.Save();
    //        #endregion

    //        #region Build Album Record
    //        Albums abs = new Albums();
    //        abs.AlbumName = id3.id3v2.Album;
    //        abs.Load();
    //        if (abs.Count == 0 && abs.AlbumName != null)
    //        {
    //            ArrayList jpglist = new ArrayList();

    //            Album ab = new Album();
    //            ab.New();

    //            string tempalbum = l.album;
    //            string path = l.root + l.relativepath;
    //            string guid;
    //            string searchstring;
    //            DirectoryInfo di = new DirectoryInfo(path);
    //            FileInfo[] files = di.GetFiles();
    //            foreach (FileInfo finfo in files)
    //            {
    //                if (finfo.Name.Length > 4)
    //                {
    //                    if (finfo.Name.Substring((finfo.Name.Length - 4), 4) == ".jpg")
    //                    {
    //                        jpglist.Add(finfo.Name);
    //                    }
    //                }
    //            }
    //            //Get Album art
    //            bool foundlarge = false;
    //            bool foundsmall = false;
    //            foreach(ArtItem ai in stuff)
    //            {
    //                if (ai.album == tempalbum)
    //                {
    //                    guid = ai.guid;
    //                    stuff.Remove(ai);
                        
    //                    foreach (string jpg in jpglist)
    //                    {
    //                        if(jpg == "AlbumArt_" + guid + "_Small.jpg")
    //                        {
    //                                foundsmall = true;  
    //                                ab.picturesmallpath = path + "\\"+jpg;
    //                        }
    //                        if(jpg == "AlbumArt_" + guid + "_Large.jpg")
    //                        {
    //                                foundlarge = true;  //Solid find, do not search further.
    //                                ab.picturepath = path + "\\" + jpg;
    //                        }
    //                        if(jpg == "Folder.jpg" && !foundlarge)
    //                        {
    //                            ab.picturepath = path + "\\" + jpg;
    //                        }

    //                        if(jpg == "AlbumArtSmall.jpg" && !foundsmall)
    //                        {
    //                            ab.picturesmallpath = path + "\\" + jpg;
    //                        }
    //                        if(jpg == "Front.jpg" && !foundlarge)
    //                        {
    //                            ab.picturepath = path + "\\" + jpg;
    //                        }
    //                        if (foundlarge && foundsmall)
    //                        {
    //                            break;
    //                        }
    //                    }


    //                    break;
    //                }
    //            }

    //            if (!foundlarge)
    //            {

    //                foreach (string jpg in jpglist)
    //                {
    //                    if (jpg.IndexOf(tempalbum, StringComparison.CurrentCultureIgnoreCase) > 0)
    //                    {
    //                        ab.picturepath = path + "\\" + jpg;
    //                    }
    //                    else
    //                    {
    //                        if (jpg.IndexOf(tempalbum.Replace(" ", "_"), StringComparison.CurrentCultureIgnoreCase) > 0)
    //                        {
    //                            ab.picturepath = path + "\\" + jpg;
    //                        }
    //                    }
    //                }
    //            }

                
    //            ab.index = SongIndex;
    //            ab.AlbumName = id3.id3v2.Album;
    //            ab.AlbumArtist = id3.id3v2.Artist;
    //            ab.Save();

    //        }
    //        #endregion

    //        #region Build Artist Record
    //        Artists arts = new Artists();
    //        arts.ArtistName = id3.id3v2.Artist;
    //        arts.Load();
    //        if (arts.Count == 0 && arts.ArtistName != null)
    //        {
    //            Artist art = new Artist();
    //            art.New();
    //            art.index = SongIndex;
    //            art.ArtistName= id3.id3v2.Artist;
    //            art.Save();

    //        }
    //                    #endregion

    //        /*
    //        #region Build Path Record
    //        Paths paths = new Paths();
    //        paths.Root = MediaDir;
    //        paths.Relativepath = relativepath;
    //        paths.Load();
    //        if (paths.Count == 0)
    //        {
    //            Path path = new Path();
    //            path.New();

    //            string[] folders = relativepath.Split(new char[] { '\\' });

    //            path.Root = MediaDir;
    //            path.Relativepath = relativepath;
    //            path.TopFolder = folders[folders.Length - 1];
    //            if (folders.Length > 1)
    //            {
    //                path.BottomFolder = folders[folders.Length - 2];
    //            }
    //            path.Save();
    //        }
    //        #endregion*/
    //        //  0----+----1----+----2----+----3----+----4----+----5----+----6----+-
    //        //  \Albums\A Perfect Circle\a perfect circle - thirteenth step (2003)
    //    }

    //    private void CreateNewDatabaseThread()
    //    {
    //        DateTime test = DateTime.Now;
    //        //GetWMPData();
    //        TraverseDirectories(MediaDir);
    //        foreach (string directory in directories)
    //        {
    //            ProcessDirectory(directory);
    //        }
    //        SetKeyValue("SongCount", SongIndex.ToString());
    //        TimeSpan duration = DateTime.Now.Subtract(test);
    //        int lkasdj = (duration.Minutes * 60) + duration.Seconds;
    //        Output("DONE: " + SongIndex.ToString() + " songs in " + lkasdj + " seconds");
    //    }



    //    public void TraverseDirectories()
    //    {
    //        TraverseDirectories(MediaDir);
    //    }

    //    public void TraverseDirectories(string dirstart)
    //    {
    //        try
    //        {
                
    //            string[] dirs = Directory.GetDirectories(dirstart);

    //            //Recurse through the subdirectories
    //            foreach (string dir in dirs)
    //            {
    //                TraverseDirectories(dir);
    //            }

    //            Output("Adding directory: \n" + dirstart);
    //            directories.Add(dirstart);
                
    //        }
    //        catch (Exception e)
    //        {
    //        }
    //    }

    //    private void ProcessDirectory(string directory)
    //    {
    //        ProcessDirectory(directory, false);
    //    }

    //    public void ProcessDirectory(string directory, bool skipDups)
    //    {

    //        // Create a reference to the current directory.
    //        DirectoryInfo di = new DirectoryInfo(directory);
    //        // Create an array representing the files in the current directory.
    //        FileInfo[] fi = di.GetFiles();

    //        // Print out the names of the files in the current directory.
    //        foreach (FileInfo fiTemp in fi)
    //        {
    //            //Only return files with the mp3 extension
    //            if (fiTemp.Name.Length > 4)
    //            {
    //                if (fiTemp.Name.Substring((fiTemp.Name.Length - 4), 4) == ".mp3")
    //                {
    //                    if (!skipDups)
    //                    {
    //                        AddMp3ToDatabase(fiTemp.FullName);
    //                    }
    //                    else
    //                    {
    //                        if (songExists(fiTemp.FullName))
    //                        {
    //                            AddMp3ToDatabase(fiTemp.FullName);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    private bool songExists(string fullpath)
    //    {
    //        Librarys lib = new Librarys();
    //        lib.fullpath = fullpath;
    //        lib.Load();
    //        return (lib.Count == 0 ? false : true);
    //    }


    
    //}

    //class ArtItem
    //{
    //    public ArtItem(string Album, string Guid)
    //    {
    //        album = Album;
    //        guid = Guid;
    //    }

    //    public string album;
    //    public string guid;
    }
}
