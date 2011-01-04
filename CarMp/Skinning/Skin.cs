using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace CarMP.Skinning
{
    public class Skin
    {
        private const string XPATH_SKIN_NAME = "Skin/Name";
        private const string XPATH_VIEWS = "Skin/Views/*";
        private const string XPATH_OVERLAY_CONTROLS = "Skin/OverlayControls/*";
        public const string SKIN_PREVIEW_FULL_FILE_NAME = "preview.png";

        private Dictionary<string, XmlNode> viewNodes = new Dictionary<string, XmlNode>();
        private Dictionary<string, XmlNode> overlayNodes = new Dictionary<string, XmlNode>();

        // Resolution
        // Day/Night?

        public string CurrentSkinPath { get; private set; }
        public string Name { get; private set; }
        public string SkinLoadError { get; private set; }
        public string SkinPreviewImagePath { get; private set; }

        public Skin(string pSkinXmlPath)
        {
            LoadXml(pSkinXmlPath);
        }

        public void ReloadCurrent()
        {
            LoadXml(CurrentSkinPath);
        }

        private void LoadXml(string pXmlPath)
        {
            SkinLoadError = null;

            CurrentSkinPath = pXmlPath;
            viewNodes.Clear();
            overlayNodes.Clear();

            string xmlFile = Path.Combine(pXmlPath, Constants.SKIN_FULL_FILE_NAME);


            if(string.IsNullOrEmpty(xmlFile))
            {
                SkinLoadError = "Skin XML file not found";
                return;
            }

            XmlDocument xdoc = new XmlDocument();
            try
            {
                xdoc.Load(xmlFile);
            }
            catch (XmlException ex)
            {
                SkinLoadError = "Could not parse Skin XML: " + ex.Message;
                return;
            }

            string previewFile = Path.Combine(pXmlPath, SKIN_PREVIEW_FULL_FILE_NAME);
            if (File.Exists(previewFile))
                SkinPreviewImagePath = previewFile;

            foreach(XmlNode node in xdoc.SelectNodes(XPATH_VIEWS))
                viewNodes.Add(node.Name, node);

            foreach (XmlNode node in xdoc.SelectNodes(XPATH_OVERLAY_CONTROLS))
                overlayNodes.Add(node.Name, node);
            
            var nameNode = xdoc.SelectSingleNode(XPATH_SKIN_NAME);
            if(nameNode == null)
            {
                SkinLoadError = "Skin Name not found in XML";
                return;
            }
            Name = nameNode.InnerText;
        }

        public XmlNode GetViewNodeSkin(string pViewName)
        {
            return viewNodes[pViewName];
        }

        public XmlNode GetOverlayNodeSkin(string pViewName)
        {
            return overlayNodes[pViewName];
        }
        public IEnumerable<XmlNode> OverlayNodes
        {
            get { return overlayNodes.Select((kv) => kv.Value);}
        }
    }
}
