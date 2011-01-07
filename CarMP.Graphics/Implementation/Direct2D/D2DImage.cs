using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using Microsoft.WindowsAPICodePack.DirectX.WindowsImagingComponent;
using Microsoft.WindowsAPICodePack.DirectX.DXGI;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Implementation.Direct2D
{
    public class D2DImage : IImage
    {
        public Size Size { get; set;}

        private static ImagingFactory _imagingFactory;
        private static ImagingFactory ImagingFactory
        {
            get
            {
                if (_imagingFactory == null)
                    _imagingFactory = new ImagingFactory();
                return _imagingFactory;
            }
        }

        internal D2DBitmap ImageResource { get; set; }

        internal D2DImage(RenderTarget pRenderer, string pPath)
        {
            GetImageFromPath(pRenderer, pPath);
        }

        internal D2DImage(RenderTarget pRenderer, byte[] pData, int pStride)
        {
            throw new NotImplementedException("Not sure how to process from a stream/byte array yet.");
        }
        
        private void GetImageFromPath(RenderTarget pRenderer, string pPath)
        {
            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(pPath))
            {
                System.Drawing.Imaging.BitmapData bmpData =
                    bitmap.LockBits(
                        new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                        bitmap.PixelFormat
                );

                Size = new Size(bitmap.Width, bitmap.Height);

                //// Declare an array to hold the bytes of the bitmap.
                //IntPtr ptr = bmpData.Scan0;

                //byte[] bytes = new byte[bmpData.Stride * bitmap.Height];

                // Unlock the bits.
                bitmap.UnlockBits(bmpData);
            }
            
            BitmapDecoder decoder = ImagingFactory.CreateDecoderFromFilename(pPath, DesiredAccess.Read, DecodeMetadataCacheOptions.OnDemand);
            
            BitmapFrameDecode frameDeocder = decoder.GetFrame(0);
            WICFormatConverter formatConverter = ImagingFactory.CreateFormatConverter();
            BitmapSource src = frameDeocder.ToBitmapSource();
            formatConverter.Initialize(frameDeocder.ToBitmapSource(), PixelFormats.Pf32bppPBGRA, BitmapDitherType.None, BitmapPaletteType.MedianCut);

            ImageResource = pRenderer.CreateBitmapFromWicBitmap(formatConverter.ToBitmapSource());
        }

        private void CreateImageResource(RenderTarget pRenderer, byte[] pData, int pStride)
        {
            var properties = new BitmapProperties
            {
                PixelFormat = new PixelFormat(
                    Format.B8G8R8A8_UNORM,
                    AlphaMode.Premultiplied)
            };

            // Copy the RGB values into the array.
            //System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, bytes.Length);
            //Data = new MemoryStream();
            //Data.Write(bytes, 0, bytes.Length);
            //Stride = bmpData.Stride;
            
            
        }
    }
}
