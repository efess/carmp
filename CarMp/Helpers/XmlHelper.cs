using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CarMp
{
    public static class XmlHelper
    {
        public static RectangleF GetBoundsRectangle(string pXmlText)
        {
            string[] bounds = pXmlText.Split(new char[] { ',' });

            if (bounds.Length == 4)
            {
                try
                {
                    RectangleF boundsRect = new RectangleF((float)Convert.ToDouble(bounds[0]), (float)Convert.ToDouble(bounds[1]), (float)Convert.ToDouble(bounds[2]), (float)Convert.ToDouble(bounds[3]));
                    return boundsRect;
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
    }
}
