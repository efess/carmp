using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;

namespace CarMP.Graphics.Implementation.OpenTk
{
    public class OpenTkRenderer : IRenderer
    {

        #region IRenderer Members

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

        public void Resize(Geometry.Size pSize)
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
            throw new NotImplementedException();
        }

        public void DrawEllipse(Geometry.Ellipse pEllipse, IBrush pBrush, float pLineWidth)
        {
            throw new NotImplementedException();
        }

        public void FillEllipse(Geometry.Ellipse pEllipse, IBrush pBrush)
        {
            throw new NotImplementedException();
        }

        public void DrawString(Geometry.Point pPoint, IStringLayout pStringLayout, IBrush pBrush)
        {
            throw new NotImplementedException();
        }

        public IBrush CreateBrush(Color pColor)
        {
            throw new NotImplementedException();
        }

        public IStringLayout CreateStringLayout(string pText, string pFont, float pSize)
        {
            throw new NotImplementedException();
        }

        public IImage CreateImage(byte[] pData, int pStride)
        {
            throw new NotImplementedException();
        }

        public IImage CreateImage(string pPath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
