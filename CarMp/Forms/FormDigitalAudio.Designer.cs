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
            this.MediaList = new CarMpControls.DragableList();
            this.SuspendLayout();
            // 
            // MediaList
            // 
            this.MediaList.Location = new System.Drawing.Point(12, 38);
            this.MediaList.Name = "MediaList";
            this.MediaList.Size = new System.Drawing.Size(386, 528);
            this.MediaList.TabIndex = 0;
            this.MediaList.Text = "dragableList1";
            // 
            // FormDigitalAudio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 578);
            this.Controls.Add(this.MediaList);
            this.Name = "FormDigitalAudio";
            this.Text = "FormDigitalAudio";
            this.ResumeLayout(false);

        }

        #endregion

        private CarMpControls.DragableList MediaList;
    }
}