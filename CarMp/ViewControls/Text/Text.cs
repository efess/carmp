using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;
using System.Windows.Forms;
using CarMP.ViewControls.Interfaces;

namespace CarMP.ViewControls
{
    public class Text : ViewControlCommonBase, ISkinable, IDisposable, IText
    {
        private const string XPATH_TEXT_POSITION = "TextPosition";
        private const string XPATH_TEXT_STYLE = "TextStyle";
        private Font _font;

        private Point2F _textPosition;
        protected Point2F TextPosition { get { return _textPosition; } set { _textPosition = value; } }
        
        private TextLayout StringLayout = null;

        private object stringLayoutLock = new Object();
        
        internal bool SendMouseEventsToParent { get; set;}

        private bool _invalidateTextLayout = true;

        private TextStyle _textStyle;
        public TextStyle TextStyle { get { return _textStyle; }  set { _textStyle = value; _invalidateTextLayout = true; }}

        public void Dispose()
        {
            if(StringLayout != null) StringLayout.Dispose();
            if(_font != null) _font.Dispose();
            if(_textStyle != null) _textStyle.Dispose();
            base.Dispose();
        }

        private TextFormat StringDrawFormat = null;
        
        public Text()
        {
            TextPosition = new Point2F(0, 0);
        }

        public override void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            base.ApplySkin(pSkinNode, pSkinPath);

            TextPosition = new Point2F(0, 0);
            SkinningHelper.XmlPointFEntry(XPATH_TEXT_POSITION, pSkinNode,ref _textPosition);
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
            if (StringLayout != null)
            {
                float yPixelLocation = 0.0f;
                StringLayout.HitTestTextPosition((uint)pCharPosition, false, out xPixelLocation, out yPixelLocation);
            }
            return xPixelLocation;
        }

        public int GetTextPositionAtPoint(Point2F pPoint)
        {
            if (StringLayout != null)
            {
                bool isTrailingHit;
                bool isInside;
                var metrics = StringLayout.HitTestPoint(pPoint.X, pPoint.Y, out isTrailingHit, out isInside);
                if (!isInside && TextString.Length == (int)metrics.TextPosition + 1)
                    return TextString.Length;
                return (int)metrics.TextPosition;
            }
            return 0;
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);

            if (_textString == null
                || _textStyle == null) return;

            if (_textStyle.Format == null)
                _textStyle.Initialize(D2DStatic.StringFactory);

            
            if (StringLayout == null || _invalidateTextLayout)
                StringLayout = D2DStatic.StringFactory.CreateTextLayout(_textString, _textStyle.Format, Bounds.Width, Bounds.Height);

            pRenderTarget.DrawTextLayout(TextPosition, StringLayout, _textStyle.GetBrush(pRenderTarget));
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
