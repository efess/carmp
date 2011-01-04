using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Reactive
{
    public abstract class DefaultObservable<T> : IObservable<T>
    {
        public DefaultObservable()
        {
            _observerList = new List<IObserver<T>>();
        }

        protected List<IObserver<T>> _observerList;

        protected virtual void PushToObservers(T pData)
        {
            IObserver<T>[] copyArray = new IObserver<T>[_observerList.Count];

            _observerList.CopyTo(copyArray);
            for (int i = 0;
                i < copyArray.Length;
                i++)
                copyArray[i].OnNext(pData);
        }

        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_observerList)
                if (!_observerList.Contains(observer))
                    _observerList.Add(observer);

            return new UnSubscriber<T>(
                _observerList,
                _observerList[_observerList.IndexOf(observer)]
                );
        }
    }
}
