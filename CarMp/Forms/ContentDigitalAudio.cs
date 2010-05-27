using System.Windows.Forms;
using DataObjectLayer;
using CarMpControls;
using System;

namespace CarMp.Forms
{
    public partial class ContentDigitalAudio : ContentBase
    {
        public ContentDigitalAudio()
        {
            InitializeComponent();


            InitializeInitialListState();

            //librarys ls = new librarys();
            //DoQueryExpression dqe = new DoQueryExpression();

            //dqe.Add(new DoQueryConstraint("artist", "Tool", QueryPredicate.Equal));
            //ls.Read(dqe);
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

        private void graphicalButton1_Click(object sender, System.EventArgs e)
        {
            ApplicationMain.AppFormHost.ShowView(FormHost.HOME, true);
        }

        private void MediaList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            DragableListTextItem selectedItem = e.SelectedItem as DragableListTextItem;
            
            if(selectedItem == null)
                throw new Exception("Selected item is null or wrong type");

            if (selectedItem is RootItem)
            {
                RootItem rootItem = selectedItem as RootItem;

                switch(rootItem.ItemType)
                {
                    case RootItemType.DigitalMediaLibrary:
                        MediaList.ClearAndFillNextList(MediaManager.GetMLRootLevelItems().ToArray());
                        MediaList.ChangeListForward();
                        break;
                    case RootItemType.FileSystem:
                        MediaList.ClearAndFillNextList(MediaManager.GetFSRootLevelItems().ToArray());
                        MediaList.ChangeListForward();
                        break;
                }
            }
            else if (selectedItem is MediaListItem)
            {
                MediaListItem mediaItem = selectedItem as MediaListItem;

                if (mediaItem.ItemType == MediaItemType.Song)
                {
                    MediaManager.StartPlayback(mediaItem.TargetId);
                }
                else
                {
                    MediaList.ClearAndFillNextList(MediaManager.GetNewMediaList(mediaItem.TargetId).ToArray());
                    MediaList.ChangeListForward();
                }
            }
            else if (selectedItem is FileSystemItem)
            {
                FileSystemItem fsItem = selectedItem as FileSystemItem;

                switch (fsItem.ItemType)
                {
                    case FileSystemItemType.HardDrive:
                    case FileSystemItemType.MemoryCard:
                    case FileSystemItemType.Directory:
                        MediaList.ClearAndFillNextList(MediaManager.GetNewFSMediaList(fsItem.FullPath).ToArray());
                        MediaList.ChangeListForward();
                        break;
                    case FileSystemItemType.AudioFile:
                        MediaManager.StartPlayback(fsItem.FullPath);
                        break;

                }
            }
            else
            {
                throw new Exception("Unknown media item type");
            }

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
