using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls
{
    public class Text : D2DViewControl, ISkinable
    {
        private const string XPATH_BOUNDS = "Bounds";
        
        private SlimDX.DirectWrite.TextLayout StringLayout = null;
        private SlimDX.Direct2D.SolidColorBrush ColorBrush = null;

        private SlimDX.DirectWrite.TextFormat StringDrawFormat = null;
        
        public Text()
        {

        }

        public void ApplySkin(XmlNode pSkinNode, string pSkinPath)
        {
            SkinningHelper.XmlRectangleFEntry(XPATH_BOUNDS, pSkinNode, ref _bounds);
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
            if (_textString == null) return;
            if (StringDrawFormat == null)
                StringDrawFormat = new SlimDX.DirectWrite.TextFormat(
                    Direct2D.StringFactory,
                    "Arial",
                    SlimDX.DirectWrite.FontWeight.Normal,
                    SlimDX.DirectWrite.FontStyle.Normal,
                    SlimDX.DirectWrite.FontStretch.Normal,
                    20F,
                    "en-us")
                    {
                        TextAlignment = SlimDX.DirectWrite.TextAlignment.Leading,
                        WordWrapping = SlimDX.DirectWrite.WordWrapping.NoWrap
                    };

            if (StringLayout == null)
                StringLayout = new SlimDX.DirectWrite.TextLayout(Direct2D.StringFactory, _textString, StringDrawFormat, Bounds.Width, Bounds.Height);

            if (ColorBrush == null)
                ColorBrush = new SlimDX.Direct2D.SolidColorBrush(pRenderTarget.Renderer, System.Drawing.Color.NavajoWhite);
            pRenderTarget.DrawTextLayout(new System.Drawing.PointF(0,0), StringLayout, ColorBrush);
        }
    }
}
