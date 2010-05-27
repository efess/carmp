using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CarMp.Forms
{
    public partial class ContentHome : ContentBase
    {
        public ContentHome()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void graphicalButton1_Click(object sender, EventArgs e)
        {
            ApplicationMain.AppFormHost.ShowView(FormHost.DIGITAL_AUDIO, true);
        }

        private void graphicalButton2_Click(object sender, EventArgs e)
        {
            ApplicationMain.AppFormHost.ShowView(FormHost.OPTIONS, true);
        }
    }
}
