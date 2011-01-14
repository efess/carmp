using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using System.Xml;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;


namespace CarMP.ViewControls
{
    public class DragableListImageAndCaptionItem : DragableListItem
    {
        private IImage _imageBitmap;
        private string _imageBitmapPath;

        private TextStyle _textStyle;

        private IStringLayout _stringLayout = null;
        private IBrush _stringBrush = null;

        public DragableListImageAndCaptionItem(string pTextString, string pBitmapPath)
        {
            DisplayString = pTextString;
            _imageBitmapPath = pBitmapPath;

            _textStyle = new TextStyle(20F,
                "Arial",
                null,
                null,
                false,
                Graphics.StringAlignment.Left);

            //if (StringDrawFormat == null)
            //{
            //    StringDrawFormat = D2DStatic.StringFactory.CreateTextFormat(
            //        "Arial",
            //        20F,
            //        FontWeight.Normal,
            //        FontStyle.Normal,
            //        FontStretch.Normal,
            //        new System.Globalization.CultureInfo("en-us"));

            //    StringDrawFormat.TextAlignment = TextAlignment.Leading;
            //    StringDrawFormat.WordWrapping = WordWrapping.NoWrap;
            //}
        }

        /// <summary>
        /// String shown to the user
        /// </summary>
        public string DisplayString { get; private set; }

        protected override void OnRender(IRenderer pRenderer)
        {
            if (_stringLayout == null)
                _stringLayout = pRenderer.CreateStringLayout(_textStyle.Face, _textStyle.Size);

            if (_imageBitmap == null)
                _imageBitmap = pRenderer.CreateImage(_imageBitmapPath);

            if (_stringBrush == null)
                _stringBrush = pRenderer.CreateBrush(Color.Gray);
            // TODO: Gradients ? IGradientBrush? Or bitmaps?
            //if (LinearGradient == null)
            //{
            //    if (LinearGradient == null)
            //        LinearGradient = pRenderer.Renderer.CreateLinearGradientBrush(
            //                new LinearGradientBrushProperties()
            //                {
            //                    StartPoint = new Point(0, 0),
            //                    EndPoint = new Point(0, _bounds.Height)
            //                },
            //                pRenderer.Renderer.CreateGradientStopCollection(new GradientStop[] {
            //                    new GradientStop
            //                        {
            //                            Color = new ColorF(Colors.Gray, 1),
            //                            Position = 0
            //                        }
            //                        ,
            //                    new GradientStop
            //                        {
            //                            Color = new ColorF(Colors.White, 1),
            //                            Position = 1
            //                        }
            //                    },
            //                    Gamma.Gamma_10,
            //                    ExtendMode.Clamp
            //            ));

            //}

            //LinearGradient.EndPoint = new Point(0, 0);
            //LinearGradient.StartPoint = new Point(0, _bounds.Height);

            pRenderer.DrawString(new Point(4, 0), _stringLayout, _stringBrush);

            // Call base which will draw the selection is selected
            base.OnRender(pRenderer);
        }
    }
}
