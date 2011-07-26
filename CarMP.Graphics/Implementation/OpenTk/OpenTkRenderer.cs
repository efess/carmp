using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using OpenTK.Graphics.OpenGL;
using CarMP.Graphics.Geometry;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System.Threading;

namespace CarMP.Graphics.Implementation.OpenTk
{
    /// <summary>
    ///  Much of the OpenTK rendering implementation's code is barrowed from the engine
    ///  specified here: http://www.opentk.com/node/257
    /// </summary>
    public class OpenTkRenderer : IRenderer
    {
        GraphicsContext context;
        IWindowInfo _windowInfo;
        ManualResetEvent _initialize;

        Queue<Action> _openGlCommandQueue;

        public OpenTkRenderer(IntPtr pWindowHandle)
        {
            _openGlCommandQueue = new Queue<Action>();
            _initialize = new ManualResetEvent(false);
            _windowInfo = OpenTK.Platform.Utilities.CreateWindowsWindowInfo(pWindowHandle);

            context = new GraphicsContext(GraphicsMode.Default, _windowInfo);

            context.MakeCurrent(_windowInfo);
            context.LoadAll();

            InitializeGL();
            context.MakeCurrent(null);
            _initialize.Set();
        }

        private void InitializeGL()
        {
            int[] viewPort = new int[4];

            GL.GetInteger(GetPName.Viewport, viewPort);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Ortho(viewPort[0], viewPort[0] + viewPort[2], viewPort[1] + viewPort[3], viewPort[1], -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Translate(0.375, 0.375, 0.0);

            GL.PushAttrib(AttribMask.DepthBufferBit);
            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Texture2D);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        private void ProcessActionQueue()
        {
            lock (_openGlCommandQueue)
            {
                while (_openGlCommandQueue.Count > 0)
                    _openGlCommandQueue.Dequeue().Invoke();
            }
        }

        #region IRenderer Members

        public Geometry.Rectangle CurrentBounds
        {
            get;
            set;
        }
        
        public void Resize(Geometry.SizeI pSize)
        {
            _openGlCommandQueue.Enqueue(() => GL.Viewport(0, 0, pSize.Width, pSize.Height));

            _openGlCommandQueue.Enqueue(() => GL.MatrixMode(MatrixMode.Projection));
            _openGlCommandQueue.Enqueue(() => GL.LoadIdentity());
            _openGlCommandQueue.Enqueue(() => GL.Ortho(0.0, pSize.Width, pSize.Height, 0.0, -1, 1));
            _openGlCommandQueue.Enqueue(() => GL.MatrixMode(MatrixMode.Modelview));
        }

        public void BeginDraw()
        {
            _initialize.WaitOne();
            context.MakeCurrent(_windowInfo);
            ProcessActionQueue();
        }

        public void EndDraw()
        {
            context.SwapBuffers();
        }

        public void Clear(Color pColor)
        {
            if (context.IsCurrent)
            {
                ClearInternal(pColor);
            }
            else
            {
                lock(_openGlCommandQueue)
                    _openGlCommandQueue.Enqueue(() => ClearInternal(pColor));
            }
        }

        private void ClearInternal(Color pColor)
        {
            GL.ClearColor(Color4.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public void Flush()
        {
            //GL.Flush();
        }

        public void PushClip(Geometry.Rectangle pRectangle)
        {
        }

        public void PopClip()
        {
        }

        public void DrawRectangle(IBrush pBrush, Geometry.Rectangle pRectangle, float pLineWidth)
        {
        }

        public void DrawLine(Geometry.Point pPoint1, Geometry.Point pPoint2, IBrush pBrush, float pLineWidth)
        {
        }

        public void FillRectangle(IBrush pBrush, Geometry.Rectangle pRectangle)
        {
        }

        public void DrawImage(Geometry.Rectangle pRectangle, IImage pImage, float pAlpha)
        {
            OTKImage image = pImage as OTKImage;
            image.SetDimensions(TransformRectangle(pRectangle));
            image.SetBlending(pAlpha);
            image.Draw();
        }

        public void DrawEllipse(Geometry.Ellipse pEllipse, IBrush pBrush, float pLineWidth)
        {
        }

        public void FillEllipse(Geometry.Ellipse pEllipse, IBrush pBrush)
        {
        }

        public void DrawString(Geometry.Rectangle pRectangle, IStringLayout pStringLayout, IBrush pBrush)
        {
            var otkStringLayout = pStringLayout as OTKStringLayout;
            otkStringLayout.SetDimensions(pRectangle);
            otkStringLayout.SetBrush(pBrush);
            otkStringLayout.Draw();
        }

        public IBrush CreateBrush(Color pColor)
        {
            return new OTKBrush
                {
                    Color = pColor
                };
        }

        public IStringLayout CreateStringLayout(string pText, string pFont, float pSize)
        {
            return new OTKStringLayout(pText, pFont, pSize);
        }

        public IStringLayout CreateStringLayout(string pText, string pFont, float pSize, StringAlignment pAlignment)
        {
            return new OTKStringLayout(pText, pFont, pSize, pAlignment);
        }

        public IImage CreateImage(byte[] pData, int pStride)
        {
            throw new NotImplementedException();
        }

        public IImage CreateImage(string pPath)
        {
            return new OTKImage(pPath);
        }

        #endregion

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
