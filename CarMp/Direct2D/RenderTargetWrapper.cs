using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;

namespace CarMP.Direct2D
{

    /// <summary>
    /// Allows child ViewControls to paint at coordinates relative to their
    /// parent
    /// </summary>
    public class RenderTargetWrapper
    {
        public delegate void WindowResizeHandler(Size Size);
        public RenderTarget Renderer { get; private set; }
        public RectF CurrentBounds { get; set; }

        public void Resize(Size pSize)
        {
            if (Renderer is HwndRenderTarget)
                (Renderer as HwndRenderTarget).Resize(pSize);
        }
        public void EndDraw() { Renderer.EndDraw(); }
        public void BeginDraw() { Renderer.BeginDraw(); }
        public void Clear(ColorF pColor) { Renderer.Clear(pColor); }
        public Matrix3x2F Transform { get { return Renderer.Transform; } set { Renderer.Transform = value; } }
        public bool IsOccluded
        {
            get
            {
                return
                    (Renderer is HwndRenderTarget) ?
                    (Renderer as HwndRenderTarget).IsOccluded : true;
            }
        }
        public RenderTargetWrapper(RenderTarget pRenderer)
        {
            Renderer = pRenderer;
        }
        public void DrawRectangle(Brush pBrush, RectF pRectangle, float pStrokeWidth)
        {
            Renderer.DrawRectangle(TransformRectangle(pRectangle), pBrush, pStrokeWidth);
        }
        public void DrawLine(Point pPoint1, Point pPoint2, Brush pBrush, float pStrokeWidth)
        {
            Renderer.DrawLine(TransformPoint(pPoint1), TransformPoint(pPoint2), pBrush, pStrokeWidth);
        }
        public void FillRectangle(Brush pBrush, RectF pRectangle)
        {
            Renderer.FillRectangle(TransformRectangle(pRectangle), pBrush);
        }

        public void DrawBitmap(D2DBitmap pBitmap, RectF pRectangle)
        {
            Renderer.DrawBitmap(pBitmap, 1f, BitmapInterpolationMode.Linear, TransformRectangle(pRectangle));
        }

        public void DrawEllipse(Ellipse pEllipse, Brush pBrush, float pStrokeWidth)
        {
            Renderer.DrawEllipse(TransformEllipse(pEllipse), pBrush, pStrokeWidth);
        }

        public void FillEllipse(Ellipse pEllipse, Brush pBrush)
        {
            Renderer.FillEllipse(TransformEllipse(pEllipse), pBrush);
        }

        public void DrawTextLayout(Point pPoint, TextLayout pTextLayout, Brush pBrush)
        {
            if (pBrush is LinearGradientBrush)
            {
                LinearGradientBrush brush = pBrush as LinearGradientBrush;
                brush.EndPoint = TransformPoint(brush.EndPoint);
                brush.StartPoint = TransformPoint(brush.StartPoint);
            }
            Renderer.DrawTextLayout(TransformPoint(pPoint), pTextLayout, pBrush);
        }
        private Point2U TransformPoint(Point2U pPoint)
        {
            return new Point2U(Convert.ToUInt32(pPoint.X + CurrentBounds.Left), Convert.ToUInt32(pPoint.Y + CurrentBounds.Top));
        }

        private Point TransformPoint(Point pPoint)
        {
            return new Point(pPoint.X + CurrentBounds.Left, pPoint.Y + CurrentBounds.Top);
        }
        private RectU TransformRectangle(RectU pRectangle)
        {
            return new RectU(Convert.ToUInt32(pRectangle.Left + CurrentBounds.Left), Convert.ToUInt32(pRectangle.Top + CurrentBounds.Top), pRectangle.Width, pRectangle.Height);
        }
        private RectF TransformRectangle(RectF pRectangle)
        {
            float left = pRectangle.Left + CurrentBounds.Left;
            float top = pRectangle.Top + CurrentBounds.Top;
            return new RectF(left, top, left + pRectangle.Width, top + pRectangle.Height);
        }
        private Ellipse TransformEllipse(Ellipse pEllipse)
        {
            return new Ellipse(TransformPoint(pEllipse.Point), pEllipse.RadiusX, pEllipse.RadiusY);
        }
    }
}
