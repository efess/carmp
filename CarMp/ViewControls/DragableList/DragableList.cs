using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using CarMP.Reactive.Touch;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;
using System.Runtime.Remoting.Messaging;


namespace CarMP.ViewControls
{
    public class DragableList : ViewControlCommonBase, ISkinable
    {
        public delegate void ListChangedEventHandler(object sender, ListChangeEventArgs e);
        public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs e);

        private delegate void ListChangeDelegate(int SwitchDirectionSign, int NewIndex);
        private delegate void VelocityStart(int InitialVelocity);

        public event ListChangedEventHandler BeforeListChanged;
        public event ListChangedEventHandler AfterListChanged;
        public event SelectedItemChangedEventHandler SelectedItemChanged;

        // List vars
        private int m_lastYDirection;
        
        // Height of each list item
        private float m_listItemSize = 25;

        // Horizontal shift during list changes
        private float m_listHShift_px;

        /// <summary>
        /// Current List scrollable height in pixles (ListCount - ViewAble) * ItemSize
        /// </summary>
        private float m_currentListSize_px;
        /// <summary>
        /// Next List scrollable height in pixles (ListCount - ViewAble) * ItemSize
        /// </summary>
        private float m_nextListSize_px;

        /// <summary>
        /// Number of items that can be visible
        /// </summary>
        private int m_listDisplayCount;

        /// <summary>
        /// Current displayed list
        /// </summary>
        private DragableListCollection m_listCurrentDisplay = new DragableListCollection(); // SHould base class these items
        private float m_currentListLoc_px;
        
        /// <summary>
        /// Next list shown only during transition - reference is assigned to m_listContents immediately after
        /// </summary>
        private DragableListCollection m_listNextDisplay = new DragableListCollection();
        private float m_nextListLoc_px;

        /// <summary>
        /// Collection of lists that can be navigated by this control
        /// </summary>
        private List<DragableListCollection> m_listCollection = new List<DragableListCollection>();
        private int m_listCollectionIndex;

        private int m_mouseVelocityStartThreshold = 2;
        
        //touch vars
        private TouchMove _touchPreviousPoint;

        // Velocity Delegate
        private Action<int,int> m_velocityDelegate;
        private bool m_velocityStop;

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);
            this.OnSizeChanged(null, null);
        }

        // Constructors
        public DragableList()
        {
            m_listCollection.Add(m_listCurrentDisplay);
        }

        // Accessors

        private DragableListCollection CurrentList
        {
            get
            {
                return m_listCurrentDisplay;
            }
            set
            {
                m_listCurrentDisplay = value;
                m_currentListLoc_px = value.ListLocPx;
                if (m_listCurrentDisplay.Count >= m_listDisplayCount)
                {
                    m_currentListSize_px = (m_listCurrentDisplay.Count * m_listItemSize) - this.Height;
                }
                else
                {
                    m_currentListSize_px = 0;
                }
            }
        }


        private DragableListCollection NextList
        {
            get
            {
                return m_listCurrentDisplay;
            }
            set
            {
                m_listNextDisplay = value;
                m_nextListLoc_px = value.ListLocPx;
                if (m_listNextDisplay.Count >= m_listDisplayCount)
                {
                    m_nextListSize_px = (m_listNextDisplay.Count * m_listItemSize) - Convert.ToInt32(this.Height); 
                }
                else
                {
                    m_nextListSize_px = 0;
                }
            }
        }

        /// <summary>
        /// Vertical pixel offset of display list (0 - item.Height)
        /// </summary>
        private float CurrentListLocVertOffset_px
        {
            get
            {
                return m_currentListLoc_px % m_listItemSize;
            }
        }

        /// <summary>
        /// Vertical pixel offset of display list (0 - item.Height)
        /// </summary>
        private float NextListLocVertOffset_px
        {
            get
            {
                return m_nextListLoc_px % m_listItemSize;
            }
        }

        /// <summary>
        /// Returns Pixel location of top of display list
        /// </summary>
        private float ListLoc_px
        {
            set
            {
                if (value < 0)
                {
                    m_currentListLoc_px = 0;
                    CurrentList.ListLocPx = 0;
                }
                else if(value > this.m_currentListSize_px)
                {
                    m_currentListLoc_px = m_currentListSize_px;
                    CurrentList.ListLocPx = m_currentListSize_px;
                }
                else
                {
                    m_currentListLoc_px = value;
                    CurrentList.ListLocPx = value;
                }
            }
            get
            {
                return m_currentListLoc_px;
            }
        }

        /// <summary>
        /// Returns current pixel size of whole list
        /// </summary>
        private float ListSize_px
        {
            get
            {
                return this.m_listCurrentDisplay.Count * this.m_listItemSize;
            }
        }

        /// <summary>
        /// Returns current item count of the list
        /// </summary>
        private int ListCount
        {
            get
            {
                return m_listCurrentDisplay.Count;
            }
        }

        /// <summary>
        /// Returns list index at top of viewable portion
        /// </summary>
        private int CurrentListItemViewIndexZero
        {
            get
            {
                if (this.m_listCurrentDisplay.Count < m_listDisplayCount)
                    return 0;
                else
                    return Convert.ToInt32(Math.Floor(m_currentListLoc_px / m_listItemSize));//(m_currentListLoc_px * (this.m_listCurrentDisplay.Count - CurrentListViewableItemCount)) / m_currentListSize_px;
            }
        }

        /// <summary>
        /// Returns list index at top of viewable portion
        /// </summary>
        private int NextListItemViewIndexZero
        {
            get
            {
                if (this.m_listNextDisplay.Count < m_listDisplayCount)
                    return 0;
                else
                    return Convert.ToInt32(Math.Floor((m_nextListLoc_px * (this.m_listNextDisplay.Count - NextListViewableItemCount)) / m_nextListSize_px));
            }
        }

        private int NextListViewableItemCount
        {
            get
            {
                return m_listNextDisplay.Count < m_listDisplayCount ? m_listNextDisplay.Count : m_listDisplayCount;
            }
        }

        private int CurrentListViewableItemCount
        {
            get
            {
                return m_listCurrentDisplay.Count < m_listDisplayCount ? m_listCurrentDisplay.Count : m_listDisplayCount;
            }
        }

        /// <summary>
        /// Returns true if the CurrentList count is more than the viewable count
        /// </summary>
        private bool Scrollable
        {
            get
            {
                return CurrentList.Count > m_listDisplayCount;
            }
        }

        /// <summary>
        /// Returns the item index for pixel location
        /// </summary>
        /// <param name="pPixel"></param>
        /// <returns></returns>
        private int GetItemAtPx(float pPixel)
        {
            if (this.m_listCurrentDisplay.Count == 0)
                return 0;
            else
                return Convert.ToInt32(Math.Floor((pPixel * (this.m_listCurrentDisplay.Count)) / (m_listItemSize * CurrentList.Count)));
        }

        /// <summary>
        /// Index of this list
        /// </summary>
        public int CurrentListIndex
        {
            get
            {
                return m_listCollection.IndexOf(m_listCurrentDisplay);
            }
        }

        /// <summary>
        /// Number of lists
        /// </summary>
        public int ListCollectionCount
        {
            get
            {
                return m_listCollection.Count;
            }
        }

        // Public Methods
        public void InsertNextIntoCurrent(DragableListItem[] pItems)
        {
            foreach (DragableListItem item in pItems)
            {
                InsertNextIntoCurrent(item);
            }
        }

        /// <summary>
        /// Inserts a display item in the next location in the current collection
        /// </summary>
        /// <param name="item"></param>
        public void InsertNextIntoCurrent(DragableListItem item)
        {
            item.Bounds = new RectF(0, 0, this.Width, m_listItemSize);

            item.Index = this.m_listCurrentDisplay.Count;

            this.m_listCurrentDisplay.Add(item);
            if(m_listCurrentDisplay.Count > m_listDisplayCount)
                this.m_currentListSize_px += this.m_listItemSize;
        }

        /// <summary>
        /// Inserts a display item in the next location in the specified list index
        /// </summary>
        /// <param name="item"></param>
        public void InsertNextIntoListIndex(DragableListItem item, int pListIndex)
        {
            if (pListIndex < 0)
            {
                throw new Exception("ListIndex is out of rage n- Less than zero");
            }
            if (this.m_listCollection.Count == pListIndex &&
                this.m_listCollection[pListIndex - 1] != null)
            {
                this.m_listCollection.Add(new DragableListCollection());
            }
            else if (m_listCollection.Count <= pListIndex)
            {
                throw new Exception("ListIndex is out of range - greater than next insertion");
            }

            item.Bounds = new RectF(0, 0, this.Width, m_listItemSize);
            item.Index = this.m_listCurrentDisplay.Count;

            this.m_listCollection[pListIndex].Add(item);

            if (this.m_listCollection[pListIndex] == m_listCurrentDisplay
                && m_listCurrentDisplay.Count > m_listDisplayCount)
                this.m_currentListSize_px += this.m_listItemSize;
        }

        public void ClearAndFillNextList(DragableListItem[] pList)
        {
            int listIndex = CurrentListIndex;
            int newListIndex = listIndex + 1;

            if (newListIndex < ListCollectionCount)
            {
                ClearListAtIndex(newListIndex, true);
            }
            for (int i = 0; i < pList.Length; i++)
            {
                InsertNextIntoListIndex(pList[i], newListIndex);
            }
        }

        /// <summary>
        /// Clears all dragable list items in list at specified listindex
        /// 
        /// </summary>
        /// <param name="pListIndex"></param>
        public void ClearListAtIndex(int pListIndex, bool pRemoveFutureLists)
        {
            D("Clearing at index " + pListIndex);

            if (pListIndex >= m_listCollection.Count || pListIndex < 0)
            {
                throw new Exception("ListIndex is out of rage");
            }

            // Call dispose on each object
            for (int i = m_listCollection.Count - 1; i >= pListIndex; i--)
            {
                if (pRemoveFutureLists || i == pListIndex)
                {
                    DragableListCollection listCollection = m_listCollection[i];
                    listCollection.ListLocPx = 0;

                    for (int j = 0; j < listCollection.Count; j++)
                    {
                        listCollection[j].Dispose();
                    }

                    listCollection.Clear();

                    if (i != pListIndex)
                        m_listCollection.Remove(listCollection);
                }
            }
        }

        // Private Methods

        private void D(string Message)
        {
            Debug.Print(DateTime.Now.ToString("hh:mm:ss") + " \t> " + Message);
        }

        private void SelectItem(int pMouseX, int pMouseY)
        {
            this.m_listCurrentDisplay.SelectedIndex = GetItemAtPx(m_currentListLoc_px + pMouseY);

            if (this.m_listCurrentDisplay.SelectedIndex > -1)
            {
                // Execute Event
                if (SelectedItemChanged != null)
                {
                    SelectedItemChanged.BeginInvoke(this, 
                        new SelectedItemChangedEventArgs(
                            this.m_listCurrentDisplay.SelectedItem, 
                            this.m_listCurrentDisplay.SelectedIndex),
                        null, 
                        null);
                }
            }
        }

        private void ShiftList(int pDelta)
        {
            if (!Scrollable)
                return;
            this.ListLoc_px += pDelta;
            this.m_listCurrentDisplay.BufferLoc = this.CurrentListItemViewIndexZero;
        }

        /// <summary>
        /// Moves list forward as long as there is a list available
        /// </summary>
        public void ChangeListForward()
        {
            if (CurrentListIndex < m_listCollection.Count)
            {
                ChangeList(CurrentListIndex + 1);
            }
        }

        /// <summary>
        /// Moves list back as long as there is a list available
        /// </summary>
        public void ChangeListBack()
        {
            if (CurrentListIndex > 0)
            {
                ChangeList(CurrentListIndex - 1);
            }
        }

        public void ChangeList(int pNewIndex)
        {
            DragableListSwitchDirection direction = Math.Sign(pNewIndex - CurrentListIndex) == -1
                ? DragableListSwitchDirection.Back
                : DragableListSwitchDirection.Forward;

            // Return if at the beginning or end of list
            if (pNewIndex < 0
                || pNewIndex >= m_listCollection.Count)
                return;

            if (BeforeListChanged != null)
            {
                BeforeListChanged(this, new ListChangeEventArgs(direction, pNewIndex));
            }

            ExecuteChangeList(direction, pNewIndex);

            if (AfterListChanged != null)
            {
                AfterListChanged(this, new ListChangeEventArgs(direction, pNewIndex));
            }
        }

        /// <summary>
        /// Moves in the provided direction as long as there is a list available
        /// </summary>
        private void ChangeList(DragableListSwitchDirection pDirection)
        {
        }

        private void ChangeListHorizontalDistance(int pDistance)
        {
            ChangeList(Math.Sign(pDistance) + CurrentListIndex);           
        }

        private System.Windows.Forms.MouseEventArgs FixMouseEventArgs(
            System.Windows.Forms.MouseEventArgs pEventArgs)
        {
            return new System.Windows.Forms.MouseEventArgs(pEventArgs.Button,
                pEventArgs.Clicks,
                pEventArgs.X - Convert.ToInt32(X),
                pEventArgs.Y - Convert.ToInt32(Y),
                pEventArgs.Delta);
        }


        private void DirtyView()
        {
        }
        // Overrided Events

        protected override void PreRender()
        {
            this.Clear();
            
            for (int i = 0; i < this.m_listDisplayCount; i++)
            {
                if (this.CurrentListItemViewIndexZero + i < m_listCurrentDisplay.Count)
                {
                    float yShift = (m_listItemSize * i) - CurrentListLocVertOffset_px;
                    RectF currentRect = new RectF(
                        m_listHShift_px, 
                        yShift,
                        this.Width + m_listHShift_px,
                        m_listItemSize + yShift);

                    D2DViewControl control = this.m_listCurrentDisplay[this.CurrentListItemViewIndexZero + i];
                    control.StartRender();
                    control.Bounds = currentRect;
                    this.AddViewControl(control);
                    
                    //this.m_listCurrentDisplay[this.CurrentListItemViewIndexZero + i].DrawItem(pRenderTarget, currentRect);
                }

                if (m_listHShift_px != 0 && i < m_listNextDisplay.Count)
                {
                    float yShift = ((m_listItemSize * i) - NextListLocVertOffset_px);
                    float xShift = m_listHShift_px - (this.Width * Math.Sign(m_listHShift_px));

                    RectF nextRect = new RectF(
                        xShift,
                        yShift,
                        this.Width + xShift,
                        m_listItemSize + yShift);

                    D2DViewControl control = this.m_listNextDisplay[this.NextListItemViewIndexZero + i];
                    control.StartRender();
                    control.Bounds = nextRect;
                    this.AddViewControl(control);
                }
            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);
        }

        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            switch (pTouchGesture.Gesture)
            {
                case Reactive.Touch.GestureType.Click:
                    SelectItem(Convert.ToInt32(pTouchGesture.Location.X), Convert.ToInt32(pTouchGesture.Location.Y));
                    return;
                case Reactive.Touch.GestureType.SwipeLeft:
                    ChangeListForward();
                    return;
                case Reactive.Touch.GestureType.SwipeRight:
                    ChangeListBack();
                    return;
            }
        }

        protected override void OnTouchMove(Reactive.Touch.TouchMove pTouchMove)
        {
            if (_touchPreviousPoint != null)
            {
                int sign = Math.Sign(_touchPreviousPoint.Y - pTouchMove.Location.Y);
                m_lastYDirection = sign != 0 ? sign : m_lastYDirection;

                if (pTouchMove.TouchDown)
                {
                    m_velocityStop = true;
                    int delta = Convert.ToInt32(_touchPreviousPoint.Y - pTouchMove.Location.Y);
                    ShiftList(delta);
                }
                else if(_touchPreviousPoint.TouchDown)
                {
                    if (pTouchMove.Velocity.VelocityY > m_mouseVelocityStartThreshold)
                    {
                        StartVelocity(Convert.ToInt32(pTouchMove.Velocity.VelocityY), m_lastYDirection);
                    }
                }
            }
            
            _touchPreviousPoint = pTouchMove;
        }


        private void StartVelocity(int pVelocity, int pDirection)
        {
            m_velocityDelegate = (i, d) =>
            {
                Velocity(i, d);
                //System.Threading.Thread.CurrentThread.Name = "Velocity";
            };
            m_velocityDelegate.BeginInvoke(pVelocity, pDirection, null, null);
        }

        private void Velocity(int pInitialVelocity, int pDirection)
        {
            double iterationMs = 10;
            double velocityDecrease = (float)pInitialVelocity / (1000 / iterationMs);

            // Coefficient of friction between metal & ice:
            double friction = 0.015;
            double zero = 0.1;

            m_velocityStop = false;
            while (Math.Abs(velocityDecrease) > zero && !m_velocityStop)
            {
                velocityDecrease = velocityDecrease - (friction * velocityDecrease);
                ShiftList((int)(velocityDecrease * pDirection));

                Thread.Sleep(10);
            }
        }

        private void ExecuteChangeList(DragableListSwitchDirection pDirection, int pNewIndex)
        {
            int directionSign = pDirection == DragableListSwitchDirection.Back ? -1 : 1;
            _touchPreviousPoint = null;

            NextList = m_listCollection[pNewIndex];
            double j = 1;

            // fast switch
            for (; Math.Abs(m_listHShift_px) < this.Width; )
            {
                if (Math.Abs(m_listHShift_px) < this.Width - 200)
                {
                    m_listHShift_px += directionSign * -50;
                }
                else
                {
                    m_listHShift_px += directionSign * (int)(-50 / j);
                    j+= .5;
                }

                Thread.Sleep(5);
            }

            m_listCollectionIndex += directionSign;
            CurrentList = m_listNextDisplay;
            m_listHShift_px = 0;

        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            this.m_listDisplayCount = Convert.ToInt32(this.Size.Height / m_listItemSize + 1);
        }
    }
    
    public class ListChangeEventArgs : EventArgs
    {
        public ListChangeEventArgs(DragableListSwitchDirection pSwitchDirection, int pNewIndex)
        {
            m_switchDirection = pSwitchDirection;
            m_newIndex = pNewIndex;
        }
        private int m_newIndex;
        public int NewIndex
        {
            get { return m_newIndex; }
        }

        private DragableListSwitchDirection m_switchDirection;
        public DragableListSwitchDirection SwitchDirection
        {
            get 
            {
                return m_switchDirection;
            }
        }
    }
    
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public SelectedItemChangedEventArgs(DragableListItem pDragableListItem,
                                            int pSelectedIndex)
        {
            SelectedItem = pDragableListItem;
            SelectedIndex = pSelectedIndex;
        }
        public int SelectedIndex { get; private set; }
        public DragableListItem SelectedItem { get; private set; }
        
    }
}

