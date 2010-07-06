using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Reactive
{
    public class UnSubscriber<T> : IDisposable
    {
        private List<IObserver<T>> _observerCollection;
        private IObserver<T> _observer;

        public UnSubscriber(List<IObserver<T>> pObservers, IObserver<T> pObserver)
        {
            _observer = pObserver;
            _observerCollection = pObservers;
        }

        public void Dispose()
        {
            lock (_observerCollection)
            {
                if (_observer != null && _observerCollection.Contains(_observer))
                    _observerCollection.Remove(_observer);
            }
        }
    }
}
