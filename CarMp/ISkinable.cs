using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMP
{
    public interface ISkinable
    {
        void ApplySkin(XmlNode pNode, string pPath);
    }
}
