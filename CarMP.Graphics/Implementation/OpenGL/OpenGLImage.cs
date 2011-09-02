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
        private static extern int CreateImage(string pPath);
        public int TextureId { get; private set;}


        public Size Size { get; set; }

        public OpenGLImage(string pPath)
        {
            
            //IntPtr ip = Marshal.StringToHGlobalAnsi(pPath);
            //Cast
            //const char* str = static_cast<const char*>(ip.ToPointer());
            TextureId = CreateImage(pPath);
            
            //Marshal.FreeHGlobal(ip);
        }

        public void Dispose()
        {
            OpenGLRenderer.DeleteResource(TextureId);
        }

    }
}
