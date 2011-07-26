using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Background
{
    public class BackgroundTasks
    {
        private readonly List<IBackgroundOperation> _runningTasks = new List<IBackgroundOperation>();
        public IEnumerable<IBackgroundOperation> CurrentRunningTasks
        {
            get { return _runningTasks.AsEnumerable<IBackgroundOperation>(); }
        }

        public void ScanMedia(bool pFullScan)
        {
            new MediaGroupCreater().ReCreateArtistAlbumGroupCreation();
            var artScanner = new AlbumArtScanner();
            _runningTasks.Add(artScanner);
            artScanner.StartScan();

            return;

            if(pFullScan)
                AppMain.MediaManager.ClearMediaLibrary();
            var scanner = new DigitalMediaScanner();
            scanner.Path = AppMain.Settings.MusicPath;
            scanner.SupportedFormats.Add("MP3");
            scanner.MediaOut += AppMain.MediaManager.SaveMediaToLibrary;
            scanner.MediaUpdateSize = 100;
            scanner.FullScan = pFullScan;
            scanner.StartScan();
            scanner.ProgressChanged += (s, e) => DebugHandler.DebugPrint("Media Scan: " + e.Percent + " percent " + e.Status);
            scanner.Finish += (s, e) =>
                {
                    DebugHandler.DebugPrint("Finished with " + e.TotalCount + " total items " + e.TotalTime.TotalSeconds + " total seconds");
                    new MediaGroupCreater().ReCreateArtistAlbumGroupCreation();
                    //var artScanner = new AlbumArtScanner();
                    _runningTasks.Add(artScanner);
                    artScanner.StartScan();
                };
            _runningTasks.Add(scanner);

        }
    }
}
