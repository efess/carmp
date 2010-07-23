using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMp
{
    public class TextStyle : IDisposable
    {
        private const string SIZE = "Size";
        private const string FACE = "Face";
        private const string COLOR_ONE = "ColorOne";
        private const string COLOR_TWO = "ColorTwo";
        private const string WORD_WRAP = "WordWrap";
        private const string ALIGNMENT = "Alignment";

        public float Size { get; private set; }
        public string Face { get; private set; }
        public float[] Color1 { get; private set; }
        public float[] Color2 { get; private set; }
        public bool WordWrap { get; private set; }
        public TextAlignment Alignment { get; private set;}

        public TextFormat Format { get; private set;}

        private Brush _textBrush;
        private RectF _bounds;

        public TextStyle(XmlNode pXmlNode)
        {
            foreach (XmlNode node in pXmlNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case SIZE:
                        Size = (float)Convert.ToDouble(node.InnerText);
                        break;
                    case FACE:
                        Face = node.InnerText;
                        break;
                    case COLOR_ONE:
                        Color1 = ConvertColor(node.InnerText);
                        break;
                    case COLOR_TWO:
                        Color2 = ConvertColor(node.InnerText);
                        break;
                    case WORD_WRAP:
                        WordWrap = Convert.ToBoolean(node.InnerText);
                        break;
                    case ALIGNMENT:
                        Alignment = (TextAlignment)Enum.Parse(typeof(TextAlignment), node.InnerText);
                        break;
                }
            }
        }

        private float[] ConvertColor(string pValue)
        {
            if(pValue.Contains(','))
                return XmlHelper.GetColorFromCommaRgb(pValue);
            return XmlHelper.GetColorFromHtmlCode(pValue);
        }

        private ColorF ConvertToColorF(float[] pFloatArray)
        {
            return new ColorF(
                pFloatArray[0] / 256,
                pFloatArray[1] / 256,
                pFloatArray[2] / 256,
                pFloatArray[3] / 256);
        }

        public Brush GetBrush(CarMp.Direct2D.RenderTargetWrapper pRenderWrapper)
        {
            if(pRenderWrapper.CurrentBounds.Height !=
                _bounds.Height || _textBrush == null)
            {
                _bounds = pRenderWrapper.CurrentBounds;
                // LinearGradient
                if (Color2 != null)
                {
                    _textBrush = pRenderWrapper.Renderer.CreateLinearGradientBrush(
                            new LinearGradientBrushProperties()
                            {
                                StartPoint = new Point2F(0, 0),
                                EndPoint = new Point2F(0, pRenderWrapper.CurrentBounds.Height)
                            },
                            pRenderWrapper.Renderer.CreateGradientStopCollection(new GradientStop[] {
                            new GradientStop
                                {
                                    Color = ConvertToColorF(Color1),
                                    Position = 0
                                }
                                ,
                            new GradientStop
                                {
                                    Color = ConvertToColorF(Color2),
                                    Position = 1
                                }
                            },
                                Gamma.Gamma_10,
                                ExtendMode.Clamp
                        ));
                    return _textBrush;
                }
                else
                {
                    _textBrush = pRenderWrapper.Renderer.CreateSolidColorBrush(ConvertToColorF(Color1));
                }
            }
            return _textBrush;
        }


        public void Initialize(DWriteFactory pWriteFactory)
        {
            Format = pWriteFactory.CreateTextFormat(
                    Face,
                    Size,
                    FontWeight.Normal,
                    FontStyle.Normal,
                    FontStretch.Normal,
                    new System.Globalization.CultureInfo("en-us"));

            Format.TextAlignment = Alignment;
            Format.WordWrapping = WordWrap ? WordWrapping.Wrap : WordWrapping.NoWrap;

        }

        public void Dispose()
        {
            Format.Dispose();
            Format = null;
        }
    }
}
