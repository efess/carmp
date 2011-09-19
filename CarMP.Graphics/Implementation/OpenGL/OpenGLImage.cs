using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using CarMP.Graphics.Interfaces;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Implementation.OpenGL
{
    public class OpenGLImage : IImage, IDisposable
    {
        [DllImport(OpenGLRenderer.InterfaceLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "CreateImage"), SuppressUnmanagedCodeSecurity]
        private static extern IntPtr NativeCreateImage(string pPath);

        [DllImport(OpenGLRenderer.InterfaceLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "CreateImageFromByteArray"), SuppressUnmanagedCodeSecurity]
        private static extern IntPtr NativeCreateImage(byte[] pByteArray, int pStride);

        [DllImport(OpenGLRenderer.InterfaceLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FreeImage"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeFreeImage(IntPtr pNativePointer);
        
        internal IntPtr NativeImagePointer { get; private set;}

        public Size Size { get; set; }

        public OpenGLImage(string pPath)
        {
            NativeImagePointer = IntPtr.Zero;
            //IntPtr ip = Marshal.StringToHGlobalAnsi(pPath);
            //Cast
            //const char* str = static_cast<const char*>(ip.ToPointer());
            NativeImagePointer = NativeCreateImage(pPath);
            
            //Marshal.FreeHGlobal(ip);
        }

        public OpenGLImage(byte[] pByteArray, int pStride)
        {
            NativeImagePointer = IntPtr.Zero;
            //IntPtr ip = Marshal.StringToHGlobalAnsi(pPath);
            //Cast
            //const char* str = static_cast<const char*>(ip.ToPointer());
            NativeImagePointer = NativeCreateImage(pByteArray, pStride);

            //Marshal.FreeHGlobal(ip);
        }

        ~OpenGLImage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (NativeImagePointer != IntPtr.Zero)
            {
                NativeFreeImage(NativeImagePointer);
                NativeImagePointer = IntPtr.Zero;
            }
        }
    }
}
