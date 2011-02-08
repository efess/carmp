using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;
using System.Xml;


namespace CarMP.ViewControls
{
    public class DragableListTextItem : DragableListItem
    {
        private IStringLayout _stringLayout = null;
        private IBrush _linearGradient = null;

        private static TextStyle _textStyle = null;

        public DragableListTextItem(string pTextString)
        {
            DisplayString = pTextString;

            // Initialize _textStyle
            if (_textStyle == null)
            {
                _textStyle = new TextStyle(
                    20F,
                    "Arial",
                    Color.LightGray,
                    null,
                    false,
                    StringAlignment.Left);

                //StringDrawFormat = D2DStatic.StringFactory.CreateTextFormat(
                //    "Arial",
                //    20F,
                //    FontWeight.Normal,
                //    FontStyle.Normal,
                //    FontStretch.Normal, 
                //    new System.Globalization.CultureInfo("en-us")) ;
            
                //StringDrawFormat.TextAlignment = TextAlignment.Leading;
                //StringDrawFormat.WordWrapping = WordWrapping.NoWrap;
            }
        }

        private void InitializeStyle()
        {
        }

        public string _displayString;
        /// <summary>
        /// String shown to the user
        /// </summary>
        public string DisplayString 
        {
            get { return _displayString; }
            set { _displayString = value; _stringLayout = null; }
        }


        protected override void OnRender(IRenderer pRenderer)
        {
            if(_stringLayout == null)
            {
                _stringLayout = pRenderer.CreateStringLayout(_displayString, _textStyle.Face, _textStyle.Size);
                
                //_stringLayout = D2DStatic.StringFactory.CreateTextLayout(DisplayString, StringDrawFormat, _bounds.Width, _bounds.Height);
            }

            if (_linearGradient == null)
            {
                _linearGradient = pRenderer.CreateBrush(_textStyle.Color1);
            }
            // TODO: Gradients?

            //if (_linearGradient == null)
            //{
            //    if (_linearGradient == null)
            //        _linearGradient = pRenderer.Renderer.CreateLinearGradientBrush(
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

            //_linearGradient.EndPoint = new Point(0, 0);
            //_linearGradient.StartPoint = new Point(0, _bounds.Height);

            pRenderer.DrawString(new Point(4, 0), _stringLayout, _linearGradient);
            
            // Call base which will draw the selection is selected
            base.OnRender(pRenderer);
        }
    }
}
