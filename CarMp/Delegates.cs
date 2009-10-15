using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp
{
    public delegate void MediaUpdate(List<MediaItem> MInfo);
    public delegate bool FileCheck(String FileName, Int32 FileSize);
    public delegate void FinishHandler(object sender, FinishEventArgs pFinishEventArgs);
    public delegate void ProgressDelegate(object sender, ProgressEventArgs pEventArgs);
    public delegate void WinampStateUpdate(object sender, WinampStateUpdateArgs pWinampStateEventArgs);
    
}
