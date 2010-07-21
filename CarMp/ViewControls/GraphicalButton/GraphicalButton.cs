using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using CarMp.Reactive.Touch;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

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
        private TextLayout StringLayout = null;
        private TextFormat StringDrawFormat = null;
        private SolidColorBrush StringBrush = null;
        private D2DBitmap ButtonUpBitmap = null;
        private D2DBitmap ButtonDownBitmap = null;

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
            
            if (StringDrawFormat == null)
            {
                StringDrawFormat = Direct2D.StringFactory.CreateTextFormat(
                    "Arial",
                    20F,
                    FontWeight.Normal,
                    FontStyle.Normal,
                    FontStretch.Normal,
                    new System.Globalization.CultureInfo("en-us"));

                StringDrawFormat.TextAlignment = TextAlignment.Center;
                StringDrawFormat.WordWrapping = WordWrapping.Wrap;
            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderer)
        {
            if (StringLayout == null)
            {
                StringLayout = Direct2D.StringFactory.CreateTextLayout(_buttonString, StringDrawFormat, Bounds.Width, Bounds.Height);
            }

            if(StringBrush == null)
            {
                StringBrush = pRenderer.Renderer.CreateSolidColorBrush(new ColorF(Colors.White, 1F));
            }

            if (ButtonUpBitmap == null
                && _buttonUpImageData.Data != null)
            {
                ButtonUpBitmap = Direct2D.GetBitmap(_buttonUpImageData, pRenderer.Renderer);
            }

            if (ButtonDownBitmap == null
                && _buttonDownImageData.Data != null)
            {
                ButtonDownBitmap = Direct2D.GetBitmap(_buttonDownImageData, pRenderer.Renderer);
            }

            pRenderer.DrawTextLayout(new Point2F(0, 0), StringLayout, StringBrush);
            
            //pRenderTarget.DrawTextring(_buttonString, new Font(FontFamily.GenericSansSerif, 12), new SolidBrush(Color.Blue), new Point(5, 5));

            RectF imageLocation = new RectF(
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

        protected override void OnTouchGesture(TouchGesture pTouchGesture)
        {
            if (pTouchGesture.Gesture == GestureType.Click
                && Click != null)
            {
                Click(this, new EventArgs());
            }       
        }
    }
}
