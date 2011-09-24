using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;

namespace CarMP.ViewControls
{
    public class TextInput : Text
    {
        private const string XPATH_TEXT = "Text";
        private const string XPATH_DEFAULT_TEXT = "DefaultText";

        private Rectangle _borderRect;
        private IBrush _borderBrush;
        private Point _cursorTopPoint;
        private Point _cursorBottomPoint;
        private IBrush _cursorLineBrush;
        private int millisecondWindow = 0;
        private int _cursorPosition;

        public TextInput()
        {
            this.TextPosition = new Point(2, 2);
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

            this.TextPosition = new Point(5, 4);   
            _borderBrush = null;

            XmlNode node = pXmlNode.SelectSingleNode(XPATH_DEFAULT_TEXT);
            if (node != null)
                TextString = node.InnerText;

        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            _borderRect = new Rectangle(2, 2, this.Width - 4, this.Height - 4);
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);
            if (_cursorLineBrush == null)
                _cursorLineBrush = pRenderer.CreateBrush(Color.White);

            if (_borderBrush == null)
            {
                _borderBrush = pRenderer.CreateBrush(Color.White);
                // TODO: Gradients?
                //_borderBrush = D2DStatic.GetBasicLinearGradient(pRenderer.Renderer,
                //    _borderRect,
                //    new Color(1f, 1f, 1f, .3f), new Color(1f, 1f, 1f, .8f));
            }

            pRenderer.DrawRectangle(_borderBrush, _borderRect, 2.0f);
            
            SetCursorCoordinatesPosition();

            
            base.OnRender(pRenderer);

            if (D2DViewControl.HasInputControl == this)
                if (GetCurrentMillisecondTimer() < 500)
                    pRenderer.DrawLine(_cursorTopPoint, _cursorBottomPoint, _cursorLineBrush, 1);

        }

        protected override void OnKeyPressed(Reactive.KeyInput.Key pKey)
        {
            char newChar = pKey.Character;
            switch (pKey.KeysValue)
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
                TextPosition = new Point((Bounds.Width - 20) - xPosition, TextPosition.Y);
            }
            else if (TextString != null && maxLength < this.Bounds.Width / 4)
            {
                TextPosition = new Point(5, 4);
            }
            else if (xPosition < Math.Abs(TextPosition.X))
            {
                if (xPosition < 5)
                    TextPosition = new Point(5, 4);
                else
                    TextPosition = new Point(-xPosition + 10, TextPosition.Y);
            }

            _cursorTopPoint = new Point(xPosition + TextPosition.X, 6);
            _cursorBottomPoint = new Point(xPosition + TextPosition.X, this.Height - 6);
        }

        // From X/Y, change Cursor Position, AND set coords
        private void SetTextPositionByCoords(Point pCursorPreferredLocation)
        {
            _cursorPosition = GetTextPositionAtPoint(pCursorPreferredLocation);
            SetCursorCoordinatesPosition();
        }
    }
}
