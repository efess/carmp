using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMp
{
    public static class LinearMath
    {

        public static float DistanceBetweenTwoPoint(Point2F pPoint1, Point2F pPoint2)
        {
            return (float)Math.Sqrt(
                Math.Pow(pPoint2.X - pPoint1.X, 2) +
                Math.Pow(pPoint2.Y - pPoint1.Y, 2)
                );
        }

        public static float SlopeOfTwoPoints(Point2F pPoint1, Point2F pPoint2)
        {
            float slope = 1;
            if (pPoint1.X != pPoint2.X)
            {
                slope = (pPoint2.Y - pPoint1.Y) /
                 (pPoint2.X - pPoint1.X);
            }
            else if (pPoint1.Y < pPoint2.Y)
                slope = -1;

            return slope;
        }

        /// <summary>
        /// Finds angle of Slope in Radians
        /// </summary>
        public static float AngleOfTwoPoints(Point2F pPoint1, Point2F pPoint2)
        {
            float slope = 0;
            if (pPoint1.X != pPoint2.X)
            {
                slope = (pPoint2.Y - pPoint1.Y) /
                 (pPoint2.X - pPoint1.X);
            }
            else if (pPoint1.Y < pPoint2.Y)
                return (float)Math.PI / 2;
            else
                return (float)Math.PI * 1.5f;

            return (float)Math.Atan(slope);

        }
    }
}
