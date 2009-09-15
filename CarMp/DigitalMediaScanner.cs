using System;
using System.Collections.Generic;

namespace CarMp
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Collections;

    /// <summary>
    /// Fake. To make it compile.
    /// </summary>
    public struct MEDIAINFO
    {

    }

    public delegate void MediaUpdate(List<MEDIAINFO> MInfo);
    public delegate bool FileCheck(String FileName, Int32 FileSize);
    public delegate void FinishCallBack();

    public class DigitalMediaScanner
    {
        private List<String> _SupportedFormats = new List<String>();
        private Boolean _PreSearchDirectory = false;
        private Boolean _Cancel = false;
        private Thread _ScanThread;

        public Int32 MediaUpdateSize { get; set; }
        public String Path { get; set; }
        public MediaUpdate MediaOut { get; set; }
        public FileCheck FileChecker { get; set; }
        public FinishCallBack Finish { get; set; }
        public Boolean FullScan { get; set; }

        public List<String> SupportedFormats
        {
            get { return _SupportedFormats; }
        }

        public DigitalMediaScanner()
        {
            _ScanThread = new Thread(new ThreadStart(Run));
        }

        private void Run()
        {
            try
            {
                String[] dr = Environment.GetLogicalDrives();

                DriveInfo drinfo = new DriveInfo(dr[1]);
                if (MediaOut == null)
                    return;

                // do it.

                int _countAdded = 0;
                int _totalcount = 0;
                int _count = 0;
                List<MEDIAINFO> _mList = new List<MEDIAINFO>();

                foreach (String _directory in GetDirectories(Path))
                {
                    foreach (String _file in Directory.GetFiles(_directory))
                    {
                        if (_Cancel)
                            return;

                        FileInfo fFile = new FileInfo(_file);

                        if (FileChecker != null && !FullScan)
                            if (FileChecker(fFile.FullName, (int)fFile.Length))
                            {
                                _totalcount++;
                                Debug.WriteLine("Total: " + _totalcount);
                                continue;
                            }

                        if (FormatSupported(fFile.Extension.ToUpper()))
                        {
                            //_mList.Add(GetInfo(fFile));

                            _count++;
                            _totalcount++;
                            _countAdded++;

                            Debug.WriteLine("Total files: " + _totalcount);
                            Debug.WriteLine("Total Added: " + _countAdded);

                            if (_count >= MediaUpdateSize)
                            {
                                if (MediaOut == null)
                                    return;

                                MediaOut(_mList);

                                // Reset
                                _count = 0;
                                _mList.Clear();
                            }
                        }
                    }
                }
                
                // If any are left over
                if (MediaOut != null && _mList.Count > 0)
                    MediaOut(_mList);
            }
            finally
            {
                if(Finish != null)
                    Finish();
            }
        }

        public void StartScan()
        {
            _Cancel = false;
            _ScanThread.Start();
        }

        public void StopScan()
        {
            _Cancel = true;

            // Allow time for thread to close.
            Thread.Sleep(1000);

            // Force close
            _ScanThread.Abort();
        }

        // This is autovue specific, need to make our own.
        //private MEDIAINFO GetInfo(FileInfo pFile)
        //{ 
        //    MEDIAINFO _MEDIAI = new MEDIAINFO();

        //    _MEDIAI.Location = pFile.FullName;
        //    _MEDIAI.FileSize = (UInt64)pFile.Length;

        //    switch (pFile.Extension.Replace(".","").ToUpper())
        //    {
        //        case "MP3":

        //            // Look for Tags
        //            Hashtable newHash = MediaTag.GetMP3TagInfo(pFile.FullName);
                   
        //            // If none, try parsing filename
        //            if(newHash.Count == 0)
        //                newHash = MediaTag.GetInfoFromMusicFilename(pFile.FullName);
                    
        //            foreach (DictionaryEntry frame in newHash)
        //            {
        //                switch ((String)frame.Key)
        //                {
        //                    case "TPE1":
        //                        _MEDIAI.Artist = frame.Value.ToString();
        //                        _MEDIAI.AlbumArtist = frame.Value.ToString();
        //                        break;
        //                    case "TIT2":
        //                        _MEDIAI.Title = frame.Value.ToString();
        //                        break;
        //                    //case "TYeR":
        //                    //    _MEDIAI.y = frame.Value as String;
        //                    //    break;
        //                    case "TCON":
        //                        _MEDIAI.Genre = frame.Value.ToString();
        //                        break;
        //                    case "TRCK":
        //                        _MEDIAI.TrackNo = frame.Value.ToString();
        //                        break;
        //                    case "TALB":
        //                        _MEDIAI.Album = frame.Value.ToString();
        //                        break;
        //                    case "Album":
        //                        _MEDIAI.Album = frame.Value.ToString();
        //                        break;
        //                    case "Artist":
        //                        _MEDIAI.Artist = frame.Value.ToString();
        //                        _MEDIAI.AlbumArtist = frame.Value.ToString();
        //                        break;
        //                    case "Genre":
        //                        _MEDIAI.Genre = frame.Value.ToString();
        //                        break;
        //                    case "Track":
        //                        _MEDIAI.TrackNo = frame.Value.ToString();
        //                        break;
        //                    case "Title":
        //                        _MEDIAI.Title = frame.Value.ToString();
        //                        break;
        //                }
        //            }
                    

        //            return _MEDIAI;
        //        case "OGG":
        //            break;
        //    }
        //    return _MEDIAI;
        //}

        private ArrayList GetDirectories(String pPath)
        {
            // Could use recursive, but we want a local variable

            ArrayList _dirList = new ArrayList();

            Stack<String> _dirStack = new Stack<String>();
            _dirStack.Push(pPath);

            while (_dirStack.Count > 0)
            {
                String _dir = _dirStack.Pop();

                if (_PreSearchDirectory)
                {
                    // experimental, check to make sure this directory contains a file to scan
                    foreach (String _file in Directory.GetFiles(_dir))
                    {
                        if (FormatSupported(new FileInfo(_file).Extension))
                        {
                            _dirList.Add(_dir);
                            break;
                        }
                    }
                }
                else
                    _dirList.Add(_dir);

                foreach (String _subDir in Directory.GetDirectories(_dir))
                {
                    _dirStack.Push(_subDir);
                }
            }
            return _dirList;
        }

        private Boolean FormatSupported(String pFormat)
        {
            pFormat = pFormat.Replace(".", "");

            return _SupportedFormats.Exists(delegate(String str) { return str == pFormat; });
        }
    }
    
}
