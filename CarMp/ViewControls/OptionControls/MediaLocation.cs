using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CarMP.ViewControls.OptionControls
{
    public class MediaLocation : D2DViewControl, IOptionControl, ISkinable
    {
        private const string OPTION_NAME = "Media Location";
        private const string OPTION_ELEMENT = "MediaLocation";

        private const string XPATH_MUSIC_FOLDER = "MusicFolder";
        private const string XPATH_PICTURE_FOLDER = "PictureFolder";
        private const string XPATH_VIDEO_FOLDER = "VideoFolder";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            Clear();

            var nodeNames = new string[] { XPATH_MUSIC_FOLDER, XPATH_PICTURE_FOLDER, XPATH_VIDEO_FOLDER };
            foreach (string nodeName in nodeNames)
            {
                XmlNode node = pXmlNode.SelectSingleNode(nodeName);
                if (node != null)
                {
                    TextInput ti = new TextInput();
                    ti.ApplySkin(node, pSkinPath);
                    AddViewControl(ti);
                    ti.StartRender();

                    switch (nodeName)
                    {
                        case XPATH_MUSIC_FOLDER:
                            ti.InputLeave += () => 
                                {
                                    if (SessionSettings.MusicPath != ti.TextString)
                                    {
                                        SessionSettings.MusicPath = ti.TextString;
                                        SessionSettings.SaveXml();
                                    }
                                };
                            ti.TextString = SessionSettings.MusicPath; break;
                        case XPATH_PICTURE_FOLDER:
                            ti.InputLeave += () =>
                            {
                                if (SessionSettings.PicturePath != ti.TextString)
                                {
                                    SessionSettings.PicturePath = ti.TextString;
                                    SessionSettings.SaveXml();
                                }
                            };
                            ti.TextString = SessionSettings.PicturePath; break;
                        case XPATH_VIDEO_FOLDER:
                            ti.InputLeave += () =>
                            {
                                if (SessionSettings.VideoPath != ti.TextString)
                                {
                                    SessionSettings.VideoPath = ti.TextString;
                                    SessionSettings.SaveXml();
                                }
                            };
                            ti.TextString = SessionSettings.VideoPath; break;
                    }
                }
            }
        }

        protected override void OnRender(Direct2D.RenderTargetWrapper pRenderTarget)
        {
        }
    }
}
