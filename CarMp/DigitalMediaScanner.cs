using System;
using System.Collections.Generic;
using CarMpMediaInfo;

namespace CarMp
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Collections;

    public class DigitalMediaScanner
    {
        public event ProgressDelegate ScanProgressChanged; 
        private List<String> _SupportedFormats = new List<String>();
        private Boolean _PreSearchDirectory = false;
        private Boolean _Cancel = false;
        private Thread _ScanThread;

        public Int32 MediaUpdateSize { get; set; }
        public String Path { get; set; }
        public MediaUpdate MediaOut { get; set; }
        public FileCheck FileChecker { get; set; }
        public Boolean FullScan { get; set; }

        public event FinishHandler FinishScaning;

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
            if (MediaOut == null)
                throw new Exception("Nothing listening on Media Output delegate");

            Stopwatch scanTimer = new Stopwatch();
            int _addedCount = 0;
            int _totalAddedCount = 0;
            int _currentPercent = 0;
            int _totalFiles = 0;
            
            int _count = 0;

            try
            {
                scanTimer.Start();
                String[] dr = Environment.GetLogicalDrives();

                DriveInfo drinfo = new DriveInfo(dr[1]);
                if (MediaOut == null)
                    return;

                // do it.

                List<MediaItem> _mList = new List<MediaItem>();
                List<FileInfo> _fileList = new List<FileInfo>();
                foreach (String _directory in GetDirectories(Path))
                {
                    if (_Cancel)
                        return;
                    foreach (String _file in Directory.GetFiles(_directory))
                    {
                        if (_Cancel)
                            return;

                        FileInfo fFile = new FileInfo(_file);
                        if (FormatSupported(fFile.Extension.ToUpper()))
                        {
                            _fileList.Add(fFile);
                        }
                    }
                }

                _totalFiles = _fileList.Count;

                foreach (FileInfo file in _fileList)
                {
                    if (_Cancel)
                        return;


                    if ((int)(((double)_totalAddedCount / (double)_totalFiles) * 100) > _currentPercent)
                    {
                        _currentPercent = (int)(((double)_totalAddedCount / (double)_totalFiles) * 100);
                    }

                    OnProgressChange(_currentPercent, "Adding file " + file.Name);

                    if (FileChecker != null && !FullScan)
                        if (FileChecker(file.FullName, (int)file.Length))
                        {
                            _totalAddedCount++;
                            Debug.WriteLine("Total: " + _totalAddedCount);
                            continue;
                        }

                    if (FormatSupported(file.Extension.ToUpper()))
                    {
                        _mList.Add(GetInfo(file));

                        _count++;
                        _totalAddedCount++;
                        _addedCount++;

                        Debug.WriteLine("Total files: " + _totalAddedCount);
                        Debug.WriteLine("Total Added: " + _addedCount);

                        if (_count >= MediaUpdateSize)
                        {
                            OnProgressChange(_currentPercent, "Saving to database...");

                            MediaOut(_mList);

                            // Reset
                            _count = 0;
                            _mList.Clear();
                        }
                    }
                    
                }
                
                // If any are left over
                if (MediaOut != null && _mList.Count > 0)
                    MediaOut(_mList);

            }
            finally
            {
                scanTimer.Stop();

                if (FinishScaning != null)
                    FinishScaning(this, new FinishEventArgs(scanTimer.Elapsed, _totalAddedCount));
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

        
        private MediaItem GetInfo(FileInfo pFile)
        {
            Id3Read reader = new Id3Read(pFile.FullName);
            MediaItem item = new MediaItem();
            if (string.IsNullOrEmpty(reader.Title) && string.IsNullOrEmpty(reader.Artist))
            {
                FilenameInfo fileParser = new FilenameInfo();
                fileParser.Parse(pFile.Name);

                item.Track = fileParser.Track;
                item.Artist = fileParser.Artist;
                item.Title = fileParser.Title;
            }
            else
            {
                item.Album = reader.Album;
                item.Artist = reader.Artist;
                item.Title = reader.Title;
            }
            item.FileName = pFile.Name;
            item.Path = pFile.FullName;
            item.Track = reader.Track;
            item.Genre = reader.Genre;
            item.Kbps = reader.BitRate;

            // Clean up item records
            if (string.IsNullOrEmpty(item.Album)) item.Album = "NoAlbum";
            if (string.IsNullOrEmpty(item.Artist)) item.Artist = "NoArtist";
            if (string.IsNullOrEmpty(item.Title)) item.Title = "Untitled";

            //item.Album = reader.A
            return item;
            //reader.read();
        }//

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

        private void OnProgressChange(int pPercent, string pStatus)
        {
            if (ScanProgressChanged != null)
            {
                ScanProgressChanged(this, new ProgressEventArgs(pPercent, pStatus));
            }
        }
    }

    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int pPercent, string pStatus)
        {
            Percent = pPercent;
            Status = pStatus;
        }
        public int Percent { get; private set; }
        public string Status { get; private set;}
    }

    public class FinishEventArgs : EventArgs
    {
        public FinishEventArgs(TimeSpan pTotalTime, int pTotalCount)
        {
            TotalCount = pTotalCount;
            TotalTime = pTotalTime;
        }

        public int TotalCount { get; private set; }
        public TimeSpan TotalTime { get; private set; }
    }
}
