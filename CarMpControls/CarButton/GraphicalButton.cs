using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CarMpControls.CarButton
{
    public partial class GraphicalButton : Control
    {
        private bool _mouseDown;

        private Image _buttonDownImage;
        /// <summary>
        /// Image representin gthe button in the down "pressed" position
        /// </summary>
        public Image ButtonDownImage
        {
            get { return _buttonDownImage; }
            set { _buttonDownImage = value; }
        }
        private Image _buttonUpImage;
        /// <summary>
        /// Image representing the button in the normal position
        /// </summary>
        public Image ButtonUpImage
        {
            get { return _buttonUpImage; }
            set { _buttonUpImage = value; }
        }
        private string _buttonString;
        /// <summary>
        /// String that appears centered on the button
        /// </summary>
        public string ButtonString
        {
            get { return _buttonString; }
            set { _buttonString = value; }
        }

        public GraphicalButton()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.DrawString(_buttonString, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Blue), new Point(5, 5));

            if (_mouseDown)
            {
            }
            else
            {
            }
            base.OnPaint(pe);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseDown = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDown = true;
            Invalidate();

            base.OnMouseDown(e);
        }
    }
}
