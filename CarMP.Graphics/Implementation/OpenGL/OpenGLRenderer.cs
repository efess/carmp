using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using System.Runtime.InteropServices;
using System.Security;

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
        internal static extern int DeleteResource(int pResourceId);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterMouseCallback"), SuppressUnmanagedCodeSecurity]
        private static extern int RegisterMouseCallback(MouseEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterRenderCallback"), SuppressUnmanagedCodeSecurity]
        private static extern int RegisterRenderCallback(RenderEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterKeyboardCallback"), SuppressUnmanagedCodeSecurity]
        private static extern int RegisterKeyboardCallback(KeyboardEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "TestFuncion"), SuppressUnmanagedCodeSecurity]
        private static extern int TestFunction();

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "CreateWindow"), SuppressUnmanagedCodeSecurity]
        private static extern int CreateWindow();

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawImage"), SuppressUnmanagedCodeSecurity]
        private static extern int DrawImage(int pTextureId, float pAlpha);

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

            RegisterRenderCallback(renderDelegate);
            RegisterMouseCallback(mouseDelegate);
            RegisterKeyboardCallback(keyboardEvent);

            CreateWindow();
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
            throw new NotImplementedException();
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
            DrawImage((pImage as OpenGLImage).TextureId, pAlpha);
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
