using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMp
{
    public static class XmlHelper
    {
        public static RectF GetBoundsRectangle(string pXmlText)
        {
            string[] bounds = pXmlText.Split(new char[] { ',' });

            if (bounds.Length == 4)
            {
                try
                {
                    float left = (float)Convert.ToDouble(bounds[0]);
                    float top = (float)Convert.ToDouble(bounds[1]);
                    float right =  (float)Convert.ToDouble(bounds[2]) + left;
                    float bottom =  (float)Convert.ToDouble(bounds[3]) + top;

                    return new RectF(left, top, right, bottom);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error parsing bounds: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Incorrect bounds format. Correct format is #,#,#,#");
            }
        }

        public static CarMp.ViewControls.AnimationPathPoint GetAnimationPathPoint(string pXmlText)
        {
            string[] bounds = pXmlText.Split(new char[] { ',' });

            if (bounds.Length == 4)
            {
                try
                {
                    CarMp.ViewControls.AnimationPathPoint pathPoint
                        = new CarMp.ViewControls.AnimationPathPoint((float)Convert.ToDouble(bounds[0]), (float)Convert.ToDouble(bounds[1]), Convert.ToInt32(bounds[2]), (float)Convert.ToDouble(bounds[3]));

                    return pathPoint;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error parsing AnimationPathPoint: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Incorrect AnimationPathPoint format. Correct format is #,#,#,#");
            }
        }

        public static Point2F GetPoint(string pXmlText)
        {
            string[] bounds = pXmlText.Split(new char[] { ',' });

            if (bounds.Length == 2)
            {
                try
                {
                    return new Point2F((float)Convert.ToDouble(bounds[0]), (float)Convert.ToDouble(bounds[1]));
                }
                catch (Exception ex)
                {
                    throw new Exception("Error parsing AnimationPathPoint: " + ex.Message);
                }
            }
            else
            {
                throw new Exception("Incorrect AnimationPathPoint format. Correct format is #,#,#,#");
            }
        }
    }
}
