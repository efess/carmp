using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMp.ViewControls.OptionControls
{
    public class MediaLocation : D2DViewControl, IOptionControl, ISkinable
    {
        private const string OPTION_NAME = "Media Location";
        private const string OPTION_ELEMENT = "MediaLocation";

        private const string XPATH_MUSIC_FOLDER = "MusicFolder";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            Clear();

            XmlNode node = pXmlNode.SelectSingleNode(XPATH_MUSIC_FOLDER);
            if (node != null)
            {
                TextInput ti = new TextInput();
                ti.ApplySkin(node, pSkinPath);
                AddViewControl(ti);
                ti.StartRender();
            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
        }
    }
}
