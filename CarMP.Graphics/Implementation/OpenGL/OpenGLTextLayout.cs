using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using System.Security;
using System.Runtime.InteropServices;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Implementation.OpenGL
{
    public class OpenGLStringLayout : IStringLayout, IDisposable
    {
        [DllImport(OpenGLRenderer.InterfaceLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "CreateTextLayout"), SuppressUnmanagedCodeSecurity]
        private static extern IntPtr NativeCreateTextLayout(string pString, string pFont, float pSize, int pAlignment);

        [DllImport(OpenGLRenderer.InterfaceLibrary, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FreeTextLayout"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeFreeTextLayout(IntPtr pNativePointer);

        internal IntPtr NativeTextLayoutPointer { get; private set; }

        private string _string;
        public string String { get { return _string; } set { _string = value; UpdateText(); } }

        private float _size;
        public float Size { get { return _size; } set { _size = value; UpdateText(); } }

        private string _font;
        public string Font { get { return _font; } set { _font = value; UpdateText(); } }
        
        private StringAlignment _alignment;
        public StringAlignment Alignment { get { return _alignment; } set { _alignment= value; UpdateText(); } }
        
        private void UpdateText()
        {
            NativeTextLayoutPointer = NativeCreateTextLayout(_string, _font, _size, (int)Alignment);
        }

        public Size GetStringSize()
        {
            return new Size(3,3);
        }

        public int GetCharPositionAtPoint(Point pPoint)
        {
            return 0;
        }
        public Point GetPointAtCharPosition(int pCharPosition)
        {
            return new Point(0, 0);
        }
        public bool WordWrap { get; set;}

        internal OpenGLStringLayout(string pString, string pFont, float pSize)
            :this(pString, pFont, pSize, StringAlignment.Left)
        {
        }
        internal OpenGLStringLayout(string pString, string pFont, float pSize, StringAlignment pAlignment)
        {
            _string = pString;
            _size = pSize;
            _font = pFont;
            _alignment = pAlignment;

            UpdateText();
        }

        public void Dispose()
        {
            if (NativeTextLayoutPointer != IntPtr.Zero)
            {
                NativeFreeTextLayout(NativeTextLayoutPointer);
                NativeTextLayoutPointer = IntPtr.Zero;
            }
        }
        
    }
}
