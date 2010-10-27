using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Reactive.Messaging
{
    public interface IMessageObserver
    {
        string Name { get; }
        void ProcessMessage(Message pMessage);
        IDisposable DisposeUnsubscriber { get; set; }
    }
}
