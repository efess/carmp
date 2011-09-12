using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using System.Runtime.InteropServices;
using System.Security;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Implementation.OpenGL
{
    public class OpenGLRenderer : IRenderer, IDisposable
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseEvent
        {
            public float X;
            public float Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardEvent
        {
            public byte c;
        }

        public delegate void RenderEventHandler();
        public delegate void MouseEventHandler(MouseEvent testStructure);
        public delegate void KeyboardEventHandler(KeyboardEvent testStructure);

        internal const string InterfaceLibrary = @"CarMP_OpenGL.dll";

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DeleteResource"), SuppressUnmanagedCodeSecurity]
        internal static extern int NativeDeleteResource(int pResourceId);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterMouseCallback"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeRegisterMouseCallback(MouseEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterRenderCallback"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeRegisterRenderCallback(RenderEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterKeyboardCallback"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeRegisterKeyboardCallback(KeyboardEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "TestFuncion"), SuppressUnmanagedCodeSecurity]
        private static extern int TestFunction();

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "CreateOGLWindow"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeCreateWindow(Rectangle rect);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawImage"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeDrawImage(Rectangle pRectangle, int pTextureId, float pAlpha);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeDrawRectangle(Color pColor, Rectangle pRect, float pLineWidth);

        private List<GCHandle> _unmanaged_references = new List<GCHandle>();

        #region IRenderer Members

        public bool DoesItWork(RenderEventHandler pRenderDelegate)
        {
            var renderDelegate = pRenderDelegate;//new RenderEventHandler(OnRender);
            var mouseDelegate = new MouseEventHandler(OnMouse);
            var keyboardEvent = new KeyboardEventHandler(OnKeyboard);

            _unmanaged_references.Add(GCHandle.Alloc(renderDelegate));
            _unmanaged_references.Add(GCHandle.Alloc(mouseDelegate));
            _unmanaged_references.Add(GCHandle.Alloc(keyboardEvent));

            NativeRegisterRenderCallback(renderDelegate);
            NativeRegisterMouseCallback(mouseDelegate);
            NativeRegisterKeyboardCallback(keyboardEvent);

            NativeCreateWindow(new Rectangle(5, 7, 800, 600));

            return true;
        }

        public void OnRender()
        {
        }

        public void OnMouse(MouseEvent pMouseEvent)
        {
            Console.WriteLine("MouseEvent: (" + pMouseEvent.X + "," + pMouseEvent.Y + ")");
        }

        public void OnKeyboard(KeyboardEvent pKeyboardEvent)
        {
            Console.WriteLine("KeyboardEvent: (" + pKeyboardEvent.c + ")");
        }

        public Geometry.Rectangle CurrentBounds
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Resize(Geometry.SizeI pSize)
        {
            throw new NotImplementedException();
        }

        public void BeginDraw()
        {
            throw new NotImplementedException();
        }

        public void EndDraw()
        {
            throw new NotImplementedException();
        }

        public void Clear(Color pColor)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public void PushClip(Geometry.Rectangle pRectangle)
        {
            throw new NotImplementedException();
        }

        public void PopClip()
        {
            throw new NotImplementedException();
        }

        public void DrawRectangle(IBrush pBrush, Geometry.Rectangle pRectangle, float pLineWidth)
        {
            NativeDrawRectangle(pBrush.Color, pRectangle, pLineWidth);
        }

        public void DrawLine(Geometry.Point pPoint1, Geometry.Point pPoint2, IBrush pBrush, float pLineWidth)
        {
            throw new NotImplementedException();
        }

        public void FillRectangle(IBrush pBrush, Geometry.Rectangle pRectangle)
        {
            throw new NotImplementedException();
        }

        public void DrawImage(Geometry.Rectangle pRectangle, IImage pImage, float pAlpha)
        {
            NativeDrawImage(pRectangle, (pImage as OpenGLImage).TextureId, pAlpha);
        }

        public void DrawEllipse(Geometry.Ellipse pEllipse, IBrush pBrush, float pLineWidth)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Geometry.Ellipse pEllipse, IBrush pBrush)
        {
            throw new NotImplementedException();
        }

        public void DrawString(Geometry.Rectangle pRectangle, IStringLayout pStringLayout, IBrush pBrush)
        {
            throw new NotImplementedException();
        }

        public IBrush CreateBrush(Color pColor)
        {
            throw new NotImplementedException();
        }

        public IStringLayout CreateStringLayout(string pString, string pFont, float pSize, StringAlignment pAlignment)
        {
            throw new NotImplementedException();
        }

        public IStringLayout CreateStringLayout(string pString, string pFont, float pSize)
        {
            throw new NotImplementedException();
        }

        public IImage CreateImage(byte[] pData, int pStride)
        {
            throw new NotImplementedException();
        }

        public IImage CreateImage(string pPath)
        {
            return new OpenGLImage(pPath);
        }

        #endregion

        public void Dispose()
        {
            foreach (GCHandle handle in _unmanaged_references)
                handle.Free();
            //_unmanaged_references.
        }
    }
}
