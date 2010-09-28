using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP
{
    public class EventQueue
    {
        private Queue<EventQueueEntry> _queue;
        public EventQueue()
        {
            _queue = new Queue<EventQueueEntry>();
        }
        public void AddToQueue(EventQueueEntry pEventQueueEntry)
        {
            _queue.Enqueue(pEventQueueEntry);
        }
        public void ProcessQueue()
        {
            while (_queue.Count > 0)
            {
                EventQueueEntry eventQueueEntry = _queue.Dequeue();
                eventQueueEntry.Execute();
            }
        }
    }
    public delegate void EventQueueDelegate<T>(T pEventArgs);
    public class EventQueueEntry
    {
        

        private Delegate _eventHandler;
        private EventArgs _eventArgs;

        public EventQueueEntry(Delegate pDelegate, EventArgs pEventArgs)
        {
            _eventHandler = pDelegate;
            _eventArgs = pEventArgs;
        }
        public void Execute()
        {
            _eventHandler.DynamicInvoke(_eventArgs);
        }
    }
}
