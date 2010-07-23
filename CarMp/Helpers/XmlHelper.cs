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
        public static float[] GetColorFromCommaRgb(string pXmlText)
        {
            string[] parsed = pXmlText.Split(new char[] { ',' });
            if (parsed.Length < 3 || parsed.Length > 4)
                throw new FormatException("Invalid number of comma separated values");

            float[] rgb = new float[4];
            try
            {
                rgb[0] = (float)Convert.ToDouble(parsed[0]);
                rgb[1] = (float)Convert.ToDouble(parsed[1]);
                rgb[2] = (float)Convert.ToDouble(parsed[2]);

                if (parsed.Length == 4)
                    rgb[3] = (float)Convert.ToDouble(parsed[3]);
                else
                    rgb[3] = 256;
            }
            catch
            {
                throw new FormatException("Error parsing values from Hex");
            }
            return rgb;
        }
        public static float[] GetColorFromHtmlCode(string pXmlText)
        {
            string modifiedText = pXmlText;
            if (pXmlText.Length == 7)
            {
                if (pXmlText[0] != '#')
                    throw new FormatException("Expected hash symbol but not found");
                modifiedText = pXmlText.Substring(1, 6);
            }
            else if (pXmlText.Length != 6)
            {
                throw new FormatException("Invalid string length");
            }

            float[] rgb = new float[4];
            try
            {
                byte[] parsed = new byte[6];
                for(int i = 0; i < 6; i++)
                    parsed[i] = Convert.ToByte(modifiedText[i]);

                rgb[0] = parsed[0] + parsed[1];
                rgb[1] = parsed[2] + parsed[3];
                rgb[2] = parsed[4] + parsed[5]; 
                rgb[3] = 256f;
            }
            catch
            {
                throw new FormatException("Error parsing values from Hex");
            }
            return rgb;
        }
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
