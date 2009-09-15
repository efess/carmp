using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

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
        public static Point WindowLocation = new Point(0, 0);
        /// <summary>
        /// Screen resolution (size of this window)
        /// </summary>
        public static Size ScreenResolution = new Size(640, 480);

        public static void LoadFromXml(string pXmlFile)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(pXmlFile);
            }
            catch(Exception ex)
            {
                return;
            }

            XmlNode settings = doc.SelectSingleNode(XML_ROOT_NODE);

            if (settings == null)
            {
                DebugHandler.DebugPrint("Invalid settings file");
            }

            foreach (XmlNode node in settings.ChildNodes)
            {
                try
                {
                    switch (node.Name)
                    {
                        case XML_DATABASE_PATH_NODE:
                            DatabaseLocation = node.InnerText;
                            break;
                        case XML_WINDOW_LOCATION_NODE:
                            WindowLocation = new Point(Convert.ToInt32(node.Attributes[XML_XCOORD_ATTR].Value), Convert.ToInt32(node.Attributes[XML_YCOORD_ATTR].Value));
                            break;
                        case XML_SCREEN_SIZE_NODE:
                            ScreenResolution = new Size(Convert.ToInt32(node.Attributes[XML_XCOORD_ATTR].Value), Convert.ToInt32(node.Attributes[XML_YCOORD_ATTR].Value));
                            break;
                    }
                }
                catch
                {
                    DebugHandler.DebugPrint("Invalid node: " + node.Name);
                }
            }
        }
    }
}
