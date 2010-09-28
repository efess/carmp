using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX;
using System.Xml;


namespace CarMP.ViewControls
{
    public class DragableListTextItem : DragableListItem
    {
        private TextLayout StringLayout = null;
        private LinearGradientBrush LinearGradient = null;

        private static TextFormat StringDrawFormat = null;
        public DragableListTextItem()
        {
            if (StringDrawFormat == null)
            {
                StringDrawFormat = D2DStatic.StringFactory.CreateTextFormat(
                    "Arial",
                    20F,
                    FontWeight.Normal,
                    FontStyle.Normal,
                    FontStretch.Normal, 
                    new System.Globalization.CultureInfo("en-us")) ;
            
                StringDrawFormat.TextAlignment = TextAlignment.Leading;
                StringDrawFormat.WordWrapping = WordWrapping.NoWrap;
            }

        }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);
        }

        /// <summary>
        /// String shown to the user
        /// </summary>
        public string DisplayString { get; set; }


        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderer)
        {
            if(StringLayout == null)
            {
                StringLayout = D2DStatic.StringFactory.CreateTextLayout(DisplayString, StringDrawFormat, _bounds.Width, _bounds.Height);
            }

            if (LinearGradient == null)
            {
                if (LinearGradient == null)
                    LinearGradient = pRenderer.Renderer.CreateLinearGradientBrush(
                            new LinearGradientBrushProperties()
                            {
                                StartPoint = new Point2F(0, 0),
                                EndPoint = new Point2F(0, _bounds.Height)
                            },
                            pRenderer.Renderer.CreateGradientStopCollection(new GradientStop[] {
                                new GradientStop
                                    {
                                        Color = new ColorF(Colors.Gray, 1),
                                        Position = 0
                                    }
                                    ,
                                new GradientStop
                                    {
                                        Color = new ColorF(Colors.White, 1),
                                        Position = 1
                                    }
                                },
                                Gamma.Gamma_10,
                                ExtendMode.Clamp
                        ));

            }

            LinearGradient.EndPoint = new Point2F(0, 0);
            LinearGradient.StartPoint = new Point2F(0, _bounds.Height);

            pRenderer.DrawTextLayout(new Point2F(4, 0), StringLayout, LinearGradient);
            
            // Call base which will draw the selection is selected
            base.OnRender(pRenderer);
        }
    }
}
