using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;

namespace CarMP.ViewControls
{
    public class GraphicalProgressBar : D2DViewControl, ISkinable
    {
        public event EventHandler ScrollChanged;
        
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_HANDLE_IMAGE = "HandleImg";

        private Point _mouseOffsetOnHandle;
        private bool _mouseHasHandle;

        private Rectangle _currentHandleBounds;

        private static IBrush _grayBrush = null;

        private string _scrollBarHandleImagePath;
        private IImage _scrollBarHandle = null;

        private int _maximumValue;
        public int MaximumValue { get { return _maximumValue; } set { _maximumValue = value; } }

        private int _minimumValue;
        public int MinimumValue { get { return _minimumValue; } set { _minimumValue = value; } }

        private int _value;
        public int Value { get { return _value; } set { _value = value; } }

        private int _increment;
        public int Increment { get { return _increment; } set { _increment = value; } }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(XPATH_BOUNDS);
            if (xmlNode != null)
            {
                Bounds = XmlHelper.GetBoundsRectangle(xmlNode.InnerText);
            }
            
            Helpers.SkinningHelper.XmlValidFilePath(
                XPATH_HANDLE_IMAGE,
                pXmlNode,
                pSkinPath,
                ref _scrollBarHandleImagePath);
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            if(_grayBrush == null)
                _grayBrush= pRenderer.CreateBrush(Color.Gray);

            if (_scrollBarHandle == null
                && !string.IsNullOrEmpty(_scrollBarHandleImagePath))
            {
                _scrollBarHandle = pRenderer.CreateImage(_scrollBarHandleImagePath);
            }

            if (_scrollBarHandle == null)
                return;
            
            float currentPosition = CurrentHandleXPosition();
            _currentHandleBounds = new Rectangle(currentPosition,
                    0,
                    _scrollBarHandle.Size.Width,
                    _scrollBarHandle.Size.Height);
                    

            pRenderer.DrawImage(_currentHandleBounds, _scrollBarHandle, 1F);

            pRenderer.DrawRectangle(_grayBrush, new Rectangle(0, 0, Bounds.Width, Bounds.Height), 2F);
        }

        private int CurrentHandleXPosition()
        {
            if (_scrollBarHandle == null)
                return 0;

            return Convert.ToInt32(((float)Value / (float)(MaximumValue - MinimumValue))
            * (this.Bounds.Width - (float)_scrollBarHandle.Size.Width));
        }
        //protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (!this._mouseHasHandle)
        //        return;

        //    Value  = Convert.ToInt32((X + (float)(e.X - _mouseOffsetOnHandle.X)) / (this.Bounds.Width - (float)_scrollBarHandleImageData.Width)
        //                * (float)(MaximumValue - MinimumValue));

        //    OnScrollChanged();
        //}

        //protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (_currentHandleBounds.Contains(e.Location))
        //    {
        //        _mouseOffsetOnHandle = new Point(e.X + _currentHandleBounds.X,
        //            e.Y + _currentHandleBounds.Y);

        //        _mouseHasHandle = true;
        //    }
        //}

        //protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (_mouseHasHandle
        //        || !Bounds.Contains(e.Location)
        //        || _currentHandleBounds.Contains(e.Location))
        //    {
        //        _mouseHasHandle = false;
        //        return;
        //    }

        //    if (e.X < _currentHandleBounds.X)
        //    {
        //        // Skip backwards
        //        int newDistance = Value - Increment;
        //        if (newDistance < MinimumValue)
        //            Value = MinimumValue;
        //        else
        //            Value = newDistance;
        //    }
        //    else
        //    {
        //        // Skip forwards
        //        int newDistance = Value + Increment;
        //        if (newDistance > MaximumValue)
        //            Value = MaximumValue;
        //        else
        //            Value = newDistance;
        //    }

        //    OnScrollChanged();
        //}

        private void OnScrollChanged()
        {
            if (ScrollChanged != null)
            {
                ScrollChanged(this, new EventArgs());
            }
        }
    }
}
