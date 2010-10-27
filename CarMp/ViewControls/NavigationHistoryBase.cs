using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Reactive.Messaging;
using CarMP.ViewControls.Interfaces;

namespace CarMP.ViewControls
{
    public abstract class NavigationHistoryBase : ViewControlCommonBase, INavigationHistory, IMessageObserver
    {
        protected abstract void ClearHistory();
        protected abstract void Push(string pDisplayString, int pIndexReference);
        
        public event Action<int> HistoryClick;
        public Func<IEnumerable<KeyValuePair<int, string>>> GetHistorySource { get; set;}

        public void ProcessMessage(Message pMessage)
        {
            if (pMessage.Type == MessageType.MediaHistoryChange)
            {
                UpdateHistory();
            }
        }

        private void UpdateHistory()
        {
            ClearHistory();
            foreach (var kvPair in GetHistorySource()
                .OrderBy<KeyValuePair<int, string>,int>((kv) => kv.Key))
            {
                Push(kvPair.Value, kvPair.Key);
            }
        }

        public IDisposable DisposeUnsubscriber { get; set; }
    }
}
