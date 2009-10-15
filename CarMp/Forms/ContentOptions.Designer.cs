namespace CarMp.Forms
{
    partial class ContentOptions
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnCancelScan = new CarMpControls.CarButton.GraphicalButton();
            this.btnStartScan = new CarMpControls.CarButton.GraphicalButton();
            this.dragableList1 = new CarMpControls.DragableList();
            this.graphicalButton1 = new CarMpControls.CarButton.GraphicalButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbStatus = new System.Windows.Forms.ProgressBar();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(135, 26);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(351, 38);
            this.textBox1.TabIndex = 2;
            // 
            // btnCancelScan
            // 
            this.btnCancelScan.ButtonDownImage = null;
            this.btnCancelScan.ButtonString = "Cancel";
            this.btnCancelScan.ButtonUpImage = null;
            this.btnCancelScan.Location = new System.Drawing.Point(52, 55);
            this.btnCancelScan.Name = "btnCancelScan";
            this.btnCancelScan.Size = new System.Drawing.Size(75, 23);
            this.btnCancelScan.TabIndex = 4;
            this.btnCancelScan.Text = "graphicalButton3";
            this.btnCancelScan.Click += new System.EventHandler(this.btnCancelScan_Click);
            // 
            // btnStartScan
            // 
            this.btnStartScan.ButtonDownImage = null;
            this.btnStartScan.ButtonString = "Build";
            this.btnStartScan.ButtonUpImage = null;
            this.btnStartScan.Location = new System.Drawing.Point(66, 26);
            this.btnStartScan.Name = "btnStartScan";
            this.btnStartScan.Size = new System.Drawing.Size(63, 23);
            this.btnStartScan.TabIndex = 3;
            this.btnStartScan.Text = "graphicalButton2";
            this.btnStartScan.Click += new System.EventHandler(this.graphicalButton2_Click);
            // 
            // dragableList1
            // 
            this.dragableList1.Location = new System.Drawing.Point(150, 68);
            this.dragableList1.Name = "dragableList1";
            this.dragableList1.Size = new System.Drawing.Size(345, 208);
            this.dragableList1.TabIndex = 1;
            this.dragableList1.Text = "dragableList1";
            // 
            // graphicalButton1
            // 
            this.graphicalButton1.ButtonDownImage = null;
            this.graphicalButton1.ButtonString = "Home";
            this.graphicalButton1.ButtonUpImage = null;
            this.graphicalButton1.Location = new System.Drawing.Point(12, 3);
            this.graphicalButton1.Name = "graphicalButton1";
            this.graphicalButton1.Size = new System.Drawing.Size(75, 23);
            this.graphicalButton1.TabIndex = 0;
            this.graphicalButton1.Text = "graphicalButton1";
            this.graphicalButton1.Click += new System.EventHandler(this.graphicalButton1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblPercent);
            this.groupBox1.Controls.Add(this.lblStatus);
            this.groupBox1.Controls.Add(this.pbStatus);
            this.groupBox1.Location = new System.Drawing.Point(12, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 36);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.Location = new System.Drawing.Point(7, 17);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(0, 13);
            this.lblPercent.TabIndex = 8;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(240, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 7;
            // 
            // pbStatus
            // 
            this.pbStatus.Location = new System.Drawing.Point(40, 16);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(183, 14);
            this.pbStatus.TabIndex = 6;
            // 
            // ContentOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancelScan);
            this.Controls.Add(this.btnStartScan);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dragableList1);
            this.Controls.Add(this.graphicalButton1);
            this.Name = "ContentOptions";
            this.Size = new System.Drawing.Size(576, 495);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CarMpControls.CarButton.GraphicalButton graphicalButton1;
        private CarMpControls.DragableList dragableList1;
        private System.Windows.Forms.TextBox textBox1;
        private CarMpControls.CarButton.GraphicalButton btnStartScan;
        private CarMpControls.CarButton.GraphicalButton btnCancelScan;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar pbStatus;
    }
}