using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CarMp
{
    public static class LinearMath
    {
        public static float SlopeOfTwoPoints(PointF pPoint1, PointF pPoint2)
        {
            float slope = 1;
            if (pPoint1.X != pPoint2.X)
            {
                slope = (pPoint2.Y - pPoint1.Y) /
                 (pPoint2.X - pPoint1.X);
            }
            else if (pPoint2.Y < pPoint2.Y)
                slope = -1;

            return slope;
        }

        /// <summary>
        /// Finds angle of Slope in Radians
        /// </summary>
        public static float AngleOfTwoPoints(PointF pPoint1, PointF pPoint2)
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
