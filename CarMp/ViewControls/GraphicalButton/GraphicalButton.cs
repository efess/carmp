using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX.Direct2D;
using System.Xml;
using System.IO;
using CarMp.Reactive.Touch;

namespace CarMp.ViewControls
{
    public partial class GraphicalButton : D2DViewControl, ISkinable
    {
        public event EventHandler Click;

        private const string XPATH_LAYOUT = "Layout";
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BUTTON_UP_IMAGE = "FaceUpImg";
        private const string XPATH_BUTTON_DOWN_IMAGE = "FaceDownImg";

        // Direct2d Resources
        private SlimDX.DirectWrite.TextLayout StringLayout = null;
        private SlimDX.DirectWrite.TextFormat StringDrawFormat = null;
        private SlimDX.Direct2D.SolidColorBrush StringBrush = null;
        private SlimDX.Direct2D.Bitmap ButtonUpBitmap = null;
        private SlimDX.Direct2D.Bitmap ButtonDownBitmap = null;

        ~GraphicalButton()
        {
            StringLayout.Dispose();
            StringDrawFormat.Dispose();
            StringBrush.Dispose();
            ButtonUpBitmap.Dispose();
            ButtonDownBitmap.Dispose();
        }

        private bool _mouseDown;

        private Direct2D.BitmapData _buttonDownImageData;
        private Direct2D.BitmapData _buttonUpImageData;

        private string _buttonString = "";

        /// <summary>
        /// String that appears centered on the button
        /// </summary>
        public string ButtonString
        {
            get { return _buttonString; }
            set { 
                _buttonString = value;
                StringLayout = null;
            }
        }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(XPATH_BUTTON_DOWN_IMAGE);
            if(xmlNode != null)
                _buttonDownImageData = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));

            xmlNode = pXmlNode.SelectSingleNode(XPATH_BUTTON_UP_IMAGE);
            if (xmlNode != null)
                _buttonUpImageData = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));
            
            if(!SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pXmlNode, ref _bounds))
            {
                throw new Exception("Button " + pXmlNode.Name + " does not contain a Bounds element");
            }
                   
        }

        public void SetButtonUpBitmapData(string pImageFile)
        {
            _buttonUpImageData = new Direct2D.BitmapData(pImageFile);
        }
        public void SetButtonDownBitmapData(string pImageFile)
        {
            _buttonDownImageData = new Direct2D.BitmapData(pImageFile);
        }

        public GraphicalButton()
        {
            SuscribeTouchGesture((tg) => OnClick(tg));

            if (StringDrawFormat == null)
                StringDrawFormat = new SlimDX.DirectWrite.TextFormat(
                    Direct2D.StringFactory,
                    "Arial",
                    SlimDX.DirectWrite.FontWeight.Normal,
                    SlimDX.DirectWrite.FontStyle.Normal,
                    SlimDX.DirectWrite.FontStretch.Normal,
                    20F,
                    "en-us") { TextAlignment = SlimDX.DirectWrite.TextAlignment.Leading };
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderer)
        {
            if (StringLayout == null)
            {
                StringLayout = new SlimDX.DirectWrite.TextLayout(Direct2D.StringFactory, _buttonString, StringDrawFormat, Bounds.Width, Bounds.Height);
            }

            if(StringBrush == null)
            {
                StringBrush = new SolidColorBrush(pRenderer.Renderer, (SlimDX.Color4)3453);
            }

            if (ButtonUpBitmap == null
                && _buttonUpImageData.Data != null)
            {
                ButtonUpBitmap = new SlimDX.Direct2D.Bitmap(pRenderer.Renderer, new Size(_buttonUpImageData.Width, _buttonUpImageData.Height), new SlimDX.DataStream(_buttonUpImageData.Data, true, false), _buttonUpImageData.Stride, _buttonUpImageData.BitmapProperties);
            }

            if (ButtonDownBitmap == null
                && _buttonDownImageData.Data != null)
            {
                ButtonDownBitmap = new SlimDX.Direct2D.Bitmap(pRenderer.Renderer, new Size(_buttonDownImageData.Width, _buttonDownImageData.Height), new SlimDX.DataStream(_buttonDownImageData.Data, true, false), _buttonDownImageData.Stride, _buttonDownImageData.BitmapProperties);
            }

            pRenderer.DrawTextLayout( new PointF(0, 0), StringLayout, StringBrush);
            
            //pRenderTarget.DrawTextring(_buttonString, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Blue), new Point(5, 5));

            RectangleF imageLocation = new RectangleF(
                0,
                0,
                _buttonUpImageData.Width,
                _buttonUpImageData.Height);

            if (_mouseDown)
            {
                if (ButtonDownBitmap != null)
                    pRenderer.DrawBitmap(ButtonDownBitmap, imageLocation);
            }
            else
            {
                if (ButtonUpBitmap != null)
                    pRenderer.DrawBitmap(ButtonUpBitmap, imageLocation);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseDown = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDown = true;
        }

        private void OnClick(TouchGesture pTouchGesture)
        {
            if (pTouchGesture.Gesture == GestureType.Click
                && Click != null)
            {
                Click(this, new EventArgs());
            }
        }
    }
}
