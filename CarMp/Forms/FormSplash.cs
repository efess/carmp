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
    public partial class FormSplash : Form
    {
        Pen BorderPen = new Pen(new SolidBrush(SessionSettings.DefaultFontColor), 1);
        Pen ProgressPen = new Pen(new SolidBrush(SessionSettings.DefaultFontSpecialColor), 4);
        
        Rectangle BorderRect;
        Rectangle BorderLogo;
        Point ProgressPoint;
        Image Logo;
        string LoadingStatus = string.Empty;

        bool _notClosing = true;
        int _loadingPercent;

        private const int BORDER_PADDING = 5;

        public void ShowSplash()
        {
            this.Show();
            while (_notClosing)
            {
                System.Threading.Thread.Sleep(10);
                Application.DoEvents();
            }
        }

        public FormSplash()
        {
            InitializeComponent();

            Logo = CarMp.Properties.Resources.logo;

            BorderRect = new Rectangle(BORDER_PADDING, BORDER_PADDING, this.Width - (BORDER_PADDING * 2), this.Height - (BORDER_PADDING * 2));
            BorderLogo = new Rectangle((this.Width / 2) - (Logo.Width / 2), 10, Logo.Width, Logo.Height);
            ProgressPoint = new Point(10, 100);

            this.Paint += new PaintEventHandler(FormSplash_Paint);
        }

        void FormSplash_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(BorderPen, BorderRect);
            e.Graphics.DrawImage(Logo, BorderLogo);
            e.Graphics.DrawString(LoadingStatus, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(SessionSettings.DefaultFontColor), new PointF(10, 70));
            e.Graphics.DrawLine(ProgressPen, ProgressPoint, new Point((int)((double)268 * ((double)_loadingPercent / (double)100)),100));
        }

        public void CloseSplash()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => CloseSplash()));
            }
            else
            {
                _notClosing = false;
                this.Close();
                this.Dispose();
            }
        }

        public void IncreaseProgress(int pProgressPercent, string pStatus)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>((progress, status) => IncreaseProgress(progress, status)), pProgressPercent, pStatus);
            }
            else
            {
                LoadingStatus = pStatus;
                Percent = pProgressPercent;
                Refresh();
            }
        }

        public int Percent
        {
            get
            {
                return _loadingPercent;
            }
            set
            {
                if (value < 0)
                    _loadingPercent = 0;
                else if (value > 100)
                    _loadingPercent = 100;
                else
                    _loadingPercent = value;

                this.Invalidate();
            }
        }
    }
}
