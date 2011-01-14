using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using CarMP.Graphics;

namespace CarMP.Helpers
{
    public static class SkinningHelper
    {

        public static bool XmlValidFilePath(string pXpath, XmlNode pXmlNode, string pSkinPath, ref string pFilePath)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                string tempPath = System.IO.Path.Combine(pSkinPath, xmlNode.InnerText);
                if (System.IO.File.Exists(tempPath))
                {
                    pFilePath = System.IO.Path.Combine(pSkinPath, xmlNode.InnerText);
                    return true;
                }
            }
            return false;
        }

        public static bool XmlRectangleEntry(string pXpath, XmlNode pXmlNode, ref Rectangle pRectangle)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                pRectangle = XmlHelper.GetBoundsRectangle(xmlNode.InnerText);
                return true;
            }
            return false;
        }

        public static bool XmlPointEntry(string pXpath, XmlNode pXmlNode, ref Point pPoint)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                pPoint = XmlHelper.GetPoint(xmlNode.InnerText);
                return true;
            }
            return false;
        }

        public static bool XmlTextStyleEntry(string pXpath, XmlNode pXmlNode, ref TextStyle pFont)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                pFont = new TextStyle(xmlNode);
                return true;
            }
            return false;
        }

        public static bool XmlColorEntry(string pXpath, XmlNode pXmlNode, ref Color pColor)
        {
            XmlNode xmlNode = pXmlNode.SelectSingleNode(pXpath);
            if (xmlNode != null)
            {
                pColor = GraphicsHelper.ConvertFloatArrayToColor(XmlHelper.ConvertColor(xmlNode.InnerText));
                
                return true;
            }
            return false;
                
        }

        public static bool ApplySkinNodeIfExists(string pXPath, XmlNode pXmlNode, string pSkinPath, ISkinable pSkinable)
        {
            XmlNode node = pXmlNode.SelectSingleNode(pXPath);
            if (node != null)
            {
                pSkinable.ApplySkin(node, pSkinPath);
                return true;
            }
            return false;
        }
    }
}
