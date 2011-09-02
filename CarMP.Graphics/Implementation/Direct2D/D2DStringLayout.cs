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
        private Rectangle _bounds;
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

            var metrics = _textLayoutResource.HitTestTextPosition((uint)pCharPosition, false, out x, out y);

            return new Point(x,  y);
        }

        public int GetCharPositionAtPoint(Point pPoint)
        {
            bool isTrailingHit;
            bool isInside;

            //var transformedPoint = TransformPoint(pPoint);
            var metrics = _textLayoutResource.HitTestPoint(pPoint.X, pPoint.Y, out isTrailingHit, out isInside);

            return isInside ? (int)metrics.TextPosition : 
                isTrailingHit ? _textLayoutResource.Text.Length : -1;
        }

        public Size GetStringSize()
        {
            return new Size(_textLayoutResource.Metrics.Width, _textLayoutResource.Metrics.Height);
        }

        private void UpdateTextFormat()
        {
            _textFormat = StringFactory.CreateTextFormat(
                _font,
                _size);
            _textFormat.TextAlignment =
                Alignment == StringAlignment.Left ? TextAlignment.Leading :
                Alignment == StringAlignment.Right ? TextAlignment.Trailing :
                TextAlignment.Center;
            UpdateTextLayout();
        }

        private void UpdateTextLayout()
        {
            string tempStr = _string;
            if (tempStr == null)
                tempStr = string.Empty;

            // TODO: Should there be a string size restriction? (for word wrap I think so!)
            _textLayoutResource = StringFactory.CreateTextLayout(
                tempStr,
                _textFormat,900,900);
        }

        private TextFormat _textFormat;

        private TextLayout _textLayoutResource;
        
        internal TextLayout GetTextLayout(Size pSize)
        {
            if (_textLayoutResource.MaxHeight != pSize.Width
                || _textLayoutResource.MaxWidth != pSize.Height)
            {
                _textLayoutResource.MaxWidth = pSize.Width;
                _textLayoutResource.MaxHeight = pSize.Height;
            }

            return _textLayoutResource;
        }

        internal D2DStringLayout(RenderTarget pRenderer)
        {
            UpdateTextFormat();
            UpdateTextLayout();
        }
        
        internal D2DStringLayout(RenderTarget pRenderer, string pString, string pFont, float pSize)
            :this(pRenderer, pString, pFont, pSize, StringAlignment.Left)
        {
        }
        internal D2DStringLayout(RenderTarget pRenderer, string pString, string pFont, float pSize, StringAlignment pAlignment)
        {
            _string = pString;
            _size = pSize;
            _font = pFont;
            _alignment = pAlignment;
            UpdateTextFormat();
            UpdateTextLayout();
        }
        
        private Point2F TransformPoint(Point pPoint)
        {
            return new Point2F(pPoint.X + _bounds.Left, pPoint.Y + _bounds.Top);
        }
    }
}
