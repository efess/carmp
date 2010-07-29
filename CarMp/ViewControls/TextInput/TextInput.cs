using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX;
using System.Windows.Forms;

namespace CarMp.ViewControls
{
    public class TextInput : ViewControlCommonBase
    {
        private const string XPATH_TEXT = "Text";
        private const string XPATH_DEFAULT_TEXT = "DefaultText";

        private RectF _borderRect;
        private LinearGradientBrush _borderBrush;
        private SolidColorBrush _cursorLineBrush;
        private Text _text;

        private int _cursorPosition;
        private Point2F _cursorTopPoint;
        private Point2F _cursorBottomPoint;

        private bool _invalidateCursor;
        
        public TextInput()
        {
            _text = new Text();
        }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            Clear();
            
            _text = new Text();
            _text.SendMouseEventsToParent = true;
            _borderBrush = null;

            if (!SkinningHelper.ApplySkinNodeIfExists(XPATH_TEXT, pXmlNode, pSkinPath, _text))
            {
                throw new Exception("InputText view control doesn't contain a node for Text");
            }

            AddViewControl(_text);
            _text.StartRender();

            XmlNode node = pXmlNode.SelectSingleNode(XPATH_DEFAULT_TEXT);
            if (node != null)
                _text.TextString = node.InnerText;

            base.ApplySkin(pXmlNode, pSkinPath);
        }

        protected override void OnKeyPressed(Reactive.KeyInput.Key pKey)
        {
            char newChar = pKey.Character;
            switch(pKey.DotNetKeysValue)
            {
                case Keys.Back:
                    if (!string.IsNullOrEmpty(_text.TextString))
                        _text.TextString = _text.TextString.Remove(_cursorPosition - 1, 1);
                    _cursorPosition = _cursorPosition > 0 ? _cursorPosition - 1 : 0;
                    break;
                case Keys.Delete:
                    if (!string.IsNullOrEmpty(_text.TextString)
                        && _text.TextString.Length > _cursorPosition)
                        _text.TextString = _text.TextString.Remove(_cursorPosition, 1);
                    break;
                case Keys.Left:
                    _cursorPosition = _cursorPosition > 0 ? _cursorPosition - 1 : 0;
                    break;
                case Keys.Right:
                    _cursorPosition = _text.TextString.Length > _cursorPosition 
                        ? _cursorPosition + 1 
                        : _cursorPosition;
                    break;
                default:
                    if((int)pKey.DotNetKeysValue >= 32)
                    {
                        if (string.IsNullOrEmpty(_text.TextString))
                            _text.TextString = pKey.Character.ToString();
                        else
                            if(_cursorPosition == _text.TextString.Length)
                                _text.TextString = _text.TextString + pKey.Character.ToString();
                            else
                                _text.TextString = _text.TextString.Insert(_cursorPosition, pKey.Character.ToString());
                        _cursorPosition++;
                    }
                    break;
            }
            
            SetCursorCoordinatesPosition();
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            _borderRect = new RectF(2, 2, this.Width - 2, this.Height - 2);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);
            if (_borderBrush == null)
            {
                _borderBrush = Direct2D.GetBasicLinearGradient(pRenderTarget.Renderer,
                    _borderRect,
                    new ColorF(1f, 1f, 1f, .3f), new ColorF(1f, 1f, 1f, .8f));
            }
            if(_cursorLineBrush == null)
            {
                _cursorLineBrush = pRenderTarget.Renderer.CreateSolidColorBrush(new ColorF(Colors.White, 1));
            }

            if (true) // if (_invalidateCursor)
            {
                SetCursorCoordinatesPosition();
                _invalidateCursor = false;
            }

            if (D2DViewControl.HasInputControl == this)
                pRenderTarget.DrawLine(_cursorTopPoint, _cursorBottomPoint, _cursorLineBrush, 1);
            pRenderTarget.DrawRectangle(_borderBrush, _borderRect, 2.0f);

        }

        // Code will be changing _cursorPosition itself, change the coords
        // of cursor here.
        private void SetCursorCoordinatesPosition()
        {
            float xPosition = _text.GetWidthAtCharPosition(_cursorPosition);
            _cursorTopPoint = new Point2F(xPosition + _text.Location.X, 4);
            _cursorBottomPoint = new Point2F(xPosition + _text.Location.X, this.Height - 6);
        }

        // From X/Y, change Cursor Position, AND set coords
        private void SetTextPositionByCoords(Point2F pCursorPreferredLocation)
        {
            _cursorPosition = _text.GetTextPositionAtPoint(pCursorPreferredLocation);
            SetCursorCoordinatesPosition();
        }

        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            if (pTouchGesture.Gesture == Reactive.Touch.GestureType.Click)
            {
                SetTextPositionByCoords(pTouchGesture.Location);
            }
        }
    }
}
