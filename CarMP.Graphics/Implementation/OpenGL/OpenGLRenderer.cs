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
    public class OpenGLRenderer : IRenderer, IDisposable, IWindow
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MouseButtonEvent
        {
            public int Button;
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseMoveEvent
        {
            public int X;
            public int Y;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct KeyEvent
        {
            public int Code;
            [MarshalAs(UnmanagedType.I1)]
            public bool Alt;
            [MarshalAs(UnmanagedType.I1)]
            public bool Control;
            [MarshalAs(UnmanagedType.I1)]
            public bool Shift;
            [MarshalAs(UnmanagedType.I1)]
            public bool System;
        }

        public delegate void MouseButtonEventHandler(MouseButtonEvent mouseButtonEvent);
        public delegate void MouseMoveEventHandler(MouseMoveEvent mouseButtonEvent);
        public delegate void KeyboardEventHandler(KeyEvent testStructure);
        public delegate void WindowCloseEventHandler();

        //public delegate void ResizeEvent(
        internal const string InterfaceLibrary = @"CarMP_OpenGL.dll";

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterMouseUpCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeRegisterMouseUpCallback(MouseButtonEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterMouseDownCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeRegisterMouseDownCallback(MouseButtonEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterMouseMoveCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeRegisterMouseMoveCallback(MouseMoveEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterKeyboardCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeRegisterKeyboardCallback(KeyboardEventHandler pHandler);
        
        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterWindowCloseCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeRegisterWindowCloseCallback(WindowCloseEventHandler pHandler);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "TestFuncion"), SuppressUnmanagedCodeSecurity]
        private static extern void TestFunction();

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DisplayBuffer"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDisplayBuffer();

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "CreateOGLWindow"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeCreateWindow(Rectangle rect);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawImage"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawImage(IntPtr pObjectPointer, Rectangle pRectangle, float pAlpha);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawRectangle(Color pColor, Rectangle pRect, float pLineWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FillRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeFillRectangle(Color pColor, Rectangle pRect);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawEllipse"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawEllipse(Geometry.Ellipse pEllipse, Color pColor, float pLineWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FillEllipse"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeFillEllipse(Geometry.Ellipse pEllipse, Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawLine"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawLine(Geometry.Point pPoint1, Geometry.Point pPoint2, Color pColor, float pWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawTextLayout"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawText(IntPtr pObjectPointer, Rectangle pRectangle, Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "Clear"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeClear(Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "PushClip"), SuppressUnmanagedCodeSecurity]
        private static extern void NativePushClip(Rectangle pRectangle);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "PopClip"), SuppressUnmanagedCodeSecurity]
        private static extern void NativePopClip();

        private List<GCHandle> _unmanaged_references = new List<GCHandle>();

        public OpenGLRenderer()
        {
            var mouseDelegate = new MouseMoveEventHandler(ProcessMouseMoveEvent);
            var keyboardEvent = new KeyboardEventHandler(ProcessKeyPressedEvent);
            var mouseDownEvent = new MouseButtonEventHandler(ProcessMouseDownEvent);
            var mouseUpEvent = new MouseButtonEventHandler(ProcessMouseUpEvent);

            _unmanaged_references.Add(GCHandle.Alloc(mouseDelegate));
            _unmanaged_references.Add(GCHandle.Alloc(mouseDownEvent));
            _unmanaged_references.Add(GCHandle.Alloc(mouseUpEvent));
            _unmanaged_references.Add(GCHandle.Alloc(keyboardEvent));

            NativeRegisterKeyboardCallback(keyboardEvent);
            NativeRegisterMouseDownCallback(mouseDownEvent);
            NativeRegisterMouseMoveCallback(mouseDelegate);
            NativeRegisterMouseUpCallback(mouseUpEvent);
        }

        ~OpenGLRenderer()
        {
        }

        #region IRenderer Members
        
        public Geometry.Rectangle CurrentBounds { get; set; }

        public void Resize(Geometry.SizeI pSize)
        {
            
        }

        public void BeginDraw()
        {
            
        }

        public void EndDraw()
        {
            NativeDisplayBuffer();
        }

        public void Clear(Color pColor)
        {
            NativeClear(pColor);
        }

        public void Flush()
        {
            return;
        }

        public void PushClip(Geometry.Rectangle pRectangle)
        {
            return;
        }

        public void PopClip()
        {
            return;
        }

        public void DrawRectangle(IBrush pBrush, Geometry.Rectangle pRectangle, float pLineWidth)
        {
            NativeDrawRectangle(pBrush.Color, TransformRectangle(pRectangle), pLineWidth);
        }

        public void DrawLine(Geometry.Point pPoint1, Geometry.Point pPoint2, IBrush pBrush, float pLineWidth)
        {
            NativeDrawLine(pPoint1, pPoint2, pBrush.Color, pLineWidth);
        }

        public void FillRectangle(IBrush pBrush, Geometry.Rectangle pRectangle)
        {
            NativeFillRectangle(pBrush.Color, pRectangle);
        }

        public void DrawImage(Geometry.Rectangle pRectangle, IImage pImage, float pAlpha)
        {
            NativeDrawImage((pImage as OpenGLImage).NativeImagePointer, TransformRectangle(pRectangle), pAlpha);
        }

        public void DrawEllipse(Geometry.Ellipse pEllipse, IBrush pBrush, float pLineWidth)
        {
            NativeDrawEllipse(pEllipse, pBrush.Color, pLineWidth);
        }

        public void FillEllipse(Geometry.Ellipse pEllipse, IBrush pBrush)
        {
            NativeFillEllipse(pEllipse, pBrush.Color);
        }

        public void DrawString(Geometry.Rectangle pRectangle, IStringLayout pStringLayout, IBrush pBrush)
        {
            NativeDrawText((pStringLayout as OpenGLStringLayout).NativeTextLayoutPointer,
                TransformRectangle(pRectangle),  pBrush.Color);
        }

        public IBrush CreateBrush(Color pColor)
        {
            return new OpenGLBrush
            {
                Color = pColor
            };
        }

        public IStringLayout CreateStringLayout(string pString, string pFont, float pSize, StringAlignment pAlignment)
        {
            return new OpenGLStringLayout(pString, pFont, pSize, pAlignment);
        }

        public IStringLayout CreateStringLayout(string pString, string pFont, float pSize)
        {
            return new OpenGLStringLayout(pString, pFont, pSize);
        }

        public IImage CreateImage(byte[] pData, int pStride)
        {
            return new OpenGLImage(pData, pStride);
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

        private Point TransformPoint(Point pPoint)
        {
            return new Point(pPoint.X + CurrentBounds.Left, pPoint.Y + CurrentBounds.Top);
        }
        private Rectangle TransformRectangle(Rectangle pRectangle)
        {
            return new Rectangle(pRectangle.Left + CurrentBounds.Left,
                pRectangle.Top + CurrentBounds.Top,
                pRectangle.Width,
                pRectangle.Height);
        }

        private Ellipse TransformEllipse(Geometry.Ellipse pEllipse)
        {
            return new Ellipse(TransformPoint(pEllipse.Point), pEllipse.RadiusX, pEllipse.RadiusY);
        }

        private void ProcessKeyPressedEvent(KeyEvent pKeyEvent)
        {
            // TODO: Translate KeyEvent
            if (_processKeyPress != null) _processKeyPress('l', Keys.A);
        }

        private void ProcessMouseMoveEvent(MouseMoveEvent pMouseMove)
        {
            if(_processMouseMove != null) _processMouseMove(new Point(pMouseMove.X, pMouseMove.Y));
        }

        private void ProcessMouseUpEvent(MouseButtonEvent pMouseUpEvent)
        {
            if (_processMouseUp != null) _processMouseUp(new Point(pMouseUpEvent.X, pMouseUpEvent.Y));
        }

        private void ProcessMouseDownEvent(MouseButtonEvent pMouseDownEvent)
        {
            if (_processMouseDown != null) _processMouseDown(new Point(pMouseDownEvent.X, pMouseDownEvent.Y));
        }

        private void ProcessWindowCloseEvent()
        {
        }

        #region IWindow Members

        public void SetProcessKeyPress(Action<char, CarMP.Graphics.Keys> pCallback) { _processKeyPress = pCallback; }
        public void SetProcessMouseMove(Action<Point> pCallback) { _processMouseMove = pCallback; }
        public void SetProcessMouseDown(Action<Point> pCallback) { _processMouseDown = pCallback; }
        public void SetProcessMouseUp(Action<Point> pCallback) { _processMouseUp = pCallback; }

        public void CreateWindow(Point pWindowLocation, Size pWindowSize)
        {
            NativeCreateWindow(new Rectangle(pWindowLocation, pWindowSize));
        }

        public IRenderer Renderer
        {
            get
            {
                return this;
            }
        }

        #endregion

        private Action<char, CarMP.Graphics.Keys> _processKeyPress;
        private Action<Point> _processMouseMove;
        private Action<Point> _processMouseDown;
        private Action<Point> _processMouseUp;

    }
}
