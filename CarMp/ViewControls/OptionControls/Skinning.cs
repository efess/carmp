using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls.OptionControls
{
    public class Skinning : ViewControlCommonBase, ISkinable, IOptionControl
    {
        private const string OPTION_NAME = "Skinning";
        private const string OPTION_ELEMENT = "Skinning";

        private const string XPATH_APPLY_SKIN = "ApplySkin";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        private DragableList skinList;

        public override void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            base.ApplySkin(pXmlNode, pSkinPath);

            XmlNode node = pXmlNode.SelectSingleNode(XPATH_APPLY_SKIN);
            if(node != null)
            {
                GraphicalButton button = new GraphicalButton();
                button.ButtonString = "Apply Skin";
                button.ApplySkin(node, pSkinPath);

                AddViewControl(button);
                button.StartRender();
                button.Click += (sender, e) => { AppMain.WindowManager.ApplySkin(); };
            }


        }

        protected override void OnRender(IRenderer pRenderer)
        {
            base.OnRender(pRenderer);
        }

    }
}
