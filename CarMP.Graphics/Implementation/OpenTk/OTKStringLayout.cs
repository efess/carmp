using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Geometry;
using OpenTK.Graphics;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace CarMP.Graphics.Implementation.OpenTk
{
    public class OTKStringLayout : OTKImage, IStringLayout, IDisposable
    {
        private Bitmap _textBitmap;
        private Color _currentColor = Color.Black;

        private CarMP.Graphics.Geometry.Size _textSize;
        public CarMP.Graphics.Geometry.Size TextSize { get { return _textSize; } }
        private float _size;
        public float Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
                UpdateBitmap();
            }
        }

        private string _font;
        public string Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
                UpdateBitmap();
            }
        }

        private bool _wordWrap;
        public bool WordWrap
        {
            get
            {
                return _wordWrap;
            }
            set
            {
                _wordWrap = value;
                UpdateBitmap();
            }
        }

        private StringAlignment _alignment;
        public StringAlignment Alignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                _alignment = value;
                UpdateBitmap();
            }
        }

        private string _string;
        public string String
        {
            get
            {
                return _string;
            }
            set
            {
                _string = value;
                UpdateBitmap();
            }
        }

        //private static DWriteFactory _stringFactory;
        //public static DWriteFactory StringFactory
        //{
        //    get
        //    {
        //        if (_stringFactory == null)
        //            _stringFactory = DWriteFactory.CreateFactory();
        //        return _stringFactory;
        //    }
        //}

        public CarMP.Graphics.Geometry.Point GetPointAtCharPosition(int pCharPosition)
        {
            float x = 0.0f;
            float y = 0.0f;

           // var metrics = TextLayoutResource.HitTestTextPosition((uint)pCharPosition, false, out x, out y);

            return new CarMP.Graphics.Geometry.Point(x, y);
        }

        public int GetCharPositionAtPoint(CarMP.Graphics.Geometry.Point pPoint)
        {
            bool isTrailingHit;
            bool isInside;
            //var metrics = TextLayoutResource.HitTestPoint(pPoint.X, pPoint.Y, out isTrailingHit, out isInside);

            //return isInside ? (int)metrics.TextPosition : -1;
            return 1;
        }

        public CarMP.Graphics.Geometry.Size GetStringSize()
        {
            return _textSize;
        }

        private void UpdateBitmap()
        {
            string tempStr = _string;
            if (tempStr == null)
                tempStr = string.Empty;
            
            SizeF size;
            
            using(System.Drawing.Font font = new System.Drawing.Font(_font, _size))
            {
                using (Bitmap tmpBitmap = new Bitmap(1,1))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tmpBitmap))
                {
                    size = g.MeasureString(tempStr, font);
                    _textSize = new Geometry.Size(size.Width, size.Height);
                }

                Bitmap bitmap = new Bitmap((int)Math.Ceiling(size.Width), 
                    (int)Math.Ceiling(size.Height));
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                using(System.Drawing.Brush brush = new System.Drawing.SolidBrush(
                    System.Drawing.Color.FromArgb(
                        (int)(255 * _currentColor.Red),
                        (int)(255 * _currentColor.Green), 
                        (int)(255 * _currentColor.Blue))))
                {
                    g.DrawString(tempStr, font, brush, new PointF(0,0));
                    //bitmap.Save("C:\\bin\\lol.bmp");
                }
                
                LoadBitmap(bitmap);
            }
           // System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(new Image
            // TODO: Should there be a string size restriction? (for word wrap I think so!)
            //TextLayoutResource = StringFactory.CreateTextLayout(
            //    tempStr,
            //    _textFormat,
            //    9000,
            //    9000);
        }

        internal void SetBrush(IBrush pBrush)
        {
            if (!_currentColor.Equals(pBrush.Color))
            {
                _currentColor = pBrush.Color;
                UpdateBitmap();
            }
        }


        internal Bitmap BitmapResource { get; private set; }

        internal OTKStringLayout()
        {
            UpdateBitmap();
        }

        internal OTKStringLayout(string pString, string pFont, float pSize)
            : this(pString, pFont, pSize, StringAlignment.Left)
        {
        }

        internal OTKStringLayout(string pString, string pFont, float pSize, StringAlignment pAlignment)
        {
            _string = pString;
            _font = pFont;
            _size = pSize;
            _alignment = pAlignment;
            UpdateBitmap();
        }

        public void Dispose()
        {
            if (_textBitmap != null)
                _textBitmap.Dispose();
        }
    }
}
