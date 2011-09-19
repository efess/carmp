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
        private static extern int NativeDrawImage(IntPtr pObjectPointer, Rectangle pRectangle, float pAlpha);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeDrawRectangle(Color pColor, Rectangle pRect, float pLineWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FillRectangle"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeFillRectangle(Color pColor, Rectangle pRect);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawEllipse"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeDrawEllipse(Geometry.Ellipse pEllipse, Color pColor, float pLineWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "FillEllipse"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeFillEllipse(Geometry.Ellipse pEllipse, Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawLine"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeDrawLine(Geometry.Point pPoint1, Geometry.Point pPoint2, Color pColor, float pWidth);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "DrawText"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeDrawText(IntPtr pObjectPointer, Rectangle pRectangle, Color pColor);

        [DllImport(InterfaceLibrary, CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "Clear"), SuppressUnmanagedCodeSecurity]
        private static extern int NativeClear(Color pColor);

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

        public void CreateWindow(RenderEventHandler pRenderDelegate)
        {
            _unmanaged_references.Add(GCHandle.Alloc(pRenderDelegate));
            NativeRegisterRenderCallback(pRenderDelegate);
            new Action(() => NativeCreateWindow(new Rectangle(5, 7, 800, 600))).BeginInvoke(null, null) ;
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

        public Geometry.Rectangle CurrentBounds { get; set; }

        public void Resize(Geometry.SizeI pSize)
        {
            
        }

        public void BeginDraw()
        {
            
        }

        public void EndDraw()
        {
           
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
    }
}
