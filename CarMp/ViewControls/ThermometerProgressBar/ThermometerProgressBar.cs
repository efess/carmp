using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls
{
    public class ThermometerProgressBar : D2DViewControl, ISkinable
    {
        public event EventHandler ScrollChanged;

        // Direct2d Resources
        private SlimDX.Direct2D.Bitmap OverlayMask = null;
        private SlimDX.Direct2D.Brush ProgressBrush = null;

        private Direct2D.BitmapData _overlayMask;
        private RectangleF _progressBounds;

        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_OVERLAY_MASK = "OverlayMask";

        private int _maximumValue;
        public int MaximumValue { get { return _maximumValue; } set { _maximumValue = value; } }

        private int _minimumValue;
        public int MinimumValue { get { return _minimumValue; } set { _minimumValue = value; } }

        private int _value;
        public int Value { get { return _value; } set { _value = value; } }

        public ThermometerProgressBar()
        {
        }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pXmlNode, ref _bounds);
            SkinningHelper.XmlBitmapEntry(XPATH_OVERLAY_MASK, pXmlNode,pSkinPath, ref _overlayMask);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            if (OverlayMask == null
                && _overlayMask.Data != null)
            {
                OverlayMask = Direct2D.GetBitmap(_overlayMask, pRenderTarget.Renderer);
            }

            if(ProgressBrush == null)
            {
                ProgressBrush = new SlimDX.Direct2D.SolidColorBrush(
                    pRenderTarget.Renderer,
                    Color.OrangeRed);
            }

             _progressBounds = new RectangleF(0,
                    0,
                    CurrentValueXPosition(),
                    Bounds.Height);

             pRenderTarget.FillRectangle(ProgressBrush, _progressBounds);
             pRenderTarget.DrawBitmap(OverlayMask, new System.Drawing.RectangleF(0,0,Bounds.Width, Bounds.Height));
            //pRenderTarget.DrawRectangle(GrayBrush, new Rectangle(0, 0, (int)Bounds.Width, (int)Bounds.Height));
        }

        private float CurrentValueXPosition()
        {
            return (((float)Value * Bounds.Width) / (float)(MaximumValue - MinimumValue));
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Value = Convert.ToInt32(((float)(e.X - this.Bounds.X) / (float)this.Bounds.Width) * (float)(MaximumValue - MinimumValue));

            OnScrollChanged();
        }
        private void OnScrollChanged()
        {
            if (ScrollChanged != null)
            {
                ScrollChanged(this, new EventArgs());
            }
        }
    }
}
