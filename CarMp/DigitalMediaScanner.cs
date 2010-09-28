using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.IO;
using CarMP.MediaInfo;
using CarMP.IO;

namespace CarMP
{

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

                if (MediaOut == null)
                    return;

                // do it.

                List<MediaItem> _mList = new List<MediaItem>();
                List<FileInfo> _fileList = new List<FileInfo>();

                // Load files
                foreach (String _directory in FileSystem.GetAllDirectories(Path))
                {
                    if (_Cancel)
                        return;

                    FileSystem.AppendFiles(_directory, _SupportedFormats, _fileList);
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
                        _mList.Add(FileMediaInfo.GetInfo(file));

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
