using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics;

namespace CarMP.ViewControls
{
    public class CheckBox : Text
    {
        private IBrush _outlineBrush;
        private Point _centerPoint;

        public bool Checked { get; set; }

        public CheckBox()
        {
            // Defaults
            this.Bounds = new Rectangle(0, 0, 150, 25);
            this.TextStyle = new TextStyle(_bounds.Height - 3, "Arial", Color.White, null, true, StringAlignment.Left);
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            SetSize();
        }

        protected override void OnRender(IRenderer pRenderer)
        {
            if(_outlineBrush == null)
                _outlineBrush = pRenderer.CreateBrush(Color.White);

            float stroke = _bounds.Height / 8;
            Rectangle boxBorder = new Rectangle(stroke, stroke, stroke * 2, stroke * 2);

            var firstPoint = new Point(stroke + 3, _bounds.Height / 2 - 2);
            var secondPoint = new Point(_bounds.Height / 2 - 4, _bounds.Height - stroke - 6);
            var thirdPoint = new Point(_bounds.Height - stroke - 6,  stroke + 3);

            
            pRenderer.DrawRectangle(_outlineBrush, boxBorder, stroke);
            if (Checked)
            {
                pRenderer.DrawLine(firstPoint, secondPoint, _outlineBrush, stroke);
                pRenderer.DrawLine(secondPoint, thirdPoint, _outlineBrush, stroke);
            }

            base.OnRender(pRenderer);
        }

        protected override void OnTouchGesture(Reactive.Touch.TouchGesture pTouchGesture)
        {
            switch (pTouchGesture.Gesture)
            {
                case Reactive.Touch.GestureType.Click:
                    ControlClicked();
                    break;
            }
        }

        private void SetSize()
        {
            this.TextPosition = new Point(_bounds.Height + 3, 1);
            _centerPoint = new Point(_bounds.Height / 2, _bounds.Height / 2);
        }

        private void ControlClicked()
        {
            Checked = !Checked;
        }
    }
}
