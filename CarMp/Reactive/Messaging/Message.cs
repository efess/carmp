using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Reactive.Messaging
{
    public class Message
    {
        public IEnumerable<string> Recipient { get; private set; }
        public MessageType Type { get; private set; }
        public object Data { get; private set; }

        public Message(IEnumerable<string> pRecipient, MessageType pType, object pData)
        {
            Recipient = pRecipient;
            Type = pType;
            Data = pData;
        }
    }
}
