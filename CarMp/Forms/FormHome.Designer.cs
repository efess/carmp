namespace CarMp.Forms
{
    partial class FormHome
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
            this.graphicalButton2 = new CarMpControls.CarButton.GraphicalButton();
            this.graphicalButton1 = new CarMpControls.CarButton.GraphicalButton();
            this.SuspendLayout();
            // 
            // graphicalButton2
            // 
            this.graphicalButton2.ButtonDownImage = null;
            this.graphicalButton2.ButtonString = "Options";
            this.graphicalButton2.ButtonUpImage = null;
            this.graphicalButton2.Location = new System.Drawing.Point(142, 446);
            this.graphicalButton2.Name = "graphicalButton2";
            this.graphicalButton2.Size = new System.Drawing.Size(75, 38);
            this.graphicalButton2.TabIndex = 1;
            this.graphicalButton2.Text = "graphicalButton2";
            this.graphicalButton2.Click += new System.EventHandler(this.graphicalButton2_Click);
            // 
            // graphicalButton1
            // 
            this.graphicalButton1.ButtonDownImage = null;
            this.graphicalButton1.ButtonString = "Media";
            this.graphicalButton1.ButtonUpImage = null;
            this.graphicalButton1.Location = new System.Drawing.Point(23, 446);
            this.graphicalButton1.Name = "graphicalButton1";
            this.graphicalButton1.Size = new System.Drawing.Size(86, 38);
            this.graphicalButton1.TabIndex = 0;
            this.graphicalButton1.Text = "graphicalButton1";
            this.graphicalButton1.Click += new System.EventHandler(this.graphicalButton1_Click);
            // 
            // FormHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 506);
            this.Controls.Add(this.graphicalButton2);
            this.Controls.Add(this.graphicalButton1);
            this.Name = "FormHome";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private CarMpControls.CarButton.GraphicalButton graphicalButton1;
        private CarMpControls.CarButton.GraphicalButton graphicalButton2;

    }
}

