using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using CarMP.Graphics.Geometry;
using CarMP.Graphics.Interfaces;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;

namespace CarMP.Graphics.Implementation.Direct2D
{
    public class Direct2DRenderer : IRenderer
    {
        public delegate void WindowResizeHandler(Size Size);
        private RenderTarget _renderer;
        public Rectangle CurrentBounds { get; set; }

        public void Resize(Size pSize)
        {
            if (_renderer is HwndRenderTarget)
                (_renderer as HwndRenderTarget).Resize(new SizeU((uint)pSize.Width, (uint)pSize.Height));
        }
        public void EndDraw() { _renderer.EndDraw(); }
        public void BeginDraw() { _renderer.BeginDraw(); }
        public void Clear(Color pColor) { _renderer.Clear(new ColorF(pColor.Red, pColor.Green, pColor.Blue, pColor.Alpha)); }
        public Matrix3x2F Transform { get { return _renderer.Transform; } set { _renderer.Transform = value; } }
        public bool IsOccluded
        {
            get
            {
                return
                    (_renderer is HwndRenderTarget) ?
                    (_renderer as HwndRenderTarget).IsOccluded : true;
            }
        }
        public Direct2DRenderer(RenderTarget pRenderTarget)
        {
            _renderer = pRenderTarget;
        }
        public void DrawRectangle(IBrush pBrush, Rectangle pRectangle, float pStrokeWidth)
        {
            _renderer.DrawRectangle(TransformRectangle(pRectangle), GetBrush(pBrush), pStrokeWidth);
        }
        public void DrawLine(Point pPoint1, Point pPoint2, IBrush pBrush, float pStrokeWidth)
        {
            _renderer.DrawLine(TransformPoint(pPoint1), TransformPoint(pPoint2), GetBrush(pBrush), pStrokeWidth);
        }
        public void FillRectangle(IBrush pBrush, Rectangle pRectangle)
        {
            _renderer.FillRectangle(TransformRectangle(pRectangle), GetBrush(pBrush));
        }

        public void DrawBitmap(D2DBitmap pBitmap, Rectangle pRectangle)
        {
            _renderer.DrawBitmap(pBitmap, 1f, BitmapInterpolationMode.Linear, TransformRectangle(pRectangle));
        }

        public void DrawImage(Rectangle pRectangle, IImage pImage, float pAlpha)
        {
            _renderer.DrawBitmap(GetBitmap(pImage), pAlpha, BitmapInterpolationMode.Linear, TransformRectangle(pRectangle));
        }

        public void DrawEllipse(Geometry.Ellipse pEllipse, IBrush pBrush, float pStrokeWidth)
        {
            _renderer.DrawEllipse(TransformEllipse(pEllipse), GetBrush(pBrush), pStrokeWidth);
        }

        public void FillEllipse(Geometry.Ellipse pEllipse, IBrush pBrush)
        {
            _renderer.FillEllipse(TransformEllipse(pEllipse), GetBrush(pBrush));
        }

        public void DrawString(Point pPoint, IStringLayout pStringLayout, IBrush pBrush)
        {
            _renderer.DrawTextLayout(TransformPoint(pPoint), GetTextLayout(pStringLayout), GetBrush(pBrush));
        }

        public void DrawTextLayout(Point pPoint, TextLayout pTextLayout, Brush pBrush)
        {
            //if (pBrush is LinearGradientBrush)
            //{
            //    LinearGradientBrush brush = pBrush as LinearGradientBrush;
            //    brush.EndPoint = TransformPoint(brush.EndPoint);
            //    brush.StartPoint = TransformPoint(brush.StartPoint);
            //}
            _renderer.DrawTextLayout(TransformPoint(pPoint), pTextLayout, pBrush);
        }
        private Point2U TransformPoint(Point2U pPoint)
        {
            return new Point2U(Convert.ToUInt32(pPoint.X + CurrentBounds.Left), Convert.ToUInt32(pPoint.Y + CurrentBounds.Top));
        }

        private Point2F TransformPoint(Point pPoint)
        {
            return new Point2F(pPoint.X + CurrentBounds.Left, pPoint.Y + CurrentBounds.Top);
        }
        private RectF TransformRectangle(Rectangle pRectangle)
        {
            float left = pRectangle.Left + CurrentBounds.Left;
            float top = pRectangle.Top + CurrentBounds.Top;
            return new RectF(left, top, left + pRectangle.Width, top + pRectangle.Height);
        }
        
        private Microsoft.WindowsAPICodePack.DirectX.Direct2D1.Ellipse TransformEllipse(Geometry.Ellipse pEllipse)
        {
            return new Microsoft.WindowsAPICodePack.DirectX.Direct2D1.Ellipse(TransformPoint(pEllipse.Point), pEllipse.RadiusX, pEllipse.RadiusY);
        }

        private Brush GetBrush(IBrush pBrush)
        {
            return (pBrush as D2DBrush).BrushResource;
        }

        private TextLayout GetTextLayout(IStringLayout pStringLayout)
        {
            return null;
        }

        private D2DBitmap GetBitmap(IImage pImage)
        {
            return (pImage as D2DImage).ImageResource;
        }

        public IBrush CreateBrush(Color pColor)
        {
            return new D2DBrush(_renderer, pColor);
        }

        public IImage CreateImage(string pPath)
        {
            return new D2DImage(_renderer, pPath);
        }

        public IImage CreateImage(byte[] pData, int pStride)
        {
            return new D2DImage(_renderer, pData, pStride);
        }

        public IStringLayout CreateStringLayout()
        {
            return new D2DStringLayout(_renderer);
        }
    }
}