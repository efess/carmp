using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Callbacks
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int pPercent, string pStatus)
        {
            Percent = pPercent;
            Status = pStatus;
        }
        public int Percent { get; private set; }
        public string Status { get; private set; }
    }
}
