using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMP.ViewControls
{
    public class MediaShortcut : D2DViewControl, ISkinable
    {
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";
        private const string XPATH_DRAGABLE_LIST = "DragableList";


        private DragableList _internalList;

        public MediaShortcut()
        {
            _internalList = new DragableList();
            _internalList.StartRender();
        }
        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            Clear();

            SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pSkinNode, ref _bounds);
            _internalList.SelectedItemChanged += (sender, e) =>
            {
                var listItem = e.SelectedItem as ShortcutListItem;
                AppMain.MediaManager.SetList(listItem.ListIndex);
            };

            AppMain.MediaManager.ListChanged += (sender, e) =>
                {
                    // NEed to implement functionality to populate this shortcut list
                    // and to change in this lambda

                    _internalList.ClearListAtIndex(0, false);

                    int i = 0;
                    foreach(MediaHistory item in AppMain.MediaManager.MediaListHistory)
                    {
                        _internalList.InsertNextIntoCurrent(
                            new ShortcutListItem(item.ListIndex)
                            {
                                DisplayString = item.DisplayString,
                                Selected = item.ListIndex == i
                            });
                        i++;
                    }
                };
            XmlNode node = pSkinNode.SelectSingleNode(XPATH_DRAGABLE_LIST);
            if (node != null)
            {
                if(IndexOf(_internalList) < 0)
                    AddViewControl(_internalList);

                _internalList.ApplySkin(node, pSkinPath);
            }

        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
        }
        
        private class ShortcutListItem : DragableListTextItem
        {
            public int ListIndex { get; private set; }

            public ShortcutListItem(int pListIndex)
            {
                ListIndex = pListIndex;
            }
        }
    }
}
