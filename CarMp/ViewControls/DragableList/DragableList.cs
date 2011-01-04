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
    public class DragableList : ViewControlCommonBase, IViewList
    {
        private const string XPATH_ITEM_SIZE = "ItemSize";
        private const string XPATH_LIST_ORIENTATION = "ListOrientation";

        public delegate void SelectedItemChangedEventHandler(object sender, SelectedItemChangedEventArgs e);

        private delegate void ListChangeDelegate(int SwitchDirectionSign, int NewIndex);
        private delegate void VelocityStart(int InitialVelocity);

        public event SelectedItemChangedEventHandler SelectedItemChanged;

        private Orientation listOrientation = Orientation.Vertical;
        public Orientation ListOrientation
        {
            get { return listOrientation; }
            set 
            {
                if (listOrientation != value)
                {
                    listOrientation = value;
                    UpdateLengths();
                }
            }
        }

        // List varsx
        private int _lastYDirection;
        
        // Height of each list item
        private float _listItemSize = 25;

        /// <summary>
        /// Number of items that can be visible
        /// </summary>
        private int _listDisplayCount;

        /// <summary>
        /// Current displayed list
        /// </summary>
        private DragableListCollection _listItemCollection = new DragableListCollection(); // SHould base class these items
        private float _currentListLoc_px;

        private int _listItemCollectionIndex;

        private int _mouseVelocityStartThreshold = 2;
        
        //touch vars
        private TouchMove _touchPreviousPoint;

        // Velocity Delegate
        private Action<int,int> _velocityDelegate;
        private bool _velocityStop;

        /// <summary>
        /// Current List scrollable height in pixles (ListCount - ViewAble) * ItemSize
        /// </summary>
        private float scrollableListLengthPx;
        private float scrollableItemLengthPx;
        private SizeF itemSize;
        /// <summary>
        /// Size of each item in list. 
        /// Use -1 to specify length or width of window
        /// </summary>
        public SizeF ItemSize
        {
            get { return itemSize; }
            set 
            { 
                itemSize = new SizeF(value.Width <= 0 ? this.Width : value.Width,
                    value.Height <= 0 ? this.Height : value.Height);
                UpdateLengths();
            }
        }

        private int itemPerScrollaleRow = 1;
        public int ItemsPerRow
        {
            get { return itemPerScrollaleRow; }
            set {
                if (value <= 0)
                    throw new ArgumentException("Row Size cannot be less than one");
                itemPerScrollaleRow = value; 
            }
        }

        // Constructors
        public DragableList()
        {
        }        

        /// <summary>
        /// Vertical pixel offset of display list (0 - item.Height)
        /// </summary>
        private float ListLocOffsetPx
        {
            get
            {
                return listLocPx % _listItemSize;
            }
        }

        /// <summary>
        /// Returns current pixel size of whole list
        /// </summary>
        private float ListSize_px
        {
            get
            {
                return this._listItemCollection.Count * this._listItemSize;
            }
        }

        /// <summary>
        /// Returns current item count of the list
        /// </summary>
        private int ListCount
        {
            get
            {
                return _listItemCollection.Count;
            }
        }

        // Position of list in view
        private float listLocPx;
        internal float ListLocPx
        {
            set
            {
                if (value < 0)
                {
                    listLocPx = 0;
                }
                else if (value > this.scrollableListLengthPx)
                {
                    listLocPx = scrollableListLengthPx;
                }
                else
                {
                    listLocPx = value;
                }
            }
            get
            {
                return listLocPx;
            }
        }

        /// <summary>
        /// Returns list index at top of viewable portion
        /// </summary>
        private int ListItemViewIndexZero
        {
            get
            {
                if (this._listItemCollection.Count < _listDisplayCount)
                    return 0;
                else
                    return Convert.ToInt32(Math.Floor(listLocPx / _listItemSize));//(m_currentListLoc_px * (this.m_listCurrentDisplay.Count - CurrentListViewableItemCount)) / m_currentListSize_px;
            }
        }

        private int CurrentListViewableItemCount
        {
            get
            {
                return _listItemCollection.Count < _listDisplayCount ? _listItemCollection.Count : _listDisplayCount;
            }
        }

        /// <summary>
        /// Returns true if the CurrentList count is more than the viewable count
        /// </summary>
        private bool Scrollable
        {
            get
            {
                return _listItemCollection.Count > _listDisplayCount;
            }
        }

        /// <summary>
        /// Returns the item index for pixel location
        /// </summary>
        /// <param name="pPixel"></param>
        /// <returns></returns>
        private int GetItemAtPx(float pPixel)
        {
            if (this._listItemCollection.Count == 0)
                return 0;
            else
                return Convert.ToInt32(Math.Floor((pPixel * (this._listItemCollection.Count)) / (_listItemSize * _listItemCollection.Count)));
        }

        // Public Methods
        public void InsertNext(IEnumerable<DragableListItem> pItems)
        {
            foreach (DragableListItem item in pItems)
            {
                InsertNextInternal(item);
            }
            UpdateLengths();
        }

        public int Count
        {
            get { return _listItemCollection.Count; }
        }

        public void ClearList()
        {
            for (int j = 0; j < _listItemCollection.Count; j++)
            {
                _listItemCollection[j].Dispose();
            }
            _listItemCollection.Clear();
        }

        /// <summary>
        /// Inserts a display item in the next location in the current collection
        /// </summary>
        /// <param name="item"></param>
        public void InsertNext(DragableListItem item)
        {
            InsertNextInternal(item);
            UpdateLengths();
        }

        public override void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);

            var node = pXmlNode.SelectSingleNode(XPATH_ITEM_SIZE);
            if (node != null)
                ItemSize = XmlHelper.GetSize(node.InnerText);

            node = pXmlNode.SelectSingleNode(XPATH_LIST_ORIENTATION);
            if (node != null)
                try
                {
                    ListOrientation = (Orientation)Enum.Parse(typeof(Orientation), node.InnerText, true);
                }
                catch { }
        }

        private void InsertNextInternal(DragableListItem item)
        {
            //item.Bounds = new RectF(0, 0, this.Width, _listItemSize);

            item.Index = this._listItemCollection.Count;

            this._listItemCollection.Add(item);
        }

        private void UpdateLengths()
        {
            scrollableItemLengthPx = ListOrientation == Orientation.Vertical
                ? ItemSize.Height
                : ItemSize.Width;

            this.scrollableListLengthPx = (_listItemCollection.Count / itemPerScrollaleRow)
                * scrollableItemLengthPx;
            
        }

        // Private Methods

        private void D(string Message)
        {
            Debug.Print(DateTime.Now.ToString("hh:mm:ss") + " \t> " + Message);
        }

        protected virtual void SelectItem(int pMouseX, int pMouseY)
        {
            this._listItemCollection.SelectedIndex = GetItemAtPx(listLocPx + pMouseY);

            if (this._listItemCollection.SelectedIndex > -1)
            {
                // Execute Event
                if (SelectedItemChanged != null)
                {
                    SelectedItemChanged.BeginInvoke(this, 
                        new SelectedItemChangedEventArgs(
                            this._listItemCollection.SelectedItem, 
                            this._listItemCollection.SelectedIndex),
                        null, 
                        null);
                }
            }
        }


        private void ShiftList(int pDelta)
        {
            if (!Scrollable)
                return;
            this.ListLocPx += pDelta;
            //this._listItemCollection.BufferLoc = this.CurrentListItemViewIndexZero;
        }

        // Overrided Events

        protected override void PreRender()
        {
            this.Clear();
            int itemZero = ListItemViewIndexZero;

            for (int i = 0; i < this._listDisplayCount; i++)
            {
                if (itemZero + i < _listItemCollection.Count)
                {
                    D2DViewControl control = this._listItemCollection[itemZero + i];
                    int col = i % itemPerScrollaleRow;
                    int row = (i / itemPerScrollaleRow);

                    float shiftPx = (_listItemSize * row) - ListLocOffsetPx;
                    
                    RectF currentRect ;
                    if (ListOrientation == Orientation.Vertical)
                    {
                        float left = col * control.Width;
                        currentRect = new RectF(
                            left,
                            shiftPx,
                            left + ItemSize.Width,
                            ItemSize.Height + shiftPx);
                    }
                    else
                    {
                        float top = col * control.Height;
                        currentRect = new RectF(
                            shiftPx,
                            top,
                            ItemSize.Width + shiftPx,
                            top + ItemSize.Height);
                    }

                    control.Bounds = currentRect;
                    this.AddViewControl(control);
                }
            }
        }


        public override void SendUpdate(Reactive.ReactiveUpdate pReactiveUpdate)
        {
            if (Parent != null
                && Parent is IViewList)
                Parent.SendUpdate(pReactiveUpdate);

            base.SendUpdate(pReactiveUpdate);
        }
        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            switch (pTouchGesture.Gesture)
            {
                case Reactive.Touch.GestureType.Click:
                    SelectItem(Convert.ToInt32(pTouchGesture.Location.X), Convert.ToInt32(pTouchGesture.Location.Y));
                    return;
                //case Reactive.Touch.GestureType.SwipeLeft:
                //    ChangeListForward();
                //    return;
                //case Reactive.Touch.GestureType.SwipeRight:
                //    ChangeListBack();
                //    return;
            }
        }

        protected override void OnTouchMove(Reactive.Touch.TouchMove pTouchMove)
        {
            if (_touchPreviousPoint != null)
            {
                int sign = Math.Sign(_touchPreviousPoint.Y - pTouchMove.Location.Y);
                _lastYDirection = sign != 0 ? sign : _lastYDirection;

                if (pTouchMove.TouchDown)
                {
                    _velocityStop = true;
                    int delta = Convert.ToInt32(_touchPreviousPoint.Y - pTouchMove.Location.Y);
                    ShiftList(delta);
                }
                else if(_touchPreviousPoint.TouchDown)
                {
                    if (pTouchMove.Velocity.VelocityY > _mouseVelocityStartThreshold)
                    {
                        StartVelocity(Convert.ToInt32(pTouchMove.Velocity.VelocityY), _lastYDirection);
                    }
                }
            }
            
            _touchPreviousPoint = pTouchMove;
        }


        private void StartVelocity(int pVelocity, int pDirection)
        {
            _velocityDelegate = (i, d) =>
            {
                Velocity(i, d);
                //System.Threading.Thread.CurrentThread.Name = "Velocity";
            };
            _velocityDelegate.BeginInvoke(pVelocity, pDirection, null, null);
        }

        private void Velocity(int pInitialVelocity, int pDirection)
        {
            double iterationMs = 10;
            double velocityDecrease = (float)pInitialVelocity / (1000 / iterationMs);

            // Coefficient of friction between metal & ice:
            double friction = 0.015;
            double zero = 0.1;

            _velocityStop = false;
            while (Math.Abs(velocityDecrease) > zero && !_velocityStop)
            {
                velocityDecrease = velocityDecrease - (friction * velocityDecrease);
                ShiftList((int)(velocityDecrease * pDirection));

                Thread.Sleep(10);
            }
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            this._listDisplayCount = Convert.ToInt32(this.Size.Height / _listItemSize + 1);
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

