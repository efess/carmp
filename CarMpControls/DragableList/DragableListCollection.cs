using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CarMpControls
{
    /// <summary>
    /// Wher _px is not specified, item count will be implied
    /// </summary>
    public class DragableListCollection
    {

        // Private properties
        private List<DragableListItem> m_list = new List<DragableListItem>();
        private const int BUFFER_SIZE = 100;

        /// <summary>
        /// Center of buffer
        /// </summary>
        private int currentBufferCenter = 0;

        private int m_bufferLow;
        private int m_bufferHigh;
        
        private int m_bufferSize;

        // Position of list in view
        private int m_listLocPx;

        internal int ListLocPx
        {
            get
            {
                return m_listLocPx;
            }
            set
            {
                m_listLocPx = value;
            }
        }

        public DragableListCollection()
        {
            m_bufferSize = BUFFER_SIZE;
        }

        internal bool BufferNeedsUpdate(int pNewLoc)
        {
#if !USE_DIRECT2D
            if (currentBufferCenter == 0)
                return true;
            if(pNewLoc > currentBufferCenter + (m_bufferSize / 4)
                || pNewLoc < currentBufferCenter - (m_bufferSize / 4))
                return true;
#endif
            return false;
        }

        // Accessors
        public DragableListItem this[int pIndex]
        {
            get
            {
                return m_list[pIndex];
            }
        }

        public DragableListItem SelectedItem
        {
            get
            {
                return m_list.Find(delegate(DragableListItem pItem) { return pItem.Selected == true; });
            }
            set
            {
                if (value == null)
                    return;

                SelectSingleItem(value);
            }
        }

        /// <summary>
        /// Selects a single item
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return m_list.FindIndex(delegate(DragableListItem pItem) { return pItem.Selected == true; });
            }
            set
            {
                DragableListItem listItem = (value > -1 && value < m_list.Count) ? m_list[value] : null;

                if (listItem == null)
                    return;

                SelectSingleItem(listItem);
            }
        }

        private void SelectSingleItem(DragableListItem pItem)
        {
            int itemIndex = m_list.IndexOf(pItem);

            if(itemIndex < 0)
                return;

            pItem.Selected = true;

            for (int i = 0; i < m_list.Count; i++)
            {
                if (i == itemIndex)
                    continue;

                m_list[i].Selected = false;
            }
        }

        public int BufferLoc
        {
            set
            {
                if(BufferNeedsUpdate(value))
                {
                    this.m_bufferLow = value - (m_bufferSize / 2);
                    this.m_bufferHigh = value + (m_bufferSize / 2);
                    
                    this.UpdateBuffer();
                    currentBufferCenter = value;
                }
            }
        }

        public int BufferSize
        {
            get { return m_bufferSize; }
            set { m_bufferSize = value; }
        }

        public int Count
        {
            get
            {
                return this.m_list.Count;
            }
        }

        private void UpdateBuffer()
        {
            foreach (DragableListItem dl in this.m_list)
            {
                if (dl.Index < this.m_bufferLow || dl.Index > this.m_bufferHigh)
                {
                    dl.Buffered = false;
                }
                else
                {
                    dl.Buffered = true;
                }
            }
        }

        public void Add(DragableListItem pItem)
        {
            if (m_list.Count > this.m_bufferLow && m_list.Count < this.m_bufferHigh)
                pItem.Buffered = true;

            this.m_list.Add(pItem);
        }

        public void Clear()
        {
            foreach (DragableListItem eItem in m_list)
            {
                eItem.Dispose();
            }

            m_list.Clear();
        }
    }
}
