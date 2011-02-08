using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Implementation.Direct2D
{
    public class D2DStringLayout : IStringLayout
    {
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
                UpdateTextFormat();
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
                UpdateTextFormat();
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
                UpdateTextFormat();
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
                UpdateTextFormat();
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
                UpdateTextLayout();
            }
        }

        private static DWriteFactory _stringFactory;
        public static DWriteFactory StringFactory
        {
            get
            {
                if (_stringFactory == null)
                    _stringFactory = DWriteFactory.CreateFactory();
                return _stringFactory;
            }
        }

        public Point GetPointAtCharPosition(int pCharPosition)
        {
            float x;
            float y;

            var metrics = TextLayoutResource.HitTestTextPosition((uint)pCharPosition, false, out x, out y);

            return new Point(x, y);
        }

        public int GetCharPositionAtPoint(Point pPoint)
        {
            bool isTrailingHit;
            bool isInside;
            var metrics = TextLayoutResource.HitTestPoint(pPoint.X, pPoint.Y, out isTrailingHit, out isInside);

            return isInside ? (int)metrics.TextPosition : -1;
        }

        public Size GetStringSize()
        {
            return new Size(TextLayoutResource.Metrics.Width, TextLayoutResource.Metrics.Height);
        }

        private void UpdateTextFormat()
        {
            _textFormat = StringFactory.CreateTextFormat(
                _font,
                _size);

        }

        private void UpdateTextLayout()
        {
            string tempStr = _string;
            if (tempStr == null)
                tempStr = string.Empty;

            // TODO: Should there be a string size restriction? (for word wrap I think so!)
            TextLayoutResource = StringFactory.CreateTextLayout(
                tempStr,
                _textFormat,
                9000,
                9000);
        }

        private TextFormat _textFormat;

        internal TextLayout TextLayoutResource;

        internal D2DStringLayout(RenderTarget pRenderer)
        {
            UpdateTextFormat();
            UpdateTextLayout();
        }
        
        internal D2DStringLayout(RenderTarget pRenderer, string pString, string pFont, float pSize)
        {
            _string = pString;
            _size = pSize;
            _font = pFont;
            UpdateTextFormat();
            UpdateTextLayout();
        }
    }
}
