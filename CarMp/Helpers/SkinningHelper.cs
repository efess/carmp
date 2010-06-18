using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace CarMp
{
    public static class SkinningHelper
    {

        public static bool XmlBitmapEntry(string pXpath, XmlNode pXmlNode, string pSkinPath, ref Direct2D.BitmapData pBitmap)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                pBitmap = new Direct2D.BitmapData(System.IO.Path.Combine(pSkinPath, xmlNode.InnerText));
                return true;
            }
            pBitmap = pBitmap;
            return false;
        }

        public static bool XmlRectangleFEntry(string pXpath, XmlNode pXmlNode, ref RectangleF pRectangleF)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                pRectangleF = XmlHelper.GetBoundsRectangle(xmlNode.InnerText);
                return true;
            }
            return false;
        }
    }
}
