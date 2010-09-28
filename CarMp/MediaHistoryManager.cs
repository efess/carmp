using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.ViewControls;
using System.Reflection;
using NHibernate.Criterion;
using NHibernate;
using System.Threading;
using CarMP.MediaEntities;

namespace CarMP
{

    public class MediaHistoryManager : IEnumerable<MediaHistory>
    {
        private readonly Stack<MediaHistory> _mediaHistoryStack;
        private MediaListItemFactory _mediaListItemFactory;
        
        private object _lockObject = new object();

        public MediaHistoryManager(MediaListItemFactory pMediaListItemFactory)
        {
            _mediaHistoryStack = new Stack<MediaHistory>();
            _mediaListItemFactory = pMediaListItemFactory;
        }

        /// <summary>
        /// From a song or group selection
        /// </summary>
        /// <param name="pMediaHistory"></param>
        public void AddHistoryItem(MediaListItem pMediaListItem, int pListIndex)
        {
            lock (_lockObject)
            {
                while (_mediaHistoryStack.Count > pListIndex)
                {
                    _mediaHistoryStack.Pop();
                }

                MediaHistory newHistoryItem = new MediaHistory()
                {
                    ObjectType = pMediaListItem.GetType().Name,
                    MediaType = (int)pMediaListItem.MediaType,
                    ListIndex = pListIndex,
                    DisplayString = pMediaListItem.DisplayString,
                    Key = pMediaListItem.Key,
                    ItemSpecificType = pMediaListItem.ItemSpecificType,
                };

                _mediaHistoryStack.Push(newHistoryItem);

                using (ISession dataSession = Database.GetSession())
                using (ITransaction transaction = dataSession.BeginTransaction())
                {
                    try
                    {
                        foreach (MediaHistory historyItem in dataSession.CreateCriteria<MediaHistory>()
                            .Add(Expression.Ge("ListIndex", pListIndex))
                            .List<MediaHistory>())
                        {
                            dataSession.Delete(historyItem);
                        }

                        dataSession.Save(newHistoryItem);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        /// <summary>
        /// From a database record
        /// </summary>
        /// <param name="pMediaHistory"></param>
        public void AddHistoryItem(MediaHistory pMediaHistory)
        {
            if (pMediaHistory != null)
                _mediaHistoryStack.Push(pMediaHistory);
        }

        #region IEnumerable<MediaHistory> Members

        public IEnumerator<MediaHistory> GetEnumerator()
        {
            return _mediaHistoryStack.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _mediaHistoryStack.GetEnumerator();
        }

        #endregion
    }
}
