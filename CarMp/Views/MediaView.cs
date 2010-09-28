using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.ViewControls;
using System.Xml;
using System.Runtime.Remoting.Messaging;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.MediaEntities;

namespace CarMP.Views
{
    public class MediaView : NavigationView, ISkinable
    {
        DragableList MediaList;
        GraphicalProgressBar ProgressBar;

        private Direct2D.BitmapData _background;
        private D2DBitmap Background = null;

        System.Timers.Timer ProgressBarTimer;

        private const string XPATH_BACKGROUND_IMAGE = "BackgroundImg";
        private const string XPATH_PROGRESSBAR = "GraphicalProgressBar";
        private const string XPATH_MEDIALIST = "MediaList";
        private const string XPATH_SHORTCUT_LIST = "ShortcutList";

        internal MediaView(SizeF pWindowSize)
            : base(pWindowSize)
        {
            MediaList = new DragableList();
            MediaList.Bounds = new RectF(20, 40, this.Width - 80, this.Height - 120);
            MediaList.SelectedItemChanged += new DragableList.SelectedItemChangedEventHandler(MediaList_SelectedItemChanged);

            AppMain.MediaManager.ListChangeRequest += (sender, e) =>
                {
                    MediaList.ChangeList(e.ListIndex);
                };

            MediaList.AfterListChanged += (sender, e) =>
                {
                    AppMain.MediaManager.ExecuteListChanged(e.NewIndex);
                };

            AddViewControl(MediaList);

            InitializeInitialListState();
        }

        public override string Name
        {
            get { return D2DViewFactory.MEDIA; }
        }

        public new void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            SkinningHelper.XmlBitmapEntry(XPATH_BACKGROUND_IMAGE, pSkinNode, pSkinPath, ref _background);
            XmlNode xmlNode = pSkinNode.SelectSingleNode(XPATH_PROGRESSBAR);
            if (xmlNode != null)
            {
                ProgressBar.ApplySkin(xmlNode, pSkinPath);
            }

            xmlNode = pSkinNode.SelectSingleNode(XPATH_SHORTCUT_LIST);
            if (xmlNode != null)
            {
                MediaShortcut shortcut = new MediaShortcut();
                shortcut.ApplySkin(xmlNode, pSkinPath);
                AddViewControl(shortcut);
                shortcut.StartRender();
            }

            xmlNode = pSkinNode.SelectSingleNode(XPATH_MEDIALIST);
            if (xmlNode != null)
            {
                MediaList.ApplySkin(xmlNode, pSkinPath);
                MediaList.StartRender();
            }

            MediaList.AfterListChanged += (sender, e) =>
                {
                    AppMain.MediaManager.ExecuteListChanged(e.NewIndex);
                };

            base.ApplySkin(pSkinNode, pSkinPath);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (Background == null
                && _background.Data != null)
            {
                Background = D2DStatic.GetBitmap(_background, pRenderTarget.Renderer);
            }
            if (Background != null)
            {
                pRenderTarget.DrawBitmap(Background, new RectF(0, 0, Bounds.Width, Bounds.Height));
            }

            base.OnRender(pRenderTarget);
        }

        private void InitializeInitialListState()
        {
            // Initial list- Current is first list, add two Root items:
            MediaList.InsertNextIntoCurrent(new RootItem("Media Library", RootItemType.DigitalMediaLibrary));
            MediaList.InsertNextIntoCurrent(new RootItem("File System", RootItemType.FileSystem));

            int listIndex = 1;
            foreach (MediaHistory item in AppMain.MediaManager.MediaListHistory)
            {
                listIndex++;
            }
        }

        public void OnBeforeMediaListChange(object sender, ListChangeEventArgs e)
        {

        }

        private void MediaList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            MediaListItem selectedItem = e.SelectedItem as MediaListItem;

            if (selectedItem == null)
                throw new Exception("Selected item is null or wrong type");

            switch (selectedItem.MediaType)
            {
                case MediaListItemType.Group:
                    new Action<int>(selectedIndex => 
                            {
                                AppMain.MediaManager.SetMediaHistory(selectedIndex, selectedItem);
                            }
                        ).BeginInvoke(
                        MediaList.CurrentListIndex,
                        null,
                        null);

                    MediaList.ClearAndFillNextList(AppMain.MediaManager.GetNewList(selectedItem).ToArray());
                    MediaList.ChangeListForward();
                    break;
                case MediaListItemType.Song:
                    AppMain.MediaManager.PlayMediaListItem(selectedItem);
                    break;
            }
        }

        private void MediaList_AfterListChanged(object sender, ListChangeEventArgs e)
        {
            //mediaListHistoryCurrentIndex += e.SwitchDirection == DragableListSwitchDirection.Back ? -1 : 0;
        }
    }
}
