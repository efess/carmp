using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP
{
    public enum DragableListSwitchDirection
    {
        Forward,
        Back
    }

    public enum MediaState
    {
        Stopped,
        Playing,
        Paused
    }

    public enum MediaSort
    {
        Track,
        FileName,
        Title
    }

    public enum ViewControlFunction
    {
        None,
        MediaProgressBar,
        MediaProgressText,
        MediaStop,
        MediaPlay,
        MediaPause,
        MediaNext,
        MediaPrevious,
        MediaArt,
        MediaDisplayName,
        VolumeUp,
        VolumeDown,
        ToggleState

    }
}



