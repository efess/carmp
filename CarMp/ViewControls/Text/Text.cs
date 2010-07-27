using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMp.ViewControls
{
    public class Text : ViewControlCommonBase, ISkinable, IDisposable
    {
        private const string XPATH_TEXT_POSITION = "TextPosition";
        private const string XPATH_TEXT_STYLE = "TextStyle";
        private Font _font;
        private Point2F _textPosition = new Point2F(0, 0);
        private TextStyle _textStyle;
        private TextLayout StringLayout = null;
        private SolidColorBrush ColorBrush = null;
        
        internal bool SendMouseEventsToParent { get; set;}

        public void Dispose()
        {
            if(StringLayout != null) StringLayout.Dispose();
            if(ColorBrush != null) ColorBrush.Dispose();
            if(_font != null) _font.Dispose();
            if(_textStyle != null) _textStyle.Dispose();
            base.Dispose();
        }

        private TextFormat StringDrawFormat = null;
        
        public Text()
        {

        }

        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            base.ApplySkin(pSkinNode, pSkinPath);

            _textPosition = new Point2F(0, 0);
            SkinningHelper.XmlPointFEntry(XPATH_TEXT_POSITION, pSkinNode,ref _textPosition);
            if (SkinningHelper.XmlTextStyleEntry(XPATH_TEXT_STYLE, pSkinNode, ref _textStyle))
                StringLayout = null;
        }

        private string _textString;
        public string TextString
        {
            get { return _textString; }
            set
            {
                _textString = value;
                StringLayout = null;
            }
        }
        
        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);

            if (_textString == null
                || _textStyle == null) return;

            if (_textStyle.Format == null)
                _textStyle.Initialize(Direct2D.StringFactory);
            
            if (StringLayout == null)
                StringLayout = Direct2D.StringFactory.CreateTextLayout(_textString, _textStyle.Format, Bounds.Width, Bounds.Height);

            pRenderTarget.DrawTextLayout(_textPosition, StringLayout, _textStyle.GetBrush(pRenderTarget));
        }

        public override void SendTouch(Reactive.Touch.Touch pTouch)
        {
            if (Parent != null
                && SendMouseEventsToParent)
                Parent.SendTouch(pTouch);
        }
    }
}
