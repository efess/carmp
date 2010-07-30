﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls.OptionControls
{
    public class Skinning : ViewControlCommonBase, ISkinable, IOptionControl
    {
        private const string OPTION_NAME = "Skinning";
        private const string OPTION_ELEMENT = "Skinning";

        private const string XPATH_APPLY_SKIN = "ApplySkin";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            Clear();

            XmlNode node = pXmlNode.SelectSingleNode(XPATH_APPLY_SKIN);
            if(node != null)
            {
                GraphicalButton button = new GraphicalButton();
                button.ButtonString = "Apply Skin";
                button.ApplySkin(node, pSkinPath);

                AddViewControl(button);
                button.StartRender();
                button.Click += (sender, e) => { AppMain.AppFormHost.ApplySkin(); };
            }
            base.ApplySkin(pXmlNode, pSkinPath);
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
            base.OnRender(pRenderTarget);
        }
    }
}