﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using DataObjectLayer;
using System.Collections.Generic;

namespace CarMp
{
    static class ApplicationMain
    {
        public static Dictionary<string, string> ApplicationOptions;

        public const string COMMANDLINE_DEBUG = "-DEBUG";
        public const string COMMANDLINE_XML_SETTINGS_PATH = "-settings";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] pArgs)
        {
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(HandleLowLevelException);

            DebugHandler.De += new DebugHandler.DebugException(dr => Console.WriteLine("EXCEPTION> " + dr.Message));
            DebugHandler.Ds += new DebugHandler.DebugString(Console.WriteLine);

            if (!CheckDuplicateExecution())
            {
                return;
            }

            InitializeApplication(pArgs);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormSplash());
            Application.Run(new Forms.FormHome());
        }

        private static void InitializeApplication(string[] pCommandLineArgs)
        {
            string xmlSettings = SessionSettings.SettingsXmlLocation;
            // Process any command line arguments
            for (int i = 0; i < pCommandLineArgs.Length; i++)
            {
                bool secondParm = i != pCommandLineArgs.Length - 1
                    && !pCommandLineArgs[i + 1].StartsWith("-");

                switch (pCommandLineArgs[i].ToUpper())
                {
                    case COMMANDLINE_DEBUG:
                        SessionSettings.Debug = true;
                        break;
                    case COMMANDLINE_XML_SETTINGS_PATH:
                        if (secondParm)
                        {
                            xmlSettings = pCommandLineArgs[i+1];
                            i++;
                        }
                        break;
                }
            }

            SessionSettings.LoadFromXml(xmlSettings);
            SQLiteCommon.Initialize(SessionSettings.DatabaseLocation);
            MediaManager.Initialize();           
        }

        private static bool CheckDuplicateExecution()
        {
            string proc = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by that name

            Process[] processes = Process.GetProcessesByName("carmp");

            // if there is more than one process...

            if (processes.Length > 1)
            {
                MessageBox.Show("Only one copy of CarMp can run at a time!");
                return false;
            }
            return true;
        }

        private static void HandleLowLevelException(object sender, UnhandledExceptionEventArgs e)
        {
            DebugHandler.HandleException((Exception)e.ExceptionObject);
        }
    }
}
