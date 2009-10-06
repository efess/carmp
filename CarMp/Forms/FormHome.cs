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
    public partial class FormHome : Form
    {
        public FormHome()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void graphicalButton1_Click(object sender, EventArgs e)
        {
            ApplicationMain.AppFormHost.OpenForm(FormHost.DIGITAL_AUDIO, true);
        }

        private void graphicalButton2_Click(object sender, EventArgs e)
        {
            ApplicationMain.AppFormHost.OpenForm(FormHost.OPTIONS, true);
        }
    }
}
