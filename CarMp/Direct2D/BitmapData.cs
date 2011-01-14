//using CarMP.Graphics.Geometry;
//using System.IO;
//using System;
//using Microsoft.WindowsAPICodePack.DirectX.DXGI;

//namespace CarMP.Direct2D
//{
//    public struct BitmapData
//    {
//        public int Width { get; private set; }
//        public int Height { get; private set; }
//        public int Stride { get; private set; }
//        public string FilePath { get; private set; }
//        public BitmapProperties BitmapProperties { get; private set; }
//        public Stream Data { get; private set; }

//        public BitmapData(string pBmpFilePath)
//            : this()
//        {
//            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(pBmpFilePath))
//            {
//                System.Drawing.Imaging.BitmapData bmpData =
//                    bitmap.LockBits(
//                        new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
//                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
//                        bitmap.PixelFormat
//                );

//                BitmapProperties = new BitmapProperties
//                {
//                    PixelFormat = new PixelFormat(
//                        Format.B8G8R8A8_UNORM,
//                        AlphaMode.Premultiplied)
//                };

//                FilePath = pBmpFilePath;
//                Width = bitmap.Width;
//                Height = bitmap.Height;

//                // Declare an array to hold the bytes of the bitmap.
//                IntPtr ptr = bmpData.Scan0;

//                byte[] bytes = new byte[bmpData.Stride * bitmap.Height];

//                // Copy the RGB values into the array.
//                System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, bytes.Length);
//                Data = new MemoryStream();
//                Data.Write(bytes, 0, bytes.Length);
//                Stride = bmpData.Stride;

//                // Unlock the bits.
//                bitmap.UnlockBits(bmpData);
//            }
//        }
//    }
//}
