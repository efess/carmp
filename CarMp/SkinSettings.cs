using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace CarMP
{
    public class SkinSettings
    {
        private const string XML_FILE_NAME = "skin.xml";
        private const string XPATH_SKIN_NAME = "Skin/Name";
        private const string XPATH_VIEWS = "Skin/Views/*";
        private const string XPATH_OVERLAY_CONTROLS = "Skin/OverlayControls/*";

        private Dictionary<string, XmlNode> viewNodes = new Dictionary<string, XmlNode>();
        private Dictionary<string, XmlNode> overlayNodes = new Dictionary<string, XmlNode>();

        public string CurrentSkinPath { get; private set; }
        public string Name { get; private set; }

        public SkinSettings(string pSkinXmlPath)
        {
            LoadXml(pSkinXmlPath);
        }

        public void ReloadCurrent()
        {
            LoadXml(CurrentSkinPath);
        }
        private void LoadXml(string pXmlPath)
        {
            CurrentSkinPath = pXmlPath;
            viewNodes.Clear();
            overlayNodes.Clear();

            string xmlFile = Path.Combine(pXmlPath, XML_FILE_NAME);
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xmlFile);

            foreach(XmlNode node in xdoc.SelectNodes(XPATH_VIEWS))
            {
                viewNodes.Add(node.Name, node);
            }

            foreach (XmlNode node in xdoc.SelectNodes(XPATH_OVERLAY_CONTROLS))
            {
                overlayNodes.Add(node.Name, node);
            }
            
            Name = xdoc.SelectSingleNode(XPATH_SKIN_NAME).InnerText;
        }

        public XmlNode GetViewNodeSkin(string pViewName)
        {
            return viewNodes[pViewName];
        }

        public XmlNode GetOverlayNodeSkin(string pViewName)
        {
            return overlayNodes[pViewName];
        }
    }
}
