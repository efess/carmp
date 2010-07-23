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
    public class Text : D2DViewControl, ISkinable
    {
        private const string XPATH_BOUNDS = "Bounds";
        private const string XPATH_BACKGROUND = "BackgroundImg";
        private const string XPATH_TEXT_POSITION = "TextPosition";
        private const string XPATH_TEXT_STYLE = "TextStyle";
        private Font _font;
        private D2DBitmap Background;
        private Point2F _textPosition = new Point2F(0, 0);
        private Direct2D.BitmapData _background;
        private TextStyle _textStyle;
        private TextLayout StringLayout = null;
        private SolidColorBrush ColorBrush = null;

        private TextFormat StringDrawFormat = null;
        
        public Text()
        {

        }

        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        { 
            _textPosition = new Point2F(0, 0);
            SkinningHelper.XmlPointFEntry(XPATH_TEXT_POSITION, pSkinNode,ref _textPosition);
            SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pSkinNode, ref _bounds);
            SkinningHelper.XmlBitmapEntry(XPATH_BACKGROUND, pSkinNode, pSkinPath, ref _background);
            SkinningHelper.XmlTextStyleEntry(XPATH_TEXT_STYLE, pSkinNode, ref _textStyle);
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
            if (Background == null
                && _background.Data != null)
            {
                Background = Direct2D.GetBitmap(_background, pRenderTarget.Renderer);
            }

            if (Background != null)
                pRenderTarget.DrawBitmap(Background, new RectF(0, 0, Bounds.Width, Bounds.Height));

            if (_textString == null
                || _textStyle == null) return;

            if (_textStyle.Format == null)
                _textStyle.Initialize(Direct2D.StringFactory);
            
            if (StringLayout == null)
                StringLayout = Direct2D.StringFactory.CreateTextLayout(_textString, _textStyle.Format, Bounds.Width, Bounds.Height);

            pRenderTarget.DrawTextLayout(_textPosition, StringLayout, _textStyle.GetBrush(pRenderTarget));
        }
    }
}
