using System.Windows.Forms;
using DataObjectLayer;
using CarMpControls;
using System;

namespace CarMp.Forms
{
    public partial class FormDigitalAudio : Form
    {
        public FormDigitalAudio()
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
            foreach (MediaListItem item in MediaManager.GetRootLevelItems())
            {
            }
            MediaList.InsertNextIntoCurrent(MediaManager.GetRootLevelItems());

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
            ApplicationMain.AppFormHost.OpenForm(FormHost.HOME, true);
        }

        private void MediaList_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e)
        {
            MediaListItem selectedItem = e.SelectedItem as MediaListItem;
            
            if(selectedItem == null)
                throw new Exception("Selected item is null");

            if(selectedItem.ItemType == MediaItemType.Song)
            {
                MediaManager.StartPlayback(selectedItem.TargetId);
            }
            else
            {
                int listIndex = MediaList.CurrentListIndex;
                int newListIndex = listIndex + 1;

                if (newListIndex < MediaList.ListCollectionCount)
                {
                    MediaList.ClearListAtIndex(newListIndex, true);
                }
                foreach(MediaListItem item in MediaManager.GetNewMediaList(selectedItem.TargetId))
                {
                    MediaList.InsertNextIntoListIndex(item, newListIndex);
                }

                MediaList.ChangeList(newListIndex);
            }
        }

        private void MediaList_AfterListChanged(object sender, ListChangeEventArgs e)
        {
            //mediaListHistoryCurrentIndex += e.SwitchDirection == DragableListSwitchDirection.Back ? -1 : 0;
        }
    }
}
