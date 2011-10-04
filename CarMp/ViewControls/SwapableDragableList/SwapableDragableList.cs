using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using System.Threading;
using System.Xml;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls
{
    public class SwapableDragableList : ViewControlCommonBase, IViewList
    {
        private const string XPATH_ITEM_SIZE = "ItemSize";
        private const string XPATH_LIST_ORIENTATION = "ListOrientation";

        public delegate void ListChangedEventHandler(object sender, ListChangeEventArgs e);

        public event CarMP.ViewControls.DragableList.SelectedItemChangedEventHandler SelectedItemChanged;
        public event ListChangedEventHandler BeforeListChanged;
        public event ListChangedEventHandler AfterListChanged;

        private Orientation _listOrientation;
        public Orientation ListOrientation {
            get
            {
                return _listOrientation;
            }
            private set
            {
                _listOrientation = value;
                foreach (var list in _listCollection)
                    list.ListOrientation = _listOrientation;
            }
        }

        /// <summary>
        /// Collection of lists that can be navigated by this control
        /// </summary>
        private List<DragableList> _listCollection = new List<DragableList>();

        private int _currentListIndex;

        // Pixel shift during list changes
        private float _listShiftPx;

        public SwapableDragableList()
        {
        }

        public int CurrentListIndex
        {
            get { return _currentListIndex; }
        }

        private Size _itemSize = new Size(100, 25);
        public Size ItemSize
        {
            get { return _itemSize; }
            set
            {
                _itemSize = new Size(value.Width <= 0 ? this.Width : value.Width,
                    value.Height <= 0 ? this.Height : value.Height);
                foreach (var list in _listCollection)
                    list.ItemSize = _itemSize;
            }
        }

        /// <summary>
        /// Number of lists
        /// </summary>
        public int ListCollectionCount
        {
            get
            {
                return _listCollection.Count;
            }
        }

        // Base clears controls. Need to re-add dragable lists
        public virtual void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);

            if(_listCollection.Count > _currentListIndex)
                foreach(var control in _listCollection)
                    AddViewControl(control);

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

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            foreach (var dl in _listCollection)
            {
                dl.Bounds = new Rectangle(dl.Bounds.Left, dl.Bounds.Top, Bounds.Width, Bounds.Height);
            }
        }

        /// <summary>
        /// Inserts a display item in the next location in the specified list index
        /// </summary>
        /// <param name="item"></param>
        public void InsertNextIntoListIndex(DragableListItem item, int pListIndex)
        {
            GetOrCreateListAtIndex(pListIndex).InsertNext(item);
        }
        /// <summary>
        /// Inserts a display item in the next location in the specified list index
        /// </summary>
        /// <param name="item"></param>
        public void InsertNextIntoListIndex(IEnumerable<DragableListItem> itemList, int pListIndex)
        {
            GetOrCreateListAtIndex(pListIndex).InsertNext(itemList);
        }
        /// <summary>
        /// Inserts a display item in the next location in the current list 
        /// </summary>
        /// <param name="item"></param>
        public void InsertNext(DragableListItem pItem)
        {
            InsertNextIntoListIndex(pItem, _currentListIndex);
        }

        /// <summary>
        /// Inserts a display item in the next location in the current list 
        /// </summary>
        /// <param name="item"></param>
        public void InsertNext(IEnumerable<DragableListItem> pItems)
        {
            InsertNextIntoListIndex(pItems, _currentListIndex);
        }
        
        /// <summary>
        /// Clear current displayed list and fill with givem collection
        /// </summary>
        /// <param name="pList"></param>
        public void ClearAndFillNextList(IEnumerable<DragableListItem> pList)
        {
            int listIndex = CurrentListIndex;
            int newListIndex = listIndex + 1;

            if (newListIndex < ListCollectionCount)
            {
                ClearListAtIndex(newListIndex, true);
            }

            InsertNextIntoListIndex(pList, newListIndex);
        }

        /// <summary>
        /// Clears all dragable list items in list at specified listindex
        /// 
        /// </summary>
        /// <param name="pListIndex"></param>
        public void ClearListAtIndex(int pListIndex, bool pRemoveFutureLists)
        {
            if (pListIndex >= _listCollection.Count || pListIndex < 0)
            {
                throw new Exception("ListIndex is out of rage");
            }

            // Call dispose on each object
            for (int i = _listCollection.Count - 1; i >= pListIndex; i--)
            {
                if (pRemoveFutureLists || i == pListIndex)
                {
                    DragableList dragableList = _listCollection[i];
                    dragableList.ListLocPx = 0;

                    dragableList.ClearList();

                    //if (i != pListIndex)
                    RemoveList(dragableList);
                }
            }
        }

        private void OnSelectItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(sender, e);
        }

        private void RemoveList(DragableList pList)
        {
            if (_listCollection.Contains(pList))
            {
                base.Remove(pList);
                _listCollection.Remove(pList);
            }
        }

        private DragableList CreateList()
        {
            var newList = new DragableList();
            newList.Bounds = Bounds;
            newList.SelectedItemChanged += OnSelectItem;
            AddViewControl(newList);
            newList.ListOrientation = ListOrientation;
            newList.ItemSize = ItemSize;
            return newList;
        }

        private DragableList GetOrCreateListAtIndex(int pListIndex)
        {
            if (pListIndex < 0)
            {
                throw new Exception("ListIndex is out of rage n- Less than zero");
            }
            if ((pListIndex == 0
                && this._listCollection.Count == 0) ||
                (pListIndex == this._listCollection.Count &&
                this._listCollection[pListIndex - 1] != null))
            {
                this._listCollection.Add(CreateList());
            }
            else if (pListIndex > _listCollection.Count)
            {
                throw new Exception("ListIndex is out of range - greater than next insertion");
            }
            return _listCollection[pListIndex];
        }

        public DragableList FutureListTransition { get; private set; }

        /// <summary>
        /// Moves list forward as long as there is a list available
        /// </summary>
        public void ChangeListForward()
        {
            if (CurrentListIndex < _listCollection.Count)
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
                || pNewIndex >= _listCollection.Count)
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

        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            switch (pTouchGesture.Gesture)
            {
                //case Reactive.Touch.GestureType.Click:
                //    SelectItem(Convert.ToInt32(pTouchGesture.Location.X), Convert.ToInt32(pTouchGesture.Location.Y));
                //    return;
                case Reactive.Touch.GestureType.SwipeLeft:
                    ChangeListForward();
                    return;
                case Reactive.Touch.GestureType.SwipeRight:
                    ChangeListBack();
                    return;
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

        private void ExecuteChangeList(DragableListSwitchDirection pDirection, int pNewIndex)
        {
            int directionSign = pDirection == DragableListSwitchDirection.Back ? -1 : 1;

            var currentList = _listCollection[_currentListIndex];
            var transitionList = _listCollection[pNewIndex];
            transitionList.StartRender();

            float length = ListOrientation == Orientation.Vertical ? this.Width : this.Height;
            double j = 1;
            float listShift = 0;


            // fast switch
            while(Math.Abs(_listShiftPx) < length)
            {

                if (Math.Abs(_listShiftPx) < length - 200)
                {
                    _listShiftPx += directionSign * -100;
                }
                else
                {
                    _listShiftPx += directionSign * (int)(-100 / j);
                    j += .5;
                }
                if (Math.Abs(_listShiftPx) > length)
                    _listShiftPx = Math.Sign(_listShiftPx) * length;
                //_listShiftPx += directionSign * -10;

                if (ListOrientation == Orientation.Vertical)
                {
                    float transitionShift = _listShiftPx - (this.Width * Math.Sign(_listShiftPx));

                    currentList.Bounds = new Rectangle(_listShiftPx, 0, length, this.Height);
                    transitionList.Bounds = new Rectangle(transitionShift, 0, length, this.Height);
                }
                else
                {
                    float transitionShift = _listShiftPx - (this.Height * Math.Sign(_listShiftPx));

                    currentList.Bounds = new Rectangle(0, _listShiftPx, this.Width, length);
                    transitionList.Bounds = new Rectangle(0, transitionShift, this.Width, length);
                }
                Thread.Sleep(10);
            }

            currentList.Bounds = new Rectangle(this.Width, 0, this.Width, this.Height);

            transitionList.Bounds = new Rectangle(0, 0, this.Width, this.Height); 

            currentList.StopRenderer();
            FutureListTransition = null;
            _currentListIndex = pNewIndex;
            _listShiftPx = 0;
        }
        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);
        }
    }

    public class ListChangeEventArgs : EventArgs
    {
        public ListChangeEventArgs(DragableListSwitchDirection pSwitchDirection, int pNewIndex)
        {
            _switchDirection = pSwitchDirection;
            _newIndex = pNewIndex;
        }
        private int _newIndex;
        public int NewIndex
        {
            get { return _newIndex; }
        }

        private DragableListSwitchDirection _switchDirection;
        public DragableListSwitchDirection SwitchDirection
        {
            get
            {
                return _switchDirection;
            }
        }
    }
}
