using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMp.ViewControls;
using System.Reflection;
using NHibernate.Criterion;
using NHibernate;

namespace CarMp
{

    public class MediaHistoryManager : Stack<MediaListItem>
    {
        private MediaListItemFactory _mediaListItemFactory;

        public MediaHistoryManager(MediaListItemFactory pMediaListItemFactory)
        {
            _mediaListItemFactory = pMediaListItemFactory;
        }

        /// <summary>
        /// From a song or group selection
        /// </summary>
        /// <param name="pMediaHistory"></param>
        public void AddHistoryItem(MediaHistoryItem pMediaListItem)
        {
            //while (Count >= pMediaListItem.Index)
            //{
            //    Pop();
            //}

            //Push(pMediaListItem.MediaListItem);

            using (ISession dataSession = Database.GetSession())
            using (ITransaction transaction = dataSession.BeginTransaction())
            {
                try
                {
                    foreach (MediaHistory historyItem in dataSession.CreateCriteria<MediaHistory>()
                        .Add(Expression.Ge("ListIndex", pMediaListItem.Index))
                        .List<MediaHistory>())
                    {
                        dataSession.Delete(historyItem);
                    }

                    MediaHistory newHistoryItem = new MediaHistory()
                    {
                        ObjectType = pMediaListItem.MediaListItem.GetType().Name,
                        MediaType = (int)pMediaListItem.MediaListItem.MediaType,
                        ListIndex = pMediaListItem.Index,
                        DisplayString = pMediaListItem.MediaListItem.DisplayString,
                        Key = pMediaListItem.MediaListItem.Key,
                        ItemSpecificType = pMediaListItem.MediaListItem.ItemSpecificType,
                    };

                    dataSession.Save(newHistoryItem);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        /// <summary>
        /// From a database record
        /// </summary>
        /// <param name="pMediaHistory"></param>
        public void AddHistoryItem(MediaHistory pMediaHistory)
        {
            MediaListItem item = _mediaListItemFactory.CreateListItem(
                pMediaHistory.ObjectType,
                pMediaHistory.Key,
                (MediaListItemType)pMediaHistory.MediaType,
                pMediaHistory.ItemSpecificType,
                pMediaHistory.DisplayString);


            if (item != null)
                this.Push(item);
        }

        public IEnumerable<MediaHistory> DbObjectEnumerator()
        {
            MediaListItem[] mediaListArray = this.ToArray();
            for(int i = 0; i < mediaListArray.Length; i++)
            {
                MediaListItem item = mediaListArray[i];

                MediaHistory history = new MediaHistory();
                history.ListIndex = i;
                //history.ItemSpecificType = 
                yield return history;
            }
        }
    }

    public class MediaHistoryItem
    {
        public int Index { get; set; }
        public MediaListItem MediaListItem { get; set; }
    }
}
