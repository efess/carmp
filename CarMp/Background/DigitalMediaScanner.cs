using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CarMP.MediaInfo;
using CarMP.IO;
using CarMP.Background;
using CarMP.Callbacks;

namespace CarMP
{

    public class DigitalMediaScanner : ScannerBase
    {
        private Boolean _PreSearchDirectory = false;

        public Int32 MediaUpdateSize { get; set; }
        public MediaUpdate MediaOut { get; set; }
        public FileCheck FileChecker { get; set; }
        public Boolean FullScan { get; set; }

        List<MediaItem> mediaList = new List<MediaItem>();
            
        protected override void ScannedFile(FileInfo pFileInfo)
        {
            mediaList.Add(FileMediaInfo.GetInfo(pFileInfo));
            if (mediaList.Count >= MediaUpdateSize)
            {
                MediaOut(mediaList);
                mediaList.Clear();
            }
        }

        protected override void OnScanFinish()
        {
            // If any are left over
            if (MediaOut != null && mediaList.Count > 0)
                MediaOut(mediaList);

        }

        public override void StartScan()
        {

            base.StartScan();
        }
    }

}
