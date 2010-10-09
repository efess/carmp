using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP
{
    public static class RectFExt
    {
        public static bool Contains(this RectF pRect, Point2F pPoint)
        {
            return pPoint.X < pRect.Right && pPoint.X > pRect.Left
                && pPoint.Y > pRect.Top && pPoint.Y < pRect.Bottom;
        }

        public static string ToString(this RectF pRect)
        {
            return (pRect.Left + ", " + pRect.Top + ", " + pRect.Right + ", " + pRect.Bottom);
        }

        public static RectF Intersect(this RectF pRectOne, RectF pRectTwo)
        {
            RectF rectF = new RectF(
                Math.Max(pRectOne.Left, pRectTwo.Left),
                Math.Max(pRectOne.Top, pRectTwo.Top),
                Math.Min(pRectOne.Right, pRectTwo.Right),
                Math.Min(pRectOne.Bottom, pRectTwo.Bottom));

            if (rectF.Right > rectF.Left
                && rectF.Bottom > rectF.Top)
                return rectF;
            return new RectF();
        }
    }
}
