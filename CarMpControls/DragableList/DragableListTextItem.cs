using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;


// Don't compile wiht these namespaces if we're not using them.
#if USE_DIRECT2D

using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Windows;
#endif

namespace CarMpControls
{
    public class DragableListTextItem : DragableListItem
    {
#if USE_DIRECT2D
        private SlimDX.DirectWrite.TextLayout StringLayout = null;
        private SlimDX.Direct2D.LinearGradientBrush LinearGradient = null;

        private static SlimDX.DirectWrite.Factory StringFactory = null;
        private static SlimDX.DirectWrite.TextFormat StringDrawFormat = null;
#endif
        public DragableListTextItem()
        {
#if USE_DIRECT2D
            
            if (StringFactory == null)
                StringFactory = new SlimDX.DirectWrite.Factory();

            if (StringDrawFormat == null)
                StringDrawFormat = new SlimDX.DirectWrite.TextFormat(
                    StringFactory,
                    "Arial",
                    SlimDX.DirectWrite.FontWeight.Normal,
                    SlimDX.DirectWrite.FontStyle.Normal,
                    SlimDX.DirectWrite.FontStretch.Normal,
                    20F,
                    "en-us") { TextAlignment = SlimDX.DirectWrite.TextAlignment.Leading };

            

#endif
        }

        /// <summary>
        /// String shown to the user
        /// </summary>
        public string DisplayString { get; set; }
#if USE_DIRECT2D

        internal override void DrawItem(WindowRenderTarget pRenderer, RectangleF pRectangle)
        {
            if(StringLayout == null)
            {
                StringLayout = new SlimDX.DirectWrite.TextLayout(StringFactory, DisplayString, StringDrawFormat, pRectangle.Width,pRectangle.Height);
            }

            if (LinearGradient == null)
            {
                LinearGradient = new SlimDX.Direct2D.LinearGradientBrush(pRenderer,
                    new GradientStopCollection(pRenderer, new GradientStop[] {
                    new GradientStop
                        {
                            Color = Color.Gray,
                            Position = 0
                        }
                        ,
                    new GradientStop
                        {
                            Color = Color.White,
                            Position = 1
                        }
                    }),
                        new LinearGradientBrushProperties()
                        {
                            StartPoint = new PointF(0, pRectangle.Top),
                            EndPoint = new PointF(0, pRectangle.Bottom)
                        }
                    );
            }

            LinearGradient.EndPoint = new PointF(0, pRectangle.Bottom);
            LinearGradient.StartPoint = new PointF(0, pRectangle.Top);

            pRenderer.DrawTextLayout(new PointF(pRectangle.Location.X + 4,pRectangle.Location.Y), StringLayout, LinearGradient);
            
            // Call base which will draw the selection is selected
            base.DrawItem(pRenderer, pRectangle);
        }
#else
        public override void DrawItem(System.Drawing.Graphics pCanvas)
        {
            //pCanvas.FillRectangle(Color.Gray,new Rectangle(0,0
            pCanvas.DrawString(
                DisplayString,
                new Font("Arial", 15F),
                new LinearGradientBrush(new Point(0, 0), new Point(0, ClientSize.Height), Color.White, Color.Gray),
                new PointF(1, 1)
                );

            pCanvas.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }
#endif

    }
}
