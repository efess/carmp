using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace CarMP.ViewControls
{
    public static class ViewControlFactory
    {
        private const string XML_NODE_FUNCTION = "Function";
        private const string XML_FUNCTION_ATTR_PARAMETER = "Parameter";

        private const string XML_ATTR_TYPE = "Type";

        private static readonly FunctionalProperties functionalProperties = new FunctionalProperties();
        public static D2DViewControl GetViewControlAndApplySkin(string pName, string pSkinPath, XmlNode pNode)
        {

            var functionNode = pNode.SelectSingleNode(XML_NODE_FUNCTION);
            var typeAttr = pNode.Attributes[XML_ATTR_TYPE];
            var parameterAttr = functionNode != null 
                ? functionNode.Attributes[XML_FUNCTION_ATTR_PARAMETER]
                : null;

            if (typeAttr == null)
                return null;

            var viewControl = GetViewControl(typeAttr.InnerText);
            
            if (viewControl is ISkinable)
                (viewControl as ISkinable).ApplySkin(pNode, pSkinPath);

            if (functionNode != null) functionalProperties.ApplyFunction(
                 XmlHelper.GetFunction(functionNode.InnerText),
                 parameterAttr != null ? parameterAttr.Value : null,
                 viewControl);

            return viewControl;
        }

        public static D2DViewControl GetViewControl(string pName)
        {
            try
            {
                var viewControlType = Type.GetType(pName);
                if (viewControlType != null)
                    return Activator.CreateInstance(viewControlType) as D2DViewControl;
                else
                    DebugHandler.DebugPrint("ViewControl Type " + pName + " is not recognized");
            }
            catch (Exception ex)
            {
                DebugHandler.HandleException(ex);
            }

            return null;
        }
    }
}
