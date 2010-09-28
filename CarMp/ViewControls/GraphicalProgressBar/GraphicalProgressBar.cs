using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMP.ViewControls
{
    public class GraphicalProgressBar : D2DViewControl, ISkinable
    {
        public event EventHandler ScrollChanged;
        
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_HANDLE_IMAGE = "HandleImg";

        private Point2F _mouseOffsetOnHandle;
        private bool _mouseHasHandle;

        private RectF _currentHandleBounds;

        private static SolidColorBrush GrayBrush = null;

        private Direct2D.BitmapData _scrollBarHandleImageData;
        
        private D2DBitmap ScrollBarHandle = null;

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
            xmlNode = pXmlNode.SelectSingleNode(XPATH_HANDLE_IMAGE);
            if (xmlNode != null)
                _scrollBarHandleImageData = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if(GrayBrush == null)
                GrayBrush= pRenderTarget.Renderer.CreateSolidColorBrush(new ColorF(Colors.Gray, 1f));

            if (ScrollBarHandle == null
                && _scrollBarHandleImageData.Data != null)
            {
                ScrollBarHandle = D2DStatic.GetBitmap(_scrollBarHandleImageData, pRenderTarget.Renderer);
            }
            float currentPosition = CurrentHandleXPosition();
            _currentHandleBounds = new RectF(currentPosition,
                    0,
                    currentPosition + _scrollBarHandleImageData.Width,
                    _scrollBarHandleImageData.Height);
                    

            pRenderTarget.DrawBitmap(ScrollBarHandle, _currentHandleBounds);

            pRenderTarget.DrawRectangle(GrayBrush, new RectF(0, 0, Bounds.Width, Bounds.Height), 2F);
        }

        private int CurrentHandleXPosition()
        {
            return Convert.ToInt32(((float)Value / (float)(MaximumValue - MinimumValue))
            * (this.Bounds.Width - (float)_scrollBarHandleImageData.Width));
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
