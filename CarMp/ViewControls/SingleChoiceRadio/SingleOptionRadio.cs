using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls
{
    public class SingleOptionRadio : Text
    {
        private IBrush _outlineBrush;
        private Point _centerPoint;

        public bool Checked { get; set; }

        public SingleOptionRadio()
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
            float outerWidth = _bounds.Height / 2 - stroke;
            float innerWidth = outerWidth - stroke - 1;

            pRenderer.DrawEllipse(new Ellipse(_centerPoint, outerWidth, outerWidth),  _outlineBrush, stroke);
            if (Checked) 
                pRenderer.FillEllipse(new Ellipse(_centerPoint, innerWidth, innerWidth), _outlineBrush);
            
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
            foreach (var singleOptionRadio in Parent.ViewControls.OfType<SingleOptionRadio>()
                .Where(vc => vc != this && vc.Checked))
                singleOptionRadio.Checked = false;

            if (!Checked)
                Checked = true;
        }
    }
}
