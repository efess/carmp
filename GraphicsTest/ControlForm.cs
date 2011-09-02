using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarMP.Graphics.Implementation.OpenGL;

namespace GraphicsTest
{
    public partial class ControlForm : Form
    {
        Font font;
        Brush brush;

        public ControlForm()
        {
            InitializeComponent();
        }


        private int _fps;
        public int FPS { get { return _fps; } set { _fps = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(font == null)
                font = new System.Drawing.Font("Arial", 12);
            if(brush == null)
                brush = new SolidBrush(Color.DarkSeaGreen);

            var graphics = e.Graphics;
            graphics.Clear(Color.DarkSlateGray);

            graphics.DrawString("Frames Per Second: " + _fps, font, brush, new Point(20, 20));

            base.OnPaint(e);
        }
    }
}
