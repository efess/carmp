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
            EntryPoint = "DumpDebugInfo"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDumpDebugInfo();

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "RegisterWindowCloseCallback"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeRegisterWindowCloseCallback(Action pHandler);

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
        private static extern void NativeCreateWindow(ref Rectangle rect);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawImage"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawImage(IntPtr pObjectPointer, ref Rectangle pRectangle, float pAlpha);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawRectangle(ref Color pColor, ref Rectangle pRect, float pLineWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FillRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeFillRectangle(ref Color pColor, ref Rectangle pRect);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawEllipse"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawEllipse(ref Geometry.Ellipse pEllipse, ref Color pColor, float pLineWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FillEllipse"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeFillEllipse(ref Geometry.Ellipse pEllipse, ref Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawLine"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawLine(ref Geometry.Point pPoint1, ref Geometry.Point pPoint2, ref Color pColor, float pWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawTextLayout"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeDrawText(IntPtr pObjectPointer, ref Rectangle pRectangle, ref Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "Clear"), SuppressUnmanagedCodeSecurity]
        private static extern void NativeClear(ref Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "PushClip"), SuppressUnmanagedCodeSecurity]
        private static extern void NativePushClip(ref Rectangle pRectangle);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "PopClip"), SuppressUnmanagedCodeSecurity]
        private static extern void NativePopClip();

        private List<GCHandle> _unmanaged_references = new List<GCHandle>();

        public OpenGLRenderer()
        {
            var windowCloseDelegate = new Action(() =>
                {
                    NativeDumpDebugInfo();
                    //Environment.Exit(0);
                });

            var mouseDelegate = new MouseMoveEventHandler(ProcessMouseMoveEvent);
            var keyboardEvent = new KeyboardEventHandler(ProcessKeyPressedEvent);
            var mouseDownEvent = new MouseButtonEventHandler(ProcessMouseDownEvent);
            var mouseUpEvent = new MouseButtonEventHandler(ProcessMouseUpEvent);

            _unmanaged_references.Add(GCHandle.Alloc(windowCloseDelegate));
            _unmanaged_references.Add(GCHandle.Alloc(mouseDelegate));
            _unmanaged_references.Add(GCHandle.Alloc(mouseDownEvent));
            _unmanaged_references.Add(GCHandle.Alloc(mouseUpEvent));
            _unmanaged_references.Add(GCHandle.Alloc(keyboardEvent));

            NativeRegisterWindowCloseCallback(windowCloseDelegate);
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
            var color = pColor;

            NativeClear(ref color);
        }

        public void Flush()
        {
            return;
        }

        public void PushClip(Geometry.Rectangle pRectangle)
        {
            var rectangle = pRectangle;

            NativePushClip(ref rectangle);
        }

        public void PopClip()
        {
            NativePopClip();
        }

        public void DrawRectangle(IBrush pBrush, Geometry.Rectangle pRectangle, float pLineWidth)
        {
            var color = pBrush.Color;
            var rectangle = TransformRectangle(pRectangle);

            NativeDrawRectangle(ref color, ref rectangle, pLineWidth);
        }

        public void DrawLine(Geometry.Point pPoint1, Geometry.Point pPoint2, IBrush pBrush, float pLineWidth)
        {
            var point1 = pPoint1;
            var point2 = pPoint2;
            var color = pBrush.Color;

            NativeDrawLine(ref pPoint1, ref pPoint2, ref color, pLineWidth);
        }

        public void FillRectangle(IBrush pBrush, Geometry.Rectangle pRectangle)
        {
            var color = pBrush.Color;
            var rectangle = TransformRectangle(pRectangle);

            NativeFillRectangle(ref color, ref rectangle);
        }

        public void DrawImage(Geometry.Rectangle pRectangle, IImage pImage, float pAlpha)
        {
            var rectangle = TransformRectangle(pRectangle);

            NativeDrawImage((pImage as OpenGLImage).NativeImagePointer, ref rectangle, pAlpha);
        }

        public void DrawEllipse(Geometry.Ellipse pEllipse, IBrush pBrush, float pLineWidth)
        {
            var ellipse = TransformEllipse(pEllipse);
            var color = pBrush.Color;

            NativeDrawEllipse(ref ellipse, ref color, pLineWidth);
        }

        public void FillEllipse(Geometry.Ellipse pEllipse, IBrush pBrush)
        {
            var ellipse = TransformEllipse(pEllipse);
            var color = pBrush.Color;

            NativeFillEllipse(ref ellipse, ref color);
        }

        public void DrawString(Geometry.Rectangle pRectangle, IStringLayout pStringLayout, IBrush pBrush)
        {
            var rectangle = TransformRectangle(pRectangle);
            var color = pBrush.Color;

            NativeDrawText((pStringLayout as OpenGLStringLayout).NativeTextLayoutPointer,
                ref rectangle,  ref color);
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
            var rectangle = new Rectangle(pWindowLocation, pWindowSize);

            NativeCreateWindow(ref rectangle);
        }

        public IRenderer Renderer
        {
            get
            {
                return this;
            }
        }

        public void ProcessEvents()
        {
        }
        #endregion

        private Action<char, CarMP.Graphics.Keys> _processKeyPress;
        private Action<Point> _processMouseMove;
        private Action<Point> _processMouseDown;
        private Action<Point> _processMouseUp;

    }
}
