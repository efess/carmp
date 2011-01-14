using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using CarMP.Helpers;
using CarMP.Graphics;

namespace CarMP.ViewControls
{
    public class ThermometerProgressBar : D2DViewControl, ISkinable
    {
        public event EventHandler ScrollChanged;

        // Graphics Resources
        private IImage OverlayMask = null;
        private IImage Background = null;
        private IBrush ProgressBrush = null;

        private string _backgroundPath;
        private string _overlayMaskPath;
        private Rectangle _progressBounds;

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
            SkinningHelper.XmlRectangleEntry(XPATH_BOUNDS, pXmlNode, ref _bounds);
            SkinningHelper.XmlValidFilePath(XPATH_OVERLAY_MASK, pXmlNode, pSkinPath, ref _overlayMaskPath);
            SkinningHelper.XmlValidFilePath(XPATH_BACKGROUND, pXmlNode, pSkinPath, ref _backgroundPath);
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            if (Background == null
                && !string.IsNullOrEmpty(_backgroundPath))
            {
                Background = pRenderer.CreateImage(_backgroundPath);
            }

            if (OverlayMask == null
                && !string.IsNullOrEmpty(_overlayMaskPath))
            {
                OverlayMask = pRenderer.CreateImage(_overlayMaskPath);
            }


            if(ProgressBrush == null)
            {
                ProgressBrush = pRenderer.CreateBrush(new Color(.8f,.2f,.2f));
            }

            _progressBounds = new Rectangle(0,
                0,
                CurrentValueXPosition(),
                Bounds.Height);

            if (Background != null)
                pRenderer.DrawImage(new Rectangle(0, 0, Bounds.Width, Bounds.Height), Background, 1f);

            pRenderer.FillRectangle(ProgressBrush, _progressBounds);

            if(OverlayMask != null)
                pRenderer.DrawImage(new Rectangle(0, 0, Bounds.Width, Bounds.Height), OverlayMask, 1f);

            //pRenderer.DrawRectangle(_grayBrush, new Rectangle(0, 0, (int)Bounds.Width, (int)Bounds.Height));
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
