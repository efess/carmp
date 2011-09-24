using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Reactive.Messaging
{
    public class MessageDispatcher : DefaultObservable<Message>
    {
        public void AddMessageObserver(IMessageObserver pObserver)
        {
            if (pObserver == null)
                throw new ArgumentNullException("pObserver");

            pObserver.DisposeUnsubscriber =
                base.Subscribe(new InternalMessageObserver(pObserver));
        }

        public override IDisposable Subscribe(IObserver<Message> observer)
        {
            throw new NotSupportedException("Use AddMessageObserver() to subscribe");
        }

        public void SendMessage(Message pMessage)
        {
            if (pMessage == null) return;

            new Action<Message>((m) => InternalSendMessage(m)).BeginInvoke(pMessage, null, null);
        }

        private void InternalSendMessage(Message pMessage)
        {
            DebugHandler.DebugPrint("Sending Message " + pMessage.Type.ToString() + " with data " + pMessage.Data ?? "null");
            lock (_observerList)
                foreach (var observer in _observerList.OfType<InternalMessageObserver>()
                    .Where((m) => pMessage.Recipient == null
                        || pMessage.Recipient.Count() <= 0
                        || pMessage.Recipient.Contains(m.Name, StringComparer.InvariantCultureIgnoreCase)).ToArray())
                {
                    observer.OnNext(pMessage);
                }
        }

        /// <summary>
        /// Wrapper class for IMessageObservers
        /// </summary>
        private class InternalMessageObserver : IObserver<Message>
        {
            private readonly IMessageObserver messageObserver;
            public string Name { get { return messageObserver.Name; } }

            public InternalMessageObserver(IMessageObserver pMessageObserver)
            {
                messageObserver = pMessageObserver;
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception exception)
            {
                DebugHandler.DebugPrint("Error sending to " + messageObserver.Name + " Exception: " + exception.ToString());
            }

            public void OnNext(Message pMessage)
            {
                messageObserver.ProcessMessage(pMessage);
            }
        }
    }
}
