using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Callbacks
{
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
