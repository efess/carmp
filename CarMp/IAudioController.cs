using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp
{
    public interface IAudioController
    {
        void PlayFile(string pFile);
        void AddSongToPlayList(string pFile);
        void StartPlayback();
        void StopPlayback();
        void PausePlayback();
        void SetCurrentPos(int Position);
        int GetCurrentPos();
        int GetSongLength();
    }
}
