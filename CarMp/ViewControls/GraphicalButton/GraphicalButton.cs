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
using CarMP.Graphics.Geometry;
using CarMP.ViewControls.Interfaces;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls
{
    public partial class GraphicalButton : D2DViewControl, ISkinable, IDisposable, IButton, ITrigger
    {
        public event EventHandler Click;

        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_TEXT = "Text";
        private const string XPATH_LAYOUT = "Layout";
        private const string XPATH_BUTTON_UP_IMAGE = "FaceUpImg";
        private const string XPATH_BUTTON_DOWN_IMAGE = "FaceDownImg";
        private Text _textControl;

        // Direct2d Resources
        private IImage _buttonUpImage = null;
        private IImage _buttonDownImage = null;

        public void Dispose()
        {
            if(_buttonUpImage != null) Helpers.GraphicsHelper.DisposeIfImplementsIDisposable(_buttonUpImage);
            if(_buttonDownImage != null) Helpers.GraphicsHelper.DisposeIfImplementsIDisposable(_buttonDownImage);
        }

        private bool _mouseDown;

        private string _buttonDownImagePath;
        private string _buttonUpImagePath;

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

            Helpers.SkinningHelper.XmlRectangleEntry(XPATH_BOUNDS, pXmlNode, ref _bounds);

            Helpers.SkinningHelper.XmlValidFilePath(XPATH_BUTTON_DOWN_IMAGE, pXmlNode, pSkinPath, ref _buttonDownImagePath);
            Helpers.SkinningHelper.XmlValidFilePath(XPATH_BUTTON_UP_IMAGE, pXmlNode, pSkinPath, ref _buttonUpImagePath);

            _buttonDownImage = null;
            _buttonUpImage = null;

            var xmlNode = pXmlNode.SelectSingleNode(XPATH_TEXT);
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
            _buttonUpImagePath = pImageFile;
            _buttonUpImage = null;
        }
        public void SetButtonDownBitmapData(string pImageFile)
        {
            _buttonDownImagePath = pImageFile;
            _buttonUpImage = null;
        }

        public GraphicalButton() { }

        protected override void OnRender(IRenderer pRenderer)
        {
            if (_buttonUpImage == null
                && !string.IsNullOrEmpty(_buttonUpImagePath))
            {
                _buttonUpImage = pRenderer.CreateImage(_buttonUpImagePath);
            }

            if (_buttonDownImage == null
                && !string.IsNullOrEmpty(_buttonDownImagePath))
            {
                _buttonDownImage = pRenderer.CreateImage(_buttonDownImagePath);
            }

            if (_buttonUpImage == null)
                return;
            Rectangle imageLocation = new Rectangle(
                0,
                0,
                _buttonUpImage.Size.Width,
                _buttonUpImage.Size.Height);

            if (_mouseDown)
            {
                if (_buttonDownImage != null)
                    pRenderer.DrawImage(imageLocation, _buttonDownImage, 1F);
            }
            else
            {
                if (_buttonUpImage != null)
                    pRenderer.DrawImage(imageLocation, _buttonUpImage, 1F);
            }
        }

        protected override void OnTouchGesture(TouchGesture pTouchGesture)
        {
            if (pTouchGesture.Gesture == GestureType.Click)
            {
                if(Click != null)
                    Click(this, new EventArgs());

                if (Trigger != null)
                    Trigger(null);
            }       
        }

        public Action<object> Trigger { get; set; }
    }
}
