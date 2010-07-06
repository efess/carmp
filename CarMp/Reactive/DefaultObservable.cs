using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Reactive
{
    public abstract class DefaultObservable<T> : IObservable<T>
    {
        public DefaultObservable()
        {
            _observerList = new List<IObserver<T>>();
        }

        protected List<IObserver<T>> _observerList;

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_observerList)
            {
                if (!_observerList.Contains(observer))
                    _observerList.Add(observer);
            }

            return new UnSubscriber<T>(
                _observerList,
                _observerList[_observerList.IndexOf(observer)]
                );
        }
    }
}
