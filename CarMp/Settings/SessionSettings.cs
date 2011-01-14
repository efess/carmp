using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CarMP.Graphics.Geometry;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using CarMP.Skinning;
using CarMP.Graphics;

namespace CarMP.Settings
{
    public class SessionSettings : XmlSettingsBase, IXmlSettings
    {
        // singleton pattern
        private static SessionSettings instance;
        private SessionSettings()
        {
            InstantiateSetting<MediaDisplayFormatSettings>();
        }

        internal static SessionSettings GetSettingsObject()
        {
            if(instance == null)
                instance = new SessionSettings();
            return instance;
        }

        private List<IXmlSettings> SettingObjects = new List<IXmlSettings>();
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
        private const string XML_MEDIA_SORT = "MediaListSort";
        private const string XML_MEDIA_DISPLAY_FORMAT = "MediaDisplayFormat";

        private const string XML_SKIN_NAME = "CurrentSkin";

        //  Option Properties
        /// <summary>
        /// Indicates if application should run debugging methods
        /// </summary>
        public bool Debug { get; set; }
        /// <summary>
        /// Location of database
        /// </summary>
        public string DatabaseLocation { get;  set; }
        /// <summary>
        /// Location of file containing these settinsg (overridable by the command line)
        /// </summary>
        public string SettingsXmlLocation { get; set; }
        /// <summary>
        /// Start location of this screen
        /// </summary>
        public Point WindowLocation {get;set;}
        /// <summary>
        /// Screen resolution (size of this window)
        /// </summary>
        public Size ScreenResolution { get;  set; }

        public Color DefaultFontColor { get;  set; }
        public Color DefaultFontSpecialColor { get;  set; }

        public string SkinName { get;  set; }
        public Skin CurrentSkin { get; private set; }

        public string SkinPath { get;  set; }
        public string MusicPath { get;  set; }
        public string PicturePath { get;  set; }
        public string VideoPath { get;  set; }
        public MediaSort SortMedia { get; set; }

        public MediaDisplayFormatSettings DisplayFormat
        {
            get
            {
                return SettingObjects
                .OfType<MediaDisplayFormatSettings>()
                .FirstOrDefault();
            }
        }

        private IXmlSettings InstantiateSetting<T>() 
        {
            IXmlSettings setting = Activator.CreateInstance(typeof(T))
                as IXmlSettings;
            SettingObjects.Add(setting);
            setting.SetDefaults();
            return setting;
        }

        public string SettingsPath = @".\";

        private string settingsFile = null;

        public void SetDefaults()
        {
            Debug = false;
            DatabaseLocation = @".\database.db";
            SettingsXmlLocation = @".\settings.xml";
            WindowLocation = new Point(0, 0);
            ScreenResolution = new Size(800, 480);
            DefaultFontColor = new Color(198 / 256, 198 / 256, 198 / 256,1);
            DefaultFontSpecialColor = new Color(205 / 256, 117 / 256, 2 / 256, 1);
            SkinName = "BMW";
            SkinPath = @"..\..\..\Images\Skins";        
            MusicPath = @"C:\Music";
            PicturePath = @"C:\Pictures";
            VideoPath = @"C:\Video";
            SortMedia = MediaSort.FileName;
            SettingObjects.ForEach(so => so.SetDefaults());
        }

        public string ElementName { get { return XML_ROOT_NODE; } }

        public void SaveXml()
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
                    doc = CreateXmlDocument();
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

        public void LoadFromXml(string pXmlFile)
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
                SetDefaults();
            }
            else
            {
                ExtractSettings(settings);
            }
            LoadCurrentSkin();
        }

        private static XDocument CreateXmlDocument()
        {
            XDocument xDoc = new XDocument();
            xDoc.Add(new XElement(XML_ROOT_NODE));

            return xDoc;
        }

        public void PopulateSettings(XElement pXElement)
        {
            SetOrCreateNode(pXElement, XML_DATABASE_PATH_NODE, DatabaseLocation);
            SetOrCreateNode(pXElement, XML_MUSIC_FOLDER, MusicPath);
            SetOrCreateNode(pXElement, XML_VIDEO_FOLDER, VideoPath);
            SetOrCreateNode(pXElement, XML_PICTURES_FOLDER, PicturePath);
            SetOrCreateNode(pXElement, XML_SKINS_FOLDER, SkinPath);
            SetOrCreateNode(pXElement, XML_SKIN_NAME, SkinName);
            SetOrCreateNode(pXElement, XML_MEDIA_SORT, ((int)SortMedia).ToString());
            SetOrCreateNode(pXElement, XML_SCREEN_SIZE_NODE, new Action<XElement>(
                element => element.ReplaceAll(
                    new XAttribute("Width", ScreenResolution.Width),
                    new XAttribute("Height", ScreenResolution.Height))));
            SetOrCreateNode(pXElement, XML_WINDOW_LOCATION_NODE, new Action<XElement>(
                element => element.ReplaceAll(
                    new XAttribute("X", WindowLocation.X),
                    new XAttribute("Y", WindowLocation.Y))));
            
            SettingObjects.ForEach(xs => {
                EnsureNodeExistsOrCreate(pXElement, xs.ElementName);
                xs.PopulateSettings(pXElement.Element(xs.ElementName));
            });
        }

        public void ExtractSettings(XElement pXElement)
        {
            foreach(XElement node in pXElement.Elements().OfType<XElement>())
            {
                switch (node.Name.ToString())
                {
                    case XML_DATABASE_PATH_NODE:
                        DatabaseLocation = node.Value;
                        break;
                    case XML_WINDOW_LOCATION_NODE:
                        try
                        {
                            WindowLocation = new Point(Convert.ToInt32(node.Attribute(XML_XCOORD_ATTR).Value), Convert.ToInt32(node.Attribute(XML_YCOORD_ATTR).Value));
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
                            ScreenResolution = new Size(Convert.ToInt32(node.Attribute(XML_WIDTH_ATTR).Value), Convert.ToInt32(node.Attribute(XML_HEIGHT_ATTR).Value));
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
                    case XML_MEDIA_SORT:
                        try { SortMedia = (MediaSort)Enum.Parse(typeof(MediaSort), node.Value); }
                        catch { }; // Ignore.
                        break;
                    default:
                        SettingObjects.Where(xs => xs.ElementName == node.Name)
                            .ToList()
                            .ForEach(xs => xs.ExtractSettings(node));
                        break;
                }
            }
        }

        public void LoadCurrentSkin()
        {
            if (string.IsNullOrEmpty(SkinName))
            {
                DebugHandler.DebugPrint("No skin selected");
                return;
            }

            var skinManager = new SkinManager();
            skinManager.LoadSkins();
            var skin = skinManager.GetSkin(SkinName);

            if (skin == null)
            {
                DebugHandler.DebugPrint("Skin with name \"" + SkinName + "\" not found");
                return;
            }

            CurrentSkin = skin;
        }
    }
}
