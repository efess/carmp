using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMP.ViewControls
{
    public class ThermometerProgressBar : D2DViewControl, ISkinable
    {
        public event EventHandler ScrollChanged;

        // Direct2d Resources
        private D2DBitmap OverlayMask = null;
        private D2DBitmap Background = null;
        private SolidColorBrush ProgressBrush = null;

        private Direct2D.BitmapData _background;
        private Direct2D.BitmapData _overlayMask;
        private RectF _progressBounds;

        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_OVERLAY_MASK = "OverlayMask";
        private const string XPATH_BACKGROUND = "BackgroundImg";

        private int _maximumValue;
        public int MaximumValue { get { return _maximumValue; } set { _maximumValue = value; } }

        private int _minimumValue;
        public int MinimumValue { get { return _minimumValue; } set { _minimumValue = value; } }

        private float _value;
        public float Value { get { return _value; } set { _value = value; } }

        public ThermometerProgressBar()
        {
        }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pXmlNode, ref _bounds);
            SkinningHelper.XmlBitmapEntry(XPATH_OVERLAY_MASK, pXmlNode, pSkinPath, ref _overlayMask);
            SkinningHelper.XmlBitmapEntry(XPATH_BACKGROUND, pXmlNode, pSkinPath, ref _background);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (Background == null
                && _background.Data != null)
            {
                Background = D2DStatic.GetBitmap(_background, pRenderTarget.Renderer);
            }

            if (OverlayMask == null
                && _overlayMask.Data != null)
            {
                OverlayMask = D2DStatic.GetBitmap(_overlayMask, pRenderTarget.Renderer);
            }


            if(ProgressBrush == null)
            {
                ProgressBrush = pRenderTarget.Renderer.CreateSolidColorBrush(new ColorF(Colors.OrangeRed, 1f));
            }

            _progressBounds = new RectF(0,
                0,
                CurrentValueXPosition(),
                Bounds.Height);

            if (Background != null)
                pRenderTarget.DrawBitmap(Background, new RectF(0, 0, Bounds.Width, Bounds.Height));

            pRenderTarget.FillRectangle(ProgressBrush, _progressBounds);

            if(OverlayMask != null)
                pRenderTarget.DrawBitmap(OverlayMask, new RectF(0,0,Bounds.Width, Bounds.Height));
            //pRenderTarget.DrawRectangle(GrayBrush, new Rectangle(0, 0, (int)Bounds.Width, (int)Bounds.Height));
        }

        private float CurrentValueXPosition()
        {
            return (((float)Value * Bounds.Width) / (float)(MaximumValue - MinimumValue));
        }

        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            if(pTouchGesture.Gesture == Reactive.Touch.GestureType.Click)
                PositionChanged(pTouchGesture.X);
        }

        protected override void OnTouchMove(Reactive.Touch.TouchMove pTouchMove)
        {
            if(pTouchMove.TouchDown)
                PositionChanged(pTouchMove.X);
        }
        private void PositionChanged(float pX)
        {
            Value = (pX * (MaximumValue - MinimumValue)) / this.Width;
            OnScrollChanged();
        }
        //protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        //{
        //    Value = ((float)(e.X - this.Bounds.Left) / (float)this.Bounds.Width) * (float)(MaximumValue - MinimumValue);

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
