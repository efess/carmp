using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Direct2D;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMP.ViewControls
{
    public class SingleOptionRadio : Text
    {
        private Brush _outlineBrush;
        private Point2F _centerPoint;

        public bool Checked { get; set; }

        public SingleOptionRadio()
        {
            // Defaults
            this.Bounds = new RectF(0, 0, 150, 25);
            this.TextStyle = new TextStyle(_bounds.Height - 3, "Arial", new[] { 255.0f, 255.0f, 255.0f, 255.0f }, null, true, Microsoft.WindowsAPICodePack.DirectX.DirectWrite.TextAlignment.Leading);
        }

        public override void OnSizeChanged(object sender, EventArgs e)
        {
            SetSize();
        }

        protected override void OnRender(RenderTargetWrapper pRenderTarget)
        {
            if(_outlineBrush == null)
                _outlineBrush = pRenderTarget.Renderer.CreateSolidColorBrush(new ColorF(Colors.White));

            float stroke = _bounds.Height / 8;
            float outerWidth = _bounds.Height / 2 - stroke;
            float innerWidth = outerWidth - stroke - 1;

            pRenderTarget.DrawEllipse(new Ellipse(_centerPoint, outerWidth, outerWidth), _outlineBrush, stroke);
            if (Checked) 
                pRenderTarget.FillEllipse(new Ellipse(_centerPoint, innerWidth, innerWidth), _outlineBrush);
            
            base.OnRender(pRenderTarget);
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
            this.TextPosition = new Point2F(_bounds.Height + 3, 1);
            _centerPoint = new Point2F(_bounds.Height / 2, _bounds.Height / 2);
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
