using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.DirectWrite;
using Microsoft.WindowsAPICodePack.DirectX;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;
using Microsoft.WindowsAPICodePack.DirectX.WindowsImagingComponent;
using System.IO;
using CarMP.Direct2D;
using System;

namespace CarMP
{
    public static class D2DStatic
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

        public static SizeF GetTextPixelSize(string pText, TextStyle pTextStyle)
        {
            if(pTextStyle.Format == null)
                pTextStyle.Initialize(D2DStatic.StringFactory);
            using (TextLayout layout = StringFactory.CreateTextLayout(pText, pTextStyle.Format, 9999, 9999))
            {
                return new SizeF(layout.Metrics.Width, layout.Metrics.Height);
            }
        }

        public static LinearGradientBrush GetBasicLinearGradient(
            RenderTarget pRenderer,
            RectF pBounds,
            ColorF pColor1,
            ColorF pColor2)
        {
            return pRenderer.CreateLinearGradientBrush(
                    new LinearGradientBrushProperties()
                    {
                        StartPoint = new Point2F(0, 0),
                        EndPoint = new Point2F(0, pBounds.Height)
                    },
                    pRenderer.CreateGradientStopCollection(new GradientStop[] {
                        new GradientStop
                            {
                                Color = pColor1,
                                Position = 0
                            }
                            ,
                        new GradientStop
                            {
                                Color = pColor2,
                                Position = 1
                            }
                        },
                        Gamma.Gamma_10,
                        ExtendMode.Clamp
                ));
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
            catch (Exception)
            {
                return null;
            }
        }


    }
}
