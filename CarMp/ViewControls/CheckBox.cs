using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Direct2D;
using Microsoft.WindowsAPICodePack.DirectX;

namespace CarMP.ViewControls
{
    public class CheckBox : Text
    {
        private Brush _outlineBrush;
        private Point2F _centerPoint;

        public bool Checked { get; set; }

        public CheckBox()
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
            RectF boxBorder = new RectF(stroke, stroke, this.Height - stroke * 2, this.Height - stroke * 2);

            var firstPoint = new Point2F(stroke + 3, _bounds.Height / 2 - 2);
            var secondPoint = new Point2F(_bounds.Height / 2 - 4, _bounds.Height - stroke - 6);
            var thirdPoint = new Point2F(_bounds.Height - stroke - 6,  stroke + 3);

            
            pRenderTarget.DrawRectangle(_outlineBrush, boxBorder, stroke);
            if (Checked)
            {
                pRenderTarget.DrawLine(firstPoint, secondPoint, _outlineBrush, stroke);
                pRenderTarget.DrawLine(secondPoint, thirdPoint, _outlineBrush, stroke);
            }

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
            Checked = !Checked;
        }
    }
}
