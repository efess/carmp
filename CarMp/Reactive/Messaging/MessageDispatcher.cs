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
            Subscribe(new IntermalMessageObserver(pObserver));
        }

        public override IDisposable Subscribe(IObserver<Message> observer)
        {
            throw new NotSupportedException("Use AddMessageObserver() to subscribe");
        }

        public void SendMessage(Message pMessage)
        {
            foreach (var observer in _observerList.OfType<IntermalMessageObserver>()
                .Where((m) => string.IsNullOrEmpty(pMessage.Recipient) || pMessage.Recipient == m.Name))
            {
                observer.OnNext(pMessage);
            }
        }

        /// <summary>
        /// Wrapper class for IMessageObservers
        /// </summary>
        private class IntermalMessageObserver : IObserver<Message>
        {
            private readonly IMessageObserver messageObserver;
            public string Name { get { return messageObserver.Name; } }

            public IntermalMessageObserver(IMessageObserver pMessageObserver)
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
