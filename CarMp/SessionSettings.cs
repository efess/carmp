using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;

namespace CarMP
{
    public static class SessionSettings
    {
        private const string XML_DATABASE_PATH_NODE = "databasepath";
        private const string XML_ROOT_NODE = "settings";

        private const string XML_SCREEN_SIZE_NODE = "resolution";

        private const string XML_WINDOW_LOCATION_NODE = "screenstart";

        private const string XML_XCOORD_ATTR = "X";
        private const string XML_YCOORD_ATTR = "Y";
        private const string XML_WIDTH_ATTR = "Width";
        private const string XML_HEIGHT_ATTR = "Height";

        private const string XML_MUSIC_FOLDER = "MusicLocation";
        private const string XML_VIDEO_FOLDER = "VideoLocatin";
        private const string XML_PICTURES_FOLDER = "PictureLocation";
        private const string XML_SKINS_FOLDER = "SkinsLocatin";

        private const string XML_SKIN_NAME = "CurrentSkin";
        //  Option Properties
        /// <summary>
        /// Indicates if application should run debugging methods
        /// </summary>
        public static bool Debug { get; set; }
        /// <summary>
        /// Location of database
        /// </summary>
        public static string DatabaseLocation { get;  set; }
        /// <summary>
        /// Location of file containing these settinsg (overridable by the command line)
        /// </summary>
        public static string SettingsXmlLocation { get; set; }
        /// <summary>
        /// Start location of this screen
        /// </summary>
        public static Point2F WindowLocation {get;set;}
        /// <summary>
        /// Screen resolution (size of this window)
        /// </summary>
        public static SizeF ScreenResolution { get;  set; }

        public static ColorF DefaultFontColor { get;  set; }
        public static ColorF DefaultFontSpecialColor { get;  set; }

        public static string SkinName { get;  set; }
        public static SkinSettings CurrentSkin = null;

        public static string SkinPath { get;  set; }
        public static string MusicPath { get;  set; }
        public static string PicturePath { get;  set; }
        public static string VideoPath { get;  set; }

        public static string SettingsPath = @".\";

        private static string settingsFile = null;

        public static string CurrentSkinPath
        {
            get { return System.IO.Path.Combine(SkinPath, SkinName); }
        }

        public static void SetDefault()
        {
            Debug = false;
            DatabaseLocation = @".\database.db";
            SettingsXmlLocation = @".\settings.xml";
            WindowLocation = new Point2F(0, 0);
            ScreenResolution = new SizeF(640, 480);
            DefaultFontColor = new ColorF(198 / 256, 198 / 256, 198 / 256,1);
            DefaultFontSpecialColor = new ColorF(205 / 256, 117 / 256, 2 / 256, 1);
            SkinName = "BMW";
            SkinPath = @".\";        
            MusicPath = @"C:\Music";
            PicturePath = @"C:\Pictures";
            VideoPath = @"C:\Video";
            
        }

        public static void SaveXml()
        {
            if(settingsFile == null)
            {
                DebugHandler.DebugPrint("Attempt to Save Settings: Load with an Xml File Path must be called before Save");
                return;
            }

            XDocument doc = new XDocument();

            if (File.Exists(settingsFile))
            {
                try
                {
                    doc = XDocument.Load(settingsFile);
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
                doc.Save(settingsFile);
            }
            catch (Exception ex)
            {
                DebugHandler.HandleException(ex);
            }
        }

        public static void LoadFromXml(string pXmlFile)
        {
            settingsFile = pXmlFile;
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
            {
                ExtractSettingsData(settings);
                LoadCurrentSkin();
            }
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
            foreach(XElement node in pXElement.DescendantNodes().Where(e => e is XElement))
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
                            ScreenResolution = new SizeF(Convert.ToInt32(node.Attribute(XML_WIDTH_ATTR).Value), Convert.ToInt32(node.Attribute(XML_HEIGHT_ATTR).Value));
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

        public static void LoadCurrentSkin()
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
