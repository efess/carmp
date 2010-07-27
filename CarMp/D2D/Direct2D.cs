using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;
using Microsoft.WindowsAPICodePack.DirectX.WindowsImagingComponent;
using System.IO;

namespace CarMp
{
    public static class Direct2D
    {
        // Factories
        private static DWriteFactory _stringFactory;
        public static DWriteFactory StringFactory
        {
            get
            {
                if (_stringFactory == null)
                    _stringFactory = DWriteFactory.CreateFactory();
                return _stringFactory;
            }
        }

        private static D2DFactory _factory;
        public static D2DFactory D2DFactory
        {
            get
            {
                if(_factory == null)
                    _factory = D2DFactory.CreateFactory(D2DFactoryType.MultiThreaded);
                return _factory;
            }
        }
        private static ImagingFactory _imagingFactory;
        public static ImagingFactory ImagingFactory
        {
            get
            {
                if (_imagingFactory == null)
                    _imagingFactory = new ImagingFactory();
                return _imagingFactory;
            }
        }

        public static ColorF ConvertToColorF(float[] pFloatArray)
        {
            return new ColorF(
                pFloatArray[0] / 256,
                pFloatArray[1] / 256,
                pFloatArray[2] / 256,
                pFloatArray[3] / 256);
        }
        public static D2DBitmap GetBitmap(BitmapData pBitmapData, RenderTarget pRenderTarget)
        {
            try
            {
                BitmapDecoder decoder = ImagingFactory.CreateDecoderFromFilename(pBitmapData.FilePath, DesiredAccess.Read, DecodeMetadataCacheOptions.OnDemand);//.CreateDecoderFromStream(pBitmapData.Data, DecodeMetadataCacheOptions.OnDemand);

                BitmapFrameDecode frameDeocder = decoder.GetFrame(0);
                WICFormatConverter formatConverter = ImagingFactory.CreateFormatConverter();
                BitmapSource src = frameDeocder.ToBitmapSource();
                formatConverter.Initialize(frameDeocder.ToBitmapSource(), PixelFormats.Pf32bppPBGRA, BitmapDitherType.None, BitmapPaletteType.MedianCut);

                return pRenderTarget.CreateBitmapFromWicBitmap(formatConverter.ToBitmapSource());
               
            }
             catch (Exception ex)
            {
                return null;
            }
        }

        public struct BitmapData
        {
            public int Width { get; private set; }
            public int Height { get; private set; }
            public int Stride { get; private set; }
            public string FilePath { get; private set; }
            public BitmapProperties BitmapProperties { get; private set; }
            public Stream Data { get; private set; }

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
                            Format.B8G8R8A8_UNORM, 
                            AlphaMode.Premultiplied)
                    };

                    FilePath = pBmpFilePath;
                    Width = bitmap.Width;
                    Height = bitmap.Height;

                    // Declare an array to hold the bytes of the bitmap.
                    IntPtr ptr = bmpData.Scan0;

                    byte[] bytes = new byte[bmpData.Stride * bitmap.Height];

                    // Copy the RGB values into the array.
                    System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, bytes.Length);
                    Data = new MemoryStream();
                    Data.Write(bytes, 0, bytes.Length);
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
            public delegate void WindowResizeHandler(SizeU Size);
            public HwndRenderTarget Renderer { get; private set; }
            public RectF CurrentBounds { get; set; }

            public void Resize(SizeU pSize) { Renderer.Resize(pSize); }
            public void EndDraw() { Renderer.EndDraw(); }
            public void BeginDraw() { Renderer.BeginDraw(); }
            public void Clear(ColorF pColor) { Renderer.Clear(pColor); }
            public Matrix3x2F Transform { get { return Renderer.Transform; } set { Renderer.Transform = value; } }
            public bool IsOccluded { get { return Renderer.IsOccluded; } }
            public RenderTargetWrapper(HwndRenderTarget pRenderTarget)
            {
                Renderer = pRenderTarget;
            }
            public void DrawRectangle(Brush pBrush, RectF pRectangle, float pStrokeWidth)
            {
                Renderer.DrawRectangle(TransformRectangle(pRectangle), pBrush, pStrokeWidth);
            }
            public void FillRectangle(Brush pBrush, RectF pRectangle)
            {
                Renderer.FillRectangle(TransformRectangle(pRectangle), pBrush);
            }
            
            public void DrawBitmap(D2DBitmap pBitmap, RectF pRectangle)
            {
                Renderer.DrawBitmap(pBitmap, 1f, BitmapInterpolationMode.Linear, TransformRectangle(pRectangle));
            }

            public void DrawTextLayout(Point2F pPoint, TextLayout pTextLayout, Brush pBrush)
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

            private Point2F TransformPoint(Point2F pPoint)
            {
                return new Point2F(pPoint.X + CurrentBounds.Left, pPoint.Y + CurrentBounds.Top);
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
        }
    }
}
