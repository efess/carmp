using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Interfaces;

namespace CarMP.ViewControls.OptionControls
{
    public class MediaLocation : D2DViewControl, IOptionControl, ISkinable
    {
        private const string OPTION_NAME = "Media Location";
        private const string OPTION_ELEMENT = "MediaLocation";

        private const string XPATH_MUSIC_FOLDER = "MusicFolder";
        private const string XPATH_PICTURE_FOLDER = "PictureFolder";
        private const string XPATH_VIDEO_FOLDER = "VideoFolder";
        private const string XPATH_REBUILD_DATABSE = "RebuildDatabase";

        public string OptionName { get { return OPTION_NAME; } }
        public string OptionElement { get { return OPTION_ELEMENT; } }

        public void ApplySkin(XmlNode pXmlNode, string pSkinPath)
        {
            Clear();


            XmlNode buttonNode = pXmlNode.SelectSingleNode(XPATH_REBUILD_DATABSE);

            if (buttonNode != null)
            {
                var button = new GraphicalButton();
                button.ButtonString = "Rebuild Database";
                button.ApplySkin(buttonNode, pSkinPath);

                AddViewControl(button);
                button.StartRender();
                button.Click += (sender, e) => { AppMain.BackgroundTasks.ScanMedia(true); };
            }

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
                                    if (AppMain.Settings.MusicPath != ti.TextString)
                                    {
                                        AppMain.Settings.MusicPath = ti.TextString;
                                        AppMain.Settings.SaveXml();
                                    }
                                };
                            ti.TextString = AppMain.Settings.MusicPath; break;
                        case XPATH_PICTURE_FOLDER:
                            ti.InputLeave += () =>
                            {
                                if (AppMain.Settings.PicturePath != ti.TextString)
                                {
                                    AppMain.Settings.PicturePath = ti.TextString;
                                    AppMain.Settings.SaveXml();
                                }
                            };
                            ti.TextString = AppMain.Settings.PicturePath; break;
                        case XPATH_VIDEO_FOLDER:
                            ti.InputLeave += () =>
                            {
                                if (AppMain.Settings.VideoPath != ti.TextString)
                                {
                                    AppMain.Settings.VideoPath = ti.TextString;
                                    AppMain.Settings.SaveXml();
                                }
                            };
                            ti.TextString = AppMain.Settings.VideoPath; break;
                    }
                }
            }
        }

        protected override void OnRender(IRenderer pRenderer)
        {
        }
    }
}
