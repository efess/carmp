using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX;
using System.Windows.Forms;

namespace CarMP.ViewControls
{
    public class TextInput : Text
    {
        private const string XPATH_TEXT = "Text";
        private const string XPATH_DEFAULT_TEXT = "DefaultText";

        private RectF _borderRect;
        private LinearGradientBrush _borderBrush;
        private Point2F _cursorTopPoint;
        private Point2F _cursorBottomPoint;
        private SolidColorBrush _cursorLineBrush;
        private int millisecondWindow = 0;
        private int _cursorPosition;

        public TextInput()
        {
            this.TextPosition = new Point2F(2, 2);
        }

        private int GetCurrentMillisecondTimer()
        {
            return DateTime.Now.AddMilliseconds(-millisecondWindow).Millisecond;
        }
        private void ResetMillisecondTimer()
        {
            millisecondWindow = DateTime.Now.Millisecond;
        }

        public override void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);

            this.TextPosition = new Point2F(5, 4);   
            _borderBrush = null;

            XmlNode node = pXmlNode.SelectSingleNode(XPATH_DEFAULT_TEXT);
            if (node != null)
                TextString = node.InnerText;

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
                _borderBrush = D2DStatic.GetBasicLinearGradient(pRenderTarget.Renderer,
                    _borderRect,
                    new ColorF(1f, 1f, 1f, .3f), new ColorF(1f, 1f, 1f, .8f));
            }

            pRenderTarget.DrawRectangle(_borderBrush, _borderRect, 2.0f);
            
            SetCursorCoordinatesPosition();

            if (_cursorLineBrush == null)
                _cursorLineBrush = pRenderTarget.Renderer.CreateSolidColorBrush(new ColorF(Colors.White, 1));

            
            base.OnRender(pRenderTarget);

            if (D2DViewControl.HasInputControl == this)
                if (GetCurrentMillisecondTimer() < 500)
                    pRenderTarget.DrawLine(_cursorTopPoint, _cursorBottomPoint, _cursorLineBrush, 1);

        }

        protected override void OnKeyPressed(Reactive.KeyInput.Key pKey)
        {
            char newChar = pKey.Character;
            switch (pKey.DotNetKeysValue)
            {
                case Keys.Back:
                    if (!string.IsNullOrEmpty(TextString)
                        && _cursorPosition > 0)
                    {
                        _cursorPosition = _cursorPosition - 1;
                        TextString = TextString.Remove(_cursorPosition, 1);
                    }
                    break;
                case Keys.Delete:
                    if (!string.IsNullOrEmpty(TextString)
                        && TextString.Length > _cursorPosition)
                        TextString = TextString.Remove(_cursorPosition, 1);
                    break;
                case Keys.Left:
                    _cursorPosition = _cursorPosition > 0 ? _cursorPosition - 1 : 0;
                    break;
                case Keys.Right:
                    _cursorPosition = TextString.Length > _cursorPosition
                        ? _cursorPosition + 1
                        : _cursorPosition;
                    break;
                case Keys.Home:
                    _cursorPosition = 0;
                    break;
                case Keys.End:
                    _cursorPosition = TextString.Length;
                    break;
                default:
                    if ((int)pKey.Character >= 32)
                    {
                        if (string.IsNullOrEmpty(TextString))
                            TextString = pKey.Character.ToString();
                        else
                            if (_cursorPosition == TextString.Length)
                                TextString = TextString + pKey.Character.ToString();
                            else
                                TextString = TextString.Insert(_cursorPosition, pKey.Character.ToString());
                        _cursorPosition++;
                    }
                    break;
            }
            ResetMillisecondTimer();
            SetCursorCoordinatesPosition();
        }


        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            if (pTouchGesture.Gesture == Reactive.Touch.GestureType.Click)
            {
                SetTextPositionByCoords(pTouchGesture.Location);
            }
        }

        // Code will be changing _cursorPosition itself, change the coords
        // of cursor here.
        private void SetCursorCoordinatesPosition()
        {
            float xPosition = GetWidthAtCharPosition(_cursorPosition);
            float maxLength = !string.IsNullOrEmpty(TextString)
                ? GetWidthAtCharPosition(TextString.Length)
                : 0;

            // Check for OutOfBounds cursor position
            if (xPosition + TextPosition.X > (this.Bounds.Width - 5))
            {
                TextPosition = new Point2F((Bounds.Width - 20) - xPosition, TextPosition.Y);
            }
            else if (TextString != null && maxLength < this.Bounds.Width / 4)
            {
                TextPosition = new Point2F(5, 4);
            }
            else if (xPosition < Math.Abs(TextPosition.X))
            {
                if (xPosition < 5)
                    TextPosition = new Point2F(5, 4);
                else
                    TextPosition = new Point2F(-xPosition + 10, TextPosition.Y);
            }

            _cursorTopPoint = new Point2F(xPosition + TextPosition.X, 6);
            _cursorBottomPoint = new Point2F(xPosition + TextPosition.X, this.Height - 6);
        }

        // From X/Y, change Cursor Position, AND set coords
        private void SetTextPositionByCoords(Point2F pCursorPreferredLocation)
        {
            _cursorPosition = GetTextPositionAtPoint(pCursorPreferredLocation);
            SetCursorCoordinatesPosition();
        }
    }
}
