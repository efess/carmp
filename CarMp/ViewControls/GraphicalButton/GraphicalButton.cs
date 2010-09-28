using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using CarMP.Reactive.Touch;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMP.ViewControls
{
    public partial class GraphicalButton : D2DViewControl, ISkinable, IDisposable
    {
        public event EventHandler Click;

        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_TEXT = "Text";
        private const string XPATH_LAYOUT = "Layout";
        private const string XPATH_BUTTON_UP_IMAGE = "FaceUpImg";
        private const string XPATH_BUTTON_DOWN_IMAGE = "FaceDownImg";
        private Text _textControl;

        // Direct2d Resources
        private D2DBitmap ButtonUpBitmap = null;
        private D2DBitmap ButtonDownBitmap = null;

        public void Dispose()
        {
            if(ButtonUpBitmap != null) ButtonUpBitmap.Dispose();
            if(ButtonDownBitmap != null) ButtonDownBitmap.Dispose();
        }

        private bool _mouseDown;

        private Direct2D.BitmapData _buttonDownImageData;
        private Direct2D.BitmapData _buttonUpImageData;

        private string _buttonString = "";
        public string ButtonString { 
            get { return _buttonString; }
            set { _buttonString = value;
            if (_textControl != null)
                _textControl.TextString = value;
            }
        }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            this.Clear();

            SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pXmlNode, ref _bounds);
            XmlNode xmlNode = pXmlNode.SelectSingleNode(XPATH_BUTTON_DOWN_IMAGE);

            if (xmlNode != null)
            {
                _buttonDownImageData = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));
                ButtonDownBitmap = null;
            }

            xmlNode = pXmlNode.SelectSingleNode(XPATH_BUTTON_UP_IMAGE);
            if (xmlNode != null)
            {
                _buttonUpImageData = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));
                ButtonUpBitmap = null;
            }
            
            xmlNode = pXmlNode.SelectSingleNode(XPATH_TEXT);
            if (xmlNode != null)
            {
                _textControl = new Text();
                _textControl.ApplySkin(xmlNode, pSkinPath);
                _textControl.TextString = _buttonString;
                AddViewControl(_textControl);
                _textControl.SendMouseEventsToParent = true;
                _textControl.StartRender();
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

        public GraphicalButton() { }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderer)
        {
            if (ButtonUpBitmap == null
                && _buttonUpImageData.Data != null)
            {
                ButtonUpBitmap = D2DStatic.GetBitmap(_buttonUpImageData, pRenderer.Renderer);
            }

            if (ButtonDownBitmap == null
                && _buttonDownImageData.Data != null)
            {
                ButtonDownBitmap = D2DStatic.GetBitmap(_buttonDownImageData, pRenderer.Renderer);
            }


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
