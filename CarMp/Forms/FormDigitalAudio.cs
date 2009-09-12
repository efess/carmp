﻿using System.Windows.Forms;
using DataObjectLayer;
using CarMpControls;

namespace CarMp.Forms
{
    public partial class FormDigitalAudio : Form
    {
        int mediaListHistoryCurrentIndex;

        public FormDigitalAudio()
        {
            InitializeComponent();


            MediaList.InsertNextIntoCurrent(MediaManager.RootLevelItems);
            librarys ls = new librarys();
            DoQueryExpression dqe = new DoQueryExpression();

            dqe.Add(new DoQueryConstraint("artist", "Tool", QueryPredicate.Equal));
            ls.Read(dqe);
        }

        private void InitializeInitialListState()
        {
            int listIndex = 1;
            foreach (MediaListItem item in MediaManager.MediaListHistory)
            {

                listIndex++;
            }
        }

        public void OnBeforeMediaListChange(object sender, ListChangeEventArgs e)
        {

        }

    }
}
