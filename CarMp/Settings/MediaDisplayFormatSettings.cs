using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CarMP.Settings
{
    public class MediaDisplayFormatSettings : XmlSettingsBase, IXmlSettings
    {
        private const string XML_ELEMENT_THIS = "MediaFormatSettings";

        private const string XML_NODE_FORMAT = "FormatTemplate";
        private const string XML_NODE_REPLACE_UNDERSCORES = "ReplaceUnderscoresWithSpace";
        private const string XML_NODE_REPLACE_PERCENT_20 = "ReplacePercent20WithSpace";

        public string FormatTemplate { get; set; }
        public bool ReplaceUnderscores { get; set; }
        public bool ReplacePercentTwenty { get; set; }

        public string ElementName { get { return XML_ELEMENT_THIS; } }

        public void SetDefaults()
        {
            FormatTemplate = "%artist% - %title%";
            ReplacePercentTwenty = true;
            ReplaceUnderscores = true;
        }

        public void PopulateSettings(System.Xml.Linq.XElement pXElement)
        {
            SetOrCreateNode(pXElement, XML_NODE_FORMAT, FormatTemplate);
            SetOrCreateNode(pXElement, XML_NODE_REPLACE_UNDERSCORES, GetXmlFromBool(ReplaceUnderscores));
            SetOrCreateNode(pXElement, XML_NODE_REPLACE_PERCENT_20, GetXmlFromBool(ReplacePercentTwenty));
        }

        public void ExtractSettings(System.Xml.Linq.XElement pXElement)
        {
            foreach (XElement node in pXElement.DescendantNodes().OfType<XElement>())
            {
                switch (node.Name.ToString())
                {
                    case XML_NODE_FORMAT:
                        FormatTemplate = node.Value;
                        break;
                    case XML_NODE_REPLACE_UNDERSCORES:
                        ReplaceUnderscores = GetBoolFromXml(node.Value);
                        break;
                    case XML_NODE_REPLACE_PERCENT_20:
                        ReplacePercentTwenty = GetBoolFromXml(node.Value);
                        break;
                }
            }
        }
    }
}
