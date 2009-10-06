namespace CarMp.Forms
{
    partial class FormDigitalAudio
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.graphicalButton1 = new CarMpControls.CarButton.GraphicalButton();
            this.MediaList = new CarMpControls.DragableList();
            this.SuspendLayout();
            // 
            // graphicalButton1
            // 
            this.graphicalButton1.ButtonDownImage = null;
            this.graphicalButton1.ButtonString = "Home";
            this.graphicalButton1.ButtonUpImage = null;
            this.graphicalButton1.Location = new System.Drawing.Point(23, 533);
            this.graphicalButton1.Name = "graphicalButton1";
            this.graphicalButton1.Size = new System.Drawing.Size(80, 33);
            this.graphicalButton1.TabIndex = 1;
            this.graphicalButton1.Text = "graphicalButton1";
            this.graphicalButton1.Click += new System.EventHandler(this.graphicalButton1_Click);
            // 
            // MediaList
            // 
            this.MediaList.Location = new System.Drawing.Point(12, 38);
            this.MediaList.Name = "MediaList";
            this.MediaList.Size = new System.Drawing.Size(386, 479);
            this.MediaList.TabIndex = 0;
            this.MediaList.Text = "dragableList1";
            this.MediaList.SelectedItemChanged += new CarMpControls.DragableList.SelectedItemChangedEventHandler(this.MediaList_SelectedItemChanged);
            this.MediaList.AfterListChanged += new CarMpControls.DragableList.ListChangedEventHandler(this.MediaList_AfterListChanged);
            // 
            // FormDigitalAudio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 578);
            this.Controls.Add(this.graphicalButton1);
            this.Controls.Add(this.MediaList);
            this.Name = "FormDigitalAudio";
            this.Text = "FormDigitalAudio";
            this.ResumeLayout(false);

        }

        #endregion

        private CarMpControls.DragableList MediaList;
        private CarMpControls.CarButton.GraphicalButton graphicalButton1;
    }
}