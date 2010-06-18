using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMp.ViewControls;
using System.Xml;

namespace CarMp.Views
{
    public class MediaView : NavigationView, ISkinable
    {
        DragableList MediaList;
        GraphicalProgressBar ProgressBar;

        System.Timers.Timer ProgressBarTimer;
        
        private const string XPATH_PROGRESSBAR = "GraphicalProgressBar";
        private const string XPATH_MEDIALIST = "MediaList";

        internal MediaView(System.Drawing.Size pWindowSize)
            : base(pWindowSize)
        {
            MediaList = new DragableList();
            MediaList.Bounds = new System.Drawing.RectangleF(20, 40, this.Width - 80, this.Height - 120);
            MediaList.SelectedItemChanged += new DragableList.SelectedItemChangedEventHandler(MediaList_SelectedItemChanged);
            MediaList.StartRender();

            AddViewControl(MediaList);

            InitializeInitialListState();
        }

        public override string Name
        {
            get { return D2DViewFactory.MEDIA; }
        }

        public new void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            XmlNode xmlNode = pSkinNode.SelectSingleNode(XPATH_PROGRESSBAR);
            if (xmlNode != null)
            {
                ProgressBar.ApplySkin(xmlNode, pSkinPath);
            }

            xmlNode = pSkinNode.SelectSingleNode(XPATH_MEDIALIST);
            if (xmlNode != null)
            {
                MediaList.ApplySkin(xmlNode, pSkinPath);
            }

            base.ApplySkin(pSkinNode, pSkinPath);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);
        }

        private void InitializeInitialListState()
        {
            // Initial list- Current is first list, add two Root items:
            MediaList.InsertNextIntoCurrent(new RootItem("Media Library", RootItemType.DigitalMediaLibrary));
            MediaList.InsertNextIntoCurrent(new RootItem("File System", RootItemType.FileSystem));

            int listIndex = 1;
            foreach (MediaListItem item in MediaManager.MediaListHistory)
            {
                listIndex++;
            }
        }

        public void OnBeforeMediaListChange(object sender, ListChangeEventArgs e)
        {

        }

        //private void graphicalButton1_Click(object sender, System.EventArgs e)
        //{
        //    ApplicationMain.AppFormHost.OpenContent(FormHost.HOME, true);
        //}

        //private void graphicalButton1_Click(object sender, System.EventArgs e)
        //{
        //    ApplicationMain.AppFormHost.OpenContent(FormHost.HOME, true);
        //}

        private void MediaList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            MediaListItem selectedItem = e.SelectedItem as MediaListItem;

            if (selectedItem == null)
                throw new Exception("Selected item is null or wrong type");

            switch (selectedItem.MediaType)
            {
                case MediaListItemType.Group:
                    MediaList.ClearAndFillNextList(MediaManager.GetNewList(selectedItem).ToArray());
                    MediaList.ChangeListForward();
                    break;
                case MediaListItemType.Song:
                    MediaManager.PlayMediaListItem(selectedItem);
                    break;
            }
            //if (selectedItem is RootItem)
            //{
            //    RootItem rootItem = selectedItem as RootItem;

            //    switch (rootItem.ItemType)
            //    {
            //        case RootItemType.DigitalMediaLibrary:
            //            break;
            //        case RootItemType.FileSystem:
            //            MediaList.ClearAndFillNextList(MediaManager.GetFSRootLevelItems().ToArray());
            //            MediaList.ChangeListForward();
            //            break;
            //    }
            //}
            //else if (selectedItem is MediaListItem)
            //{
            //    DigitalMediaItem mediaItem = selectedItem as DigitalMediaItem;

            //    if (mediaItem.ItemType == DigitalMediaItemType.Song)
            //    {
            //        MediaManager.StartPlayback(mediaItem);
            //    }
            //    else
            //    {
            //        MediaList.ClearAndFillNextList(MediaManager.GetNewMediaList(mediaItem.TargetId).ToArray());
            //        MediaList.ChangeListForward();
            //    }
            //}
            //else if (selectedItem is FileSystemItem)
            //{
            //    FileSystemItem fsItem = selectedItem as FileSystemItem;

            //    switch (fsItem.ItemType)
            //    {
            //        case FileSystemItemType.HardDrive:
            //        case FileSystemItemType.MemoryCard:
            //        case FileSystemItemType.Directory:
            //            MediaList.ClearAndFillNextList(MediaManager.GetNewFSMediaList(fsItem.FullPath).ToArray());
            //            MediaList.ChangeListForward();
            //            break;
            //        case FileSystemItemType.AudioFile:
            //            MediaManager.StartPlayback(fsItem);
            //            break;

            //    }
            //}
            //else
            //{
            //    throw new Exception("Unknown media item type");
            //}
        }

        private void MediaList_AfterListChanged(object sender, ListChangeEventArgs e)
        {
            //mediaListHistoryCurrentIndex += e.SwitchDirection == DragableListSwitchDirection.Back ? -1 : 0;
        }

        private void graphicalButton3_Click(object sender, EventArgs e)
        {

        }

        private void graphicalButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
