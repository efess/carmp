using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp
{
    public interface ISkinable
    {
        void ApplySkin(XmlNode pNode, string pPath);
    }
}
