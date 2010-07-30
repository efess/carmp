using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CarMp
{
    public static class SessionSettings
    {
        private const string XML_DATABASE_PATH_NODE = "databasepath";
        private const string XML_ROOT_NODE = "settings";

        private const string XML_SCREEN_SIZE_NODE = "resolution";

        private const string XML_WINDOW_LOCATION_NODE = "screenstart";

        private const string XML_XCOORD_ATTR = "xcoord";
        private const string XML_YCOORD_ATTR = "ycoord";

        private const string XML_MUSIC_FOLDER = "MusicLocation";
        private const string XML_VIDEO_FOLDER = "VideoLocatin";
        private const string XML_PICTURES_FOLDER = "PictureLocation";
        private const string XML_SKINS_FOLDER = "SkinsLocatin";

        private const string XML_SKIN_NAME = "CurrentSkin";

        /// <summary>
        /// Indicates if application should run debugging methods
        /// </summary>
        public static bool Debug = false;
        /// <summary>
        /// Location of database
        /// </summary>
        public static string DatabaseLocation = @".\database.db";
        /// <summary>
        /// Location of file containing these settinsg (overridable by the command line)
        /// </summary>
        public static string SettingsXmlLocation = @".\settings.xml";
        /// <summary>
        /// Start location of this screen
        /// </summary>
        public static Point2F WindowLocation = new Point2F(0, 0);
        /// <summary>
        /// Screen resolution (size of this window)
        /// </summary>
        public static SizeF ScreenResolution = new SizeF(640, 480);

        public static ColorF DefaultFontColor = new ColorF(198 / 256, 198 / 256, 198 / 256,1);
        public static ColorF DefaultFontSpecialColor = new ColorF(205 / 256, 117 / 256, 2 / 256, 1);

        public static string SkinName = "BMW";
        public static SkinSettings CurrentSkin = null;

        public static string SkinPath = @".\";        
        public static string MusicPath = @"C:\Music";
        public static string PicturePath = @"C:\Pictures";
        public static string VideoPath = @"C:\Video";

        public static string SettingsPath = @".\";

        public static string CurrentSkinPath
        {
            get { return System.IO.Path.Combine(SkinPath, SkinName); }
        }
        public static void SaveXml()
        {
            XDocument doc = new XDocument();

            if (File.Exists(pXmlFile))
            {
                try
                {
                    doc = XDocument.Load(pXmlFile);
                }
                catch (Exception ex)
                {
                    DebugHandler.HandleException(ex);
                }
            }
            else
                doc = CreateXmlDocument();
            

            XElement settings = doc.XPathSelectElement(XML_ROOT_NODE);

            if (settings == null)
            {
                DebugHandler.DebugPrint("Invalid settings file");
                return;
            }

            PopulateSettings(settings);

            try
            {
                doc.Save(pXmlFile);
            }
            catch (Exception ex)
            {
                DebugHandler.HandleException(ex);
            }
        }

        public static void LoadFromXml(string pXmlFile)
        {
            XDocument doc = new XDocument();
            try
            {
                doc = XDocument.Load(pXmlFile);
            }
            catch(Exception ex)
            {
                DebugHandler.HandleException(ex);
            }

            XElement settings = doc.XPathSelectElement(XML_ROOT_NODE);

            if (settings == null)
            {
                DebugHandler.DebugPrint("Invalid settings file");
            }
            else
                ExtractSettingsData(settings);
        }


        private static XDocument CreateXmlDocument()
        {
            XDocument xDoc = new XDocument();
            xDoc.Add(new XElement(XML_ROOT_NODE));

            return xDoc;
        }

        private static void PopulateSettings(XElement pXElement)
        {
            SetOrCreateNode(pXElement, XML_DATABASE_PATH_NODE, DatabaseLocation);
            SetOrCreateNode(pXElement, XML_MUSIC_FOLDER, MusicPath);
            SetOrCreateNode(pXElement, XML_VIDEO_FOLDER, VideoPath);
            SetOrCreateNode(pXElement, XML_PICTURES_FOLDER, PicturePath);
            SetOrCreateNode(pXElement, XML_SKINS_FOLDER, SkinPath);
            SetOrCreateNode(pXElement, XML_SKIN_NAME, SkinName);
            SetOrCreateNode(pXElement, XML_SCREEN_SIZE_NODE, new Action<XElement>(
                element => element.ReplaceAll(
                    new XAttribute("Width", ScreenResolution.Width),
                    new XAttribute("Height", ScreenResolution.Height))));
            SetOrCreateNode(pXElement, XML_WINDOW_LOCATION_NODE, new Action<XElement>(
                element => element.ReplaceAll(
                    new XAttribute("X", WindowLocation.X),
                    new XAttribute("Y", WindowLocation.Y))));
        }

        private static void SetOrCreateNode(XElement pXElement, string pNodeName, string pNodeValue)
        {
            SetOrCreateNode(pXElement, pNodeName, new Action<XElement>(element => element.Value = pNodeValue));
        }

        private static void SetOrCreateNode(XElement pXElement, string pNodeName, Action<XElement> pCreater)
        {
            XElement node = pXElement.XPathSelectElement(pNodeName);
            if (node == null)
            {
                pXElement.Add(node = new XElement(pNodeName));
            }
            pCreater(node);
        }

        private static void ExtractSettingsData(XElement pXElement)
        {
            foreach(XElement node in pXElement.DescendantNodes())
            {
                switch (node.Name.ToString())
                {
                    case XML_DATABASE_PATH_NODE:
                        DatabaseLocation = node.Value;
                        break;
                    case XML_WINDOW_LOCATION_NODE:
                        try
                        {
                            WindowLocation = new Point2F(Convert.ToInt32(node.Attribute(XML_XCOORD_ATTR).Value), Convert.ToInt32(node.Attribute(XML_YCOORD_ATTR).Value));
                        }
                        catch(Exception ex)
                        {
                            DebugHandler.DebugPrint("Invalid WindowLocation node");
                            DebugHandler.HandleException(ex);
                        }
                        break;
                    case XML_SCREEN_SIZE_NODE:
                        try
                        {
                            ScreenResolution = new SizeF(Convert.ToInt32(node.Attribute(XML_XCOORD_ATTR).Value), Convert.ToInt32(node.Attribute(XML_YCOORD_ATTR).Value));
                        }
                        catch (Exception ex)
                        {
                            DebugHandler.DebugPrint("Invalid ScreenResolution node");
                            DebugHandler.HandleException(ex);
                        }
                        break;
                    case XML_SKINS_FOLDER:
                        SkinPath = node.Value;
                        break;
                    case XML_SKIN_NAME:
                        SkinName = node.Value;
                        break;
                    case XML_MUSIC_FOLDER:
                        MusicPath = node.Value;
                        break;
                    case XML_VIDEO_FOLDER:
                        VideoPath = node.Value;
                        break;
                    case XML_PICTURES_FOLDER:
                        PicturePath = node.Value;
                        break;
                }
                
            }
        }
        public void LoadCurrentSkin()
        {
            string path = CurrentSkinPath;
            System.IO.FileAttributes attr = System.IO.File.GetAttributes(path);
            // If this is a directory, append the filename
            if ((attr & System.IO.FileAttributes.Directory) != System.IO.FileAttributes.Directory)
            {
                path = System.IO.Path.GetDirectoryName(path);
            }

            // Load Skin now
            try
            {
                CurrentSkin = new SkinSettings(path);
            }
            catch (Exception ex)
            {
                DebugHandler.DebugPrint("Could not load skin from " + SkinPath + Environment.NewLine + "Exception Info: " + ex.ToString());
            }
        }
    }
}
