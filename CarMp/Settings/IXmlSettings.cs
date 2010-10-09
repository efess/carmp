using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CarMP.Settings
{
    internal interface IXmlSettings
    {
        void PopulateSettings(XElement pXElement);
        void ExtractSettings(XElement pXElement);
        void SetDefaults();
        string ElementName { get; }
    }
}
