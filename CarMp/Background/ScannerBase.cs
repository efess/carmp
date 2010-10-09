using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CarMP.Callbacks;
using System.Diagnostics;
using System.IO;

namespace CarMP.Background
{
    public abstract class ScannerBase : BackgroundOperationBase
    {
        public ScannerBase()
        {
            SupportedFormats = new List<String>();
            _ScanThread = new Thread(new ThreadStart(Scan));
        }

        public Func<FileInfo, bool> FileChecker;
        public String Path { get; set; }
        public List<String> SupportedFormats { get; private set;}

        protected Boolean _Cancel = false;
        protected Thread _ScanThread;

        protected virtual void ScannedFile(FileInfo pFileInfo) { }

        protected virtual void Scan()
        {
            Stopwatch scanTimer = new Stopwatch();
            int _addedCount = 0;
            int _totalAddedCount = 0;
            int _currentPercent = 0;
            int _totalFiles = 0;
            
            scanTimer.Start();

            // do it.

            List<MediaItem> _mList = new List<MediaItem>();

            var crawler = new FileCrawler(Path, SupportedFormats.ToArray());
            crawler.Load();

            _totalFiles = crawler.Count;

            foreach (FileInfo file in crawler)
            {
                if (_Cancel)
                    return;

                if ((int)(((double)_totalAddedCount / (double)_totalFiles) * 100) > _currentPercent)
                {
                    _currentPercent = (int)(((double)_totalAddedCount / (double)_totalFiles) * 100);
                }

                OnProgressChanged(_currentPercent, "Adding file " + file.Name);

                if (FileChecker != null)
                    if (FileChecker(file))
                    {
                        _totalAddedCount++;
                        Debug.WriteLine("Total: " + _totalAddedCount);
                        continue;
                    }

                ScannedFile(file);
                        
                _totalAddedCount++;
                _addedCount++;
            }
                
            scanTimer.Stop();
            OnScanFinish();
            OnFinished(scanTimer.Elapsed, _totalAddedCount);
        }

        protected virtual void OnScanFinish()
        {
        }

        public virtual void StartScan()
        {
            _Cancel = false;
            _ScanThread.Start();
        }

        public virtual void StopScan()
        {
            _Cancel = true;

            // Allow time for thread to close.
            Thread.Sleep(1000);

            // Force close
            _ScanThread.Abort();
        }
    }
}
