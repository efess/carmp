using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CarMP.Settings
{
    public abstract class XmlSettingsBase
    {
        protected void EnsureNodeExistsOrCreate(XElement pXElement, string pNodeName)
        {
            XElement node = pXElement.Element(pNodeName);
            if (node == null)
            {
                pXElement.Add(node = new XElement(pNodeName));
            }
        }

        protected void SetOrCreateNode(XElement pXElement, string pNodeName, string pNodeValue)
        {
            SetOrCreateNode(pXElement, pNodeName, new Action<XElement>(element => element.Value = pNodeValue));
        }

        protected void SetOrCreateNode(XElement pXElement, string pNodeName, Action<XElement> pCreater)
        {
            XElement node = pXElement.Element(pNodeName);
            if (node == null)
            {
                pXElement.Add(node = new XElement(pNodeName));
            }
            pCreater(node);
        }

        protected bool GetBoolFromXml(string pXmlValue)
        {
            return pXmlValue.Trim() != "0";
        }

        protected string GetXmlFromBool(bool pBoolValue)
        {
            return pBoolValue ? "1" : "0";
        }

    }
}
