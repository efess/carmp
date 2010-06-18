using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMp.ViewControls;
using System.Reflection;

namespace CarMp
{

    public class MediaListHistory : Stack<MediaListItem>
    {
        public MediaListHistory(List<MediaHistory> pMediaHistories)
        {

        }

        /// <summary>
        /// From a song or group selection
        /// </summary>
        /// <param name="pMediaHistory"></param>
        public void AddHistoryItem(MediaListItem pMediaListItem)
        {
            this.Push(pMediaListItem);
        }

        /// <summary>
        /// From a database record
        /// </summary>
        /// <param name="pMediaHistory"></param>
        public void AddHistoryItem(MediaHistory pMediaHistory)
        {
            MediaListItem item = Activator.CreateInstance(Type.GetType(pMediaHistory.ObjectType),
                pMediaHistory.DisplayString,
                pMediaHistory.MediaType,
                pMediaHistory.Key,
                pMediaHistory.ItemSpecificType)
                as MediaListItem;

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
                history.Index = i;
                history.ItemSpecificType = 
                yield return 
            }
        }
    }
}
