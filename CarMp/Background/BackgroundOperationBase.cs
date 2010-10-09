using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Callbacks;

namespace CarMP.Background
{
    public abstract class BackgroundOperationBase : IBackgroundOperation
    {
        public event ProgressDelegate ProgressChanged;
        public event FinishHandler Finish;

        protected void OnFinished(TimeSpan pTotalTime)
        {
            OnFinished(pTotalTime, 0);
        }

        protected void OnFinished(TimeSpan pTotalTime, int pTotalItems)
        {
            if (Finish != null)
            {
                Finish(this, new FinishEventArgs(pTotalTime, pTotalItems));
            }
        }

        protected void OnProgressChanged(int pPercent, string pStatus)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(pPercent, new ProgressEventArgs(pPercent, pStatus));
            }
        }
    }
}
