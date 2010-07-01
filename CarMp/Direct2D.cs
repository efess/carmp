using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX.DirectWrite;
using SlimDX.Direct2D;
using System.Drawing;

namespace CarMp
{
    public static class Direct2D
    {
        private static SlimDX.DirectWrite.Factory _stringFactory;
        public static SlimDX.DirectWrite.Factory StringFactory
        {
            get
            {
                if (_stringFactory == null)
                    _stringFactory = new SlimDX.DirectWrite.Factory();
                return _stringFactory;
            }
        }

        public static SlimDX.Direct2D.Bitmap GetBitmap(BitmapData pBitmapData, RenderTarget pRenderTarget)
        {
            return new SlimDX.Direct2D.Bitmap(
                pRenderTarget,
                new System.Drawing.Size(pBitmapData.Width, pBitmapData.Height),
                new SlimDX.DataStream(pBitmapData.Data, true, false),
                pBitmapData.Stride,
                pBitmapData.BitmapProperties);
        }

        public struct BitmapData
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Stride { get; private set; }
            public BitmapProperties BitmapProperties { get; private set; }
            public byte[] Data { get; private set; }

            public BitmapData(string pBmpFilePath) : this()
            {
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(pBmpFilePath))
                {
                    System.Drawing.Imaging.BitmapData bmpData =
                        bitmap.LockBits(
                            new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                            System.Drawing.Imaging.ImageLockMode.WriteOnly,
                            bitmap.PixelFormat
                    );

                    BitmapProperties = new BitmapProperties
                    {
                        PixelFormat =  new PixelFormat(
                            SlimDX.DXGI.Format.B8G8R8A8_UNorm, 
                            SlimDX.Direct2D.AlphaMode.Premultiplied)
                    };

                    Width = bitmap.Width;
                    Height = bitmap.Height;

                    // Declare an array to hold the bytes of the bitmap.
                    IntPtr ptr = bmpData.Scan0;

                    int bytes = bmpData.Stride * bitmap.Height;
                    Data = new byte[bytes];

                    // Copy the RGB values into the array.
                    System.Runtime.InteropServices.Marshal.Copy(ptr, Data, 0, bytes);
                    Stride = bmpData.Stride;

                    // Unlock the bits.
                    bitmap.UnlockBits(bmpData);
                }
            }
        }

        /// <summary>
        /// Allows child ViewControls to paint at coordinates relative to their
        /// parent
        /// </summary>
        public class RenderTargetWrapper
        {
            public delegate void WindowResizeHandler(Size Size);
            public WindowRenderTarget Renderer { get; private set; }
            public RectangleF CurrentBounds { get; set; }

            public void Resize(Size pSize) { Renderer.Resize(pSize); }
            public void EndDraw() { Renderer.EndDraw(); }
            public void BeginDraw() { Renderer.BeginDraw(); }
            public void Clear(SlimDX.Color4 pColor) { Renderer.Clear(pColor); }
            public Matrix3x2 Transform { get { return Renderer.Transform; } set { Renderer.Transform = value; } }
            public bool IsOccluded { get { return Renderer.IsOccluded; } }
            public RenderTargetWrapper(WindowRenderTarget pRenderTarget)
            {
                Renderer = pRenderTarget;
            }
            public void DrawRectangle(SlimDX.Direct2D.Brush pBrush, RectangleF pRectangle)
            {
                Renderer.DrawRectangle(pBrush, TransformRectangle(pRectangle));
            }
            public void DrawRectangle(SlimDX.Direct2D.Brush pBrush, Rectangle pRectangle)
            {
                Renderer.DrawRectangle(pBrush, TransformRectangle(pRectangle));
            }
            public void DrawRectangle(SlimDX.Direct2D.Brush pBrush, RectangleF pRectangle, float pStrokeWidth)
            {
                Renderer.DrawRectangle(pBrush, TransformRectangle(pRectangle), pStrokeWidth);
            }
            public void DrawRectangle(SlimDX.Direct2D.Brush pBrush, Rectangle pRectangle, float pStrokeWidth)
            {
                Renderer.DrawRectangle(pBrush, TransformRectangle(pRectangle), pStrokeWidth);
            }
            public void FillRectangle(SlimDX.Direct2D.Brush pBrush, RectangleF pRectangle)
            {
                Renderer.FillRectangle(pBrush, TransformRectangle(pRectangle));
            }
            public void FillRectangle(SlimDX.Direct2D.Brush pBrush, Rectangle pRectangle)
            {
                Renderer.FillRectangle(pBrush, TransformRectangle(pRectangle));
            }

            public void DrawBitmap(SlimDX.Direct2D.Bitmap pBitmap, Rectangle pRectangle)
            {
                Renderer.DrawBitmap(pBitmap, TransformRectangle(pRectangle));
            }

            public void DrawBitmap(SlimDX.Direct2D.Bitmap pBitmap, RectangleF pRectangle)
            {
                Renderer.DrawBitmap(pBitmap, TransformRectangle(pRectangle));
            }

            public void DrawTextLayout(PointF pPoint, SlimDX.DirectWrite.TextLayout pTextLayout, SlimDX.Direct2D.Brush pBrush)
            {
                if (pBrush is LinearGradientBrush)
                {
                    LinearGradientBrush brush = pBrush as LinearGradientBrush;
                    brush.EndPoint = TransformPoint(brush.EndPoint);
                    brush.StartPoint = TransformPoint(brush.StartPoint);
                }
                Renderer.DrawTextLayout(TransformPoint(pPoint), pTextLayout, pBrush);
            }
            public void DrawTextLayout(Point pPoint, SlimDX.DirectWrite.TextLayout pTextLayout, SlimDX.Direct2D.Brush pBrush)
            {
                Renderer.DrawTextLayout(TransformPoint(pPoint), pTextLayout, pBrush);
            }
            private Point TransformPoint(Point pPoint)
            {
                return new Point(Convert.ToInt32(pPoint.X + CurrentBounds.X), Convert.ToInt32(pPoint.Y + CurrentBounds.Y));
            }

            private PointF TransformPoint(PointF pPoint)
            {
                return new PointF(pPoint.X + CurrentBounds.X, pPoint.Y + CurrentBounds.Y);
            }
            private Rectangle TransformRectangle(Rectangle pRectangle)
            {
                return new Rectangle(Convert.ToInt32(pRectangle.X + CurrentBounds.X), Convert.ToInt32(pRectangle.Y + CurrentBounds.Y), pRectangle.Width, pRectangle.Height);
            }

            private RectangleF TransformRectangle(RectangleF pRectangle)
            {
                return new RectangleF(pRectangle.X + CurrentBounds.X, pRectangle.Y + CurrentBounds.Y, pRectangle.Width, pRectangle.Height);
            }
        }
    }
}
