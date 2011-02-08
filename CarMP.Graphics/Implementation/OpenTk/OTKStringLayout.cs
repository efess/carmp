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
    public class OTKStringLayout : IStringLayout
    {
        Bitmap _textBitmap;

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
            return new CarMP.Graphics.Geometry.Size(3,3);
        }

        private void UpdateBitmap()
        {
            string tempStr = _string;
            if (tempStr == null)
                tempStr = string.Empty;

           // System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(new Image
            // TODO: Should there be a string size restriction? (for word wrap I think so!)
            //TextLayoutResource = StringFactory.CreateTextLayout(
            //    tempStr,
            //    _textFormat,
            //    9000,
            //    9000);
        }

        internal Bitmap BitmapResource { get; private set; }

        internal OTKStringLayout()
        {
            UpdateBitmap();
        }

        internal OTKStringLayout(string pFont, float pSize)
        {
            UpdateBitmap();
        }
    }
}
