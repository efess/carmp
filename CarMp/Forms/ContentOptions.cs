using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CarMp.Forms
{
    public partial class ContentOptions : ContentBase
    {
        DigitalMediaScanner gScanner;

        public ContentOptions()
        {
            InitializeComponent();
            DisableStatus();
            SystemInformation.GetDeviceId();
        }

        private void graphicalButton1_Click(object sender, EventArgs e)
        {
            ApplicationMain.AppFormHost.ShowView(CarMp.Views.ViewFactory.HOME, true);
        }

        private void graphicalButton2_Click(object sender, EventArgs e)
        {
            lblPercent.Visible = true;
            lblPercent.Text = "0 %";

            lblStatus.Visible = true;
            lblStatus.Text = "Scanning...";

            pbStatus.Value = 0;
            pbStatus.Visible = true;

            gScanner = new DigitalMediaScanner();
            gScanner.Path = @"c:\music";

            gScanner.MediaOut = new MediaUpdate(MediaManager.SaveMediaToLibrary);
            gScanner.SupportedFormats.Add("MP3");
            gScanner.MediaUpdateSize = 1000;
            gScanner.ScanProgressChanged += new ProgressDelegate(ProgressChanged);
            gScanner.FinishScaning += new FinishHandler(FinishScaning);
            
            MediaManager.ClearMediaLibrary();

            gScanner.StartScan();
            btnStartScan.Text = "Building...";

            

        }

        private void ProgressChanged(object sender, ProgressEventArgs pEventArgs)
        {
            ThreadSafeProgressChanged(pEventArgs.Percent, pEventArgs.Status);
        }

        private void ThreadSafeProgressChanged(int pPercent, string pStatus)
        {
            if (lblPercent.InvokeRequired)
            {
                this.Invoke(new Action<int, string>((p, s) => ThreadSafeProgressChanged(p, s)), pPercent, pStatus);
            }
            else
            {
                lblStatus.Text = pStatus;
                lblPercent.Text = pPercent.ToString() + " %";
                pbStatus.Value = pPercent;
            }
        }

        private void DisableStatus()
        {
            lblPercent.Visible = false;
            lblStatus.Visible = false;
            pbStatus.Visible = false;
        }

        private void btnCancelScan_Click(object sender, EventArgs e)
        {
            if (gScanner != null)
            {
                gScanner.StopScan();
                btnStartScan.Text = "Build";
            }
        }

        private void FinishScaning(object sender, FinishEventArgs e)
        {
            gScanner.ScanProgressChanged -= new ProgressDelegate(ProgressChanged);
            MessageBox.Show("Scanning finished\nTotal time: " + e.TotalTime.Minutes.ToString() + ":" + e.TotalTime.Seconds.ToString()
                + "\nTotal Files: " + e.TotalCount.ToString() + " \nStarting Artist-Album group creation..", "Finished Scanning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            MediaGroupCreater creater = new MediaGroupCreater();
            creater.ProgressChanged += new ProgressDelegate(ProgressChanged);
            creater.CreationFinished += new FinishHandler(FinishedCreation);
            
            Action action = new Action(() => creater.ReCreateArtistAlbumGroupCreation());
            action.DynamicInvoke();          
        }

        private void FinishedCreation(object sender, FinishEventArgs e)
        {
            MessageBox.Show("Group creation finished\nTotal time: " + e.TotalTime.Minutes.ToString() + ":" + e.TotalTime.Seconds.ToString()
                , "Finished Group Creation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            this.Invoke(new Action(() => DisableStatus()));
        }
    }
}
