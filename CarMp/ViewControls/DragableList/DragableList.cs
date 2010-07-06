using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Xml;


namespace CarMp.ViewControls
{
    public class DragableList : D2DViewControl, ISkinable
    {
        private const string XPATH_BOUNDS = "Bounds";

        public delegate void ListChangedEventHandler(object sender, ListChangeEventArgs e);
        public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs e);

        private delegate void ListChangeDelegate(int SwitchDirectionSign, int NewIndex);
        private delegate void VelocityStart(int InitialVelocity);

        public event ListChangedEventHandler BeforeListChanged;
        public event ListChangedEventHandler AfterListChanged;
        public event SelectedItemChangedEventHandler SelectedItemChanged;

        // List vars
        
        // Height of each list item
        private int m_listItemSize = 25;

        // Horizontal shift during list changes
        private int m_listHShift_px;

        /// <summary>
        /// Current List scrollable height in pixles (ListCount - ViewAble) * ItemSize
        /// </summary>
        private int m_currentListSize_px;
        /// <summary>
        /// Next List scrollable height in pixles (ListCount - ViewAble) * ItemSize
        /// </summary>
        private int m_nextListSize_px;

        /// <summary>
        /// Number of items that can be visible
        /// </summary>
        private int m_listDisplayCount;

        /// <summary>
        /// Used to bypass mouse event processing when internal operations are occurring
        /// </summary>
        private bool m_ignoreMouseEvents;

        /// <summary>
        /// Current displayed list
        /// </summary>
        private DragableListCollection m_listCurrentDisplay = new DragableListCollection(); // SHould base class these items
        private int m_currentListLoc_px;
        
        /// <summary>
        /// Next list shown only during transition - reference is assigned to m_listContents immediately after
        /// </summary>
        private DragableListCollection m_listNextDisplay = new DragableListCollection();
        private int m_nextListLoc_px;

        /// <summary>
        /// Collection of lists that can be navigated by this control
        /// </summary>
        private List<DragableListCollection> m_listCollection = new List<DragableListCollection>();
        private int m_listCollectionIndex;

        // Mouse vars
        private Boolean m_mouseDown;
        private Boolean m_mouseListLock;

        private DateTime m_mouseDownTime;
        private int m_mouseDownTimeMsSelectTheshold = 200;
        private int m_mouseDownTimePxSelectThreashold = 5;

        private Point m_mouseDownPoint;
        private Point m_mouseDownLastMove;
        private int m_mouseMoveVelocity;
        private int m_mouseVelocityStartThreshold = 2;
        private int m_mouseListChangeThreshold = 50;
        
        // Velocity Delegate
        private VelocityStart m_velocityDelegate;
        private bool m_velocityStop;

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(XPATH_BOUNDS);
            if (xmlNode != null)
            {
                Bounds = XmlHelper.GetBoundsRectangle(xmlNode.InnerText);
            }
            else
            {
                throw new Exception("DragableList " + pXmlNode.Name + " does not contain a Bounds element");
            }
        }

        // Constructors
        public DragableList()
        {
            this.m_mouseDown = false;
            this.m_ignoreMouseEvents = false;

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
                    m_currentListSize_px = (m_listCurrentDisplay.Count * m_listItemSize) - Convert.ToInt32(this.Height);
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
        private int CurrentListLocVertOffset_px
        {
            get
            {
                return m_currentListLoc_px % m_listItemSize;
            }
        }

        /// <summary>
        /// Vertical pixel offset of display list (0 - item.Height)
        /// </summary>
        private int NextListLocVertOffset_px
        {
            get
            {
                return m_nextListLoc_px % m_listItemSize;
            }
        }

        /// <summary>
        /// Returns Pixel location of top of display list
        /// </summary>
        private int ListLoc_px
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
        private int ListSize_px
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
                    return m_currentListLoc_px / m_listItemSize;//(m_currentListLoc_px * (this.m_listCurrentDisplay.Count - CurrentListViewableItemCount)) / m_currentListSize_px;
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
                    return (m_nextListLoc_px * (this.m_listNextDisplay.Count - NextListViewableItemCount)) / m_nextListSize_px;
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
        private int GetItemAtPx(int pPixel)
        {
            if (this.m_listCurrentDisplay.Count == 0)
                return 0;
            else
                return (pPixel * (this.m_listCurrentDisplay.Count)) / (m_listItemSize * CurrentList.Count);
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
            item.ClientSize = new Size(Convert.ToInt32(this.Width), m_listItemSize);

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

            item.ClientSize = new Size(Convert.ToInt32(this.Width), m_listItemSize);
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
                    SelectedItemChanged.BeginInvoke(this, new SelectedItemChangedEventArgs(this.m_listCurrentDisplay.SelectedItem), null, null);
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
        // Overrided Events

        protected override void PreRender()
        {
            this.Clear();
            
            for (int i = 0; i < this.m_listDisplayCount; i++)
            {
                if (this.CurrentListItemViewIndexZero + i < m_listCurrentDisplay.Count)
                {
                    RectangleF currentRect = new RectangleF(
                        m_listHShift_px, 
                        (m_listItemSize * i) - CurrentListLocVertOffset_px,
                        this.Width,
                        m_listItemSize);
                    D2DViewControl control = this.m_listCurrentDisplay[this.CurrentListItemViewIndexZero + i];
                    control.StartRender();
                    control.Bounds = currentRect;
                    this.AddViewControl(control);
                    
                    //this.m_listCurrentDisplay[this.CurrentListItemViewIndexZero + i].DrawItem(pRenderTarget, currentRect);
                }

                if (m_listHShift_px != 0 && i < m_listNextDisplay.Count)
                {

                    RectangleF nextRect = new RectangleF(
                        m_listHShift_px - (this.Width * Math.Sign(m_listHShift_px)), 
                        ((m_listItemSize * i) - NextListLocVertOffset_px),
                        this.Width,
                        m_listItemSize);
                    D2DViewControl control = this.m_listNextDisplay[this.NextListItemViewIndexZero + i];
                    control.StartRender();
                    control.Bounds = nextRect;
                    this.AddViewControl(control);
                }
            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!this.m_mouseDown || m_ignoreMouseEvents)
                return;
            
            e = FixMouseEventArgs(e);

            int x = m_mouseDownLastMove.X - e.X;
            int y = m_mouseDownLastMove.Y - e.Y;

            if (Math.Abs(x) > Math.Abs(y))
            {
                m_mouseMoveVelocity = x;
            }
            else
            {
                m_mouseMoveVelocity = y;
            }

            this.ShiftList(this.m_mouseDownLastMove.Y - e.Y);
            this.m_mouseDownLastMove = e.Location;

            if (m_mouseListLock)
                return;

            if (Math.Abs(this.m_mouseDownLastMove.X - e.X) > 5 || Math.Abs(this.m_mouseDownLastMove.Y - e.Y) > 5)
            {
                m_mouseListLock = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (m_ignoreMouseEvents)
                return;

            e = FixMouseEventArgs(e);
            this.m_mouseDown = false;

            TimeSpan tempDownTimeSpan = (DateTime.Now - this.m_mouseDownTime);
            int downTimeMs = (tempDownTimeSpan.Seconds * 1000) + tempDownTimeSpan.Milliseconds;

            if (this.m_mouseDownTimeMsSelectTheshold > downTimeMs
                && Math.Abs(m_mouseDownPoint.Y - e.Y) < m_mouseDownTimePxSelectThreashold)
            {
                SelectItem(e.X, e.Y);
                return;
            }

            int horizontalMove =  m_mouseDownPoint.X - e.X;

            // Check list change threshold
            if (Math.Abs(horizontalMove) > m_mouseListChangeThreshold)
            {
                ChangeListHorizontalDistance(horizontalMove);
                return;
            }

            // Check list move threshold
            if (Math.Abs(m_mouseMoveVelocity) > m_mouseVelocityStartThreshold)
            {
                StartVelocity();
            }
            
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (m_ignoreMouseEvents)
                return;

            e = FixMouseEventArgs(e);
            this.m_mouseDown = true;
            this.m_velocityStop = true;
            this.m_mouseDownPoint = e.Location;
            this.m_mouseDownLastMove = e.Location;
            this.m_mouseMoveVelocity = 0;
            
            this.m_mouseDownTime = DateTime.Now;

            base.OnMouseDown(e);
        }

        private void StartVelocity()
        {
            m_velocityDelegate = new VelocityStart(Velocity);
            m_velocityDelegate.BeginInvoke(m_mouseMoveVelocity, null, null);
        }

        private void Velocity(int pInitialVelocity)
        {
            double velocityDecrease = (float)pInitialVelocity;

            // Coefficient of friction between metal & ice:
            double friction = 0.02;
            double zero = 1;

            m_velocityStop = false;
            while (Math.Abs(velocityDecrease) > zero && !m_velocityStop)
            {
                velocityDecrease = velocityDecrease - (friction * velocityDecrease);
                ShiftList((int)velocityDecrease);

                Thread.Sleep(10);
            }
        }

        private void ExecuteChangeList(DragableListSwitchDirection pDirection, int pNewIndex)
        {
            int directionSign = pDirection == DragableListSwitchDirection.Back ? -1 : 1;
            m_ignoreMouseEvents = true;

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

            m_ignoreMouseEvents = false;
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            this.m_listDisplayCount = Convert.ToInt32(this.Size.Height) / m_listItemSize + 1;
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
        public SelectedItemChangedEventArgs(DragableListItem pDragableListItem)
        {
            m_selectedItem = pDragableListItem;
        }

        private DragableListItem m_selectedItem;
        public DragableListItem SelectedItem
        {
            get
            {
                return m_selectedItem;
            }
        }
    }
}
