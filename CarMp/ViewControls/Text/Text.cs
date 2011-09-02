using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using System.Windows.Forms;
using CarMP.ViewControls.Interfaces;
using CarMP.Graphics.Interfaces;
using CarMP.Helpers;

namespace CarMP.ViewControls
{
    public class Text : ViewControlCommonBase, ISkinable, IDisposable, IText
    {
        private const string XPATH_TEXT_POSITION = "TextPosition";
        private const string XPATH_TEXT_STYLE = "TextStyle";

        private Point _textPosition;
        protected Point TextPosition { get { return _textPosition; } set { _textPosition = value; } }
        
        private IStringLayout _stringLayout = null;
        private IBrush _stringBrush = null;

        private object stringLayoutLock = new Object();
        
        internal bool SendMouseEventsToParent { get; set;}

        private bool _invalidateTextLayout = true;

        private TextStyle _textStyle;
        public TextStyle TextStyle { get { return _textStyle; }  set { _textStyle = value; _invalidateTextLayout = true; }}


        public void Dispose()
        {
            if(_stringLayout != null) GraphicsHelper.DisposeIfImplementsIDisposable(_stringLayout);
            base.Dispose();
        }
        
        public Text()
        {
            TextPosition = new Point(0, 0);
        }

        public override void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            base.ApplySkin(pSkinNode, pSkinPath);

            TextPosition = new Point(0, 0);
            SkinningHelper.XmlPointEntry(XPATH_TEXT_POSITION, pSkinNode, ref _textPosition);
            if (SkinningHelper.XmlTextStyleEntry(XPATH_TEXT_STYLE, pSkinNode, ref _textStyle))
                _invalidateTextLayout = true;
        }

        private string _textString;
        public string TextString
        {
            get { return _textString; }
            set
            {
                _textString = value;
                _invalidateTextLayout = true;
            }
        }

        public float GetWidthAtCharPosition(int pCharPosition)
        {
            float xPixelLocation = 0.0f;
            if (_stringLayout != null)
            {
                xPixelLocation = _stringLayout.GetPointAtCharPosition(pCharPosition).X;
            }
            return xPixelLocation;
        }

        public int GetTextPositionAtPoint(Point pPoint)
        {
            if (_stringLayout != null)
            {
                return _stringLayout.GetCharPositionAtPoint(pPoint);

            }
            return 0;
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);

            if (_textString == null
                || _textStyle == null) return;

            if(_stringBrush == null)
                _stringBrush = pRenderer.CreateBrush(_textStyle.Color1);
            //if (_textStyle.Format == null)
            //    _textStyle.Initialize(D2DStatic.StringFactory);


            if (_stringLayout == null || _invalidateTextLayout)
                _stringLayout = pRenderer.CreateStringLayout(_textString, _textStyle.Face, _textStyle.Size,_textStyle.Alignment);//D2DStatic.StringFactory.CreateTextLayout(_textString, _textStyle.Format, Bounds.Width, Bounds.Height);

            pRenderer.DrawString(new Rectangle(TextPosition, Bounds.Size), _stringLayout,  _stringBrush);
        }

        public override void SendUpdate(Reactive.ReactiveUpdate pReactiveUpdate)
        {
            if (Parent != null
                && SendMouseEventsToParent)
                Parent.SendUpdate(pReactiveUpdate);
            else
                base.SendUpdate(pReactiveUpdate);
        }
    }
}
