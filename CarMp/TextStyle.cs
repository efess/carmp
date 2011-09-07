using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics;
using CarMP.Helpers;

namespace CarMP
{
    public class TextStyle
    {
        private const string SIZE = "Size";
        private const string FACE = "Face";
        private const string COLOR_ONE = "ColorOne";
        private const string COLOR_TWO = "ColorTwo";
        private const string WORD_WRAP = "WordWrap";
        private const string ALIGNMENT = "Alignment";

        public float Size { get; private set; }
        public string Face { get; private set; }
        public Color? Color1 { get; private set; }
        public Color? Color2 { get; private set; }
        public bool WordWrap { get; private set; }

        public StringAlignment Alignment { get; private set;}

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
                        Color1 = GraphicsHelper.ConvertFloatArrayToColor(XmlHelper.ConvertColor(node.InnerText));
                        break;
                    case COLOR_TWO:
                        Color2 = GraphicsHelper.ConvertFloatArrayToColor(XmlHelper.ConvertColor(node.InnerText));
                        break;
                    case WORD_WRAP:
                        WordWrap = Convert.ToBoolean(node.InnerText);
                        break;
                    case ALIGNMENT:
                        Alignment = (StringAlignment)Enum.Parse(typeof(StringAlignment), node.InnerText);
                        break;
                }
            }
        }

        public TextStyle(float pSize, string pFace, Color? pColor1, Color? pColor2, bool pWordWrap, StringAlignment pAlignment)
        {
            Size = pSize;
            Face = pFace;
            Color1 = pColor1;
            Color2 = pColor2;
            WordWrap = pWordWrap;
            Alignment = pAlignment;
        }

        //public Brush GetBrush(CarMP.Direct2D.RenderTargetWrapper pRenderWrapper)
        //{
        //    if(pRenderWrapper.CurrentBounds.Height !=
        //        _bounds.Height || _textBrush == null)
        //    {
        //        _bounds = pRenderWrapper.CurrentBounds;
        //        // _linearGradient
        //        if (Color2 != null)
        //        {
        //            _textBrush = pRenderWrapper.Renderer.CreateLinearGradientBrush(
        //                    new LinearGradientBrushProperties()
        //                    {
        //                        StartPoint = new Point(0, 0),
        //                        EndPoint = new Point(0, pRenderWrapper.CurrentBounds.Height)
        //                    },
        //                    pRenderWrapper.Renderer.CreateGradientStopCollection(new GradientStop[] {
        //                    new GradientStop
        //                        {
        //                            Color = D2DStatic.ConvertToColorF(Color1),
        //                            Position = 0
        //                        }
        //                        ,
        //                    new GradientStop
        //                        {
        //                            Color = D2DStatic.ConvertToColorF(Color2),
        //                            Position = 1
        //                        }
        //                    },
        //                        Gamma.Gamma_10,
        //                        ExtendMode.Clamp
        //                ));
        //            return _textBrush;
        //        }
        //        else
        //        {
        //            _textBrush = pRenderWrapper.Renderer.CreateSolidColorBrush(D2DStatic.ConvertToColorF(Color1));
        //        }
        //    }
        //    return _textBrush;
        //}


        //public void Initialize(DWriteFactory pWriteFactory)
        //{
        //    Format = pWriteFactory.CreateTextFormat(
        //            Face,
        //            Size,
        //            FontWeight.Normal,
        //            FontStyle.Normal,
        //            FontStretch.Normal,
        //            new System.Globalization.CultureInfo("en-us"));

        //    Format.TextAlignment = Alignment;
        //    Format.WordWrapping = WordWrap ? WordWrapping.Wrap : WordWrapping.NoWrap;

        //}

        //public void Dispose()
        //{
        //    Format.Dispose();
        //    Format = null;
        //}
    }
}
