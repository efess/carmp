using System;
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
        public const string COMMANDLINE_GENERATEDBCLASSES = "-GENCLASSES";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] pArgs)
        {
            if (!CheckDuplicateExecution())
            {
                return;
            }

            InitializeApplication(pArgs);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormSplash());
            Application.Run(new Forms.FormDigitalAudio());
        }

        private static void InitializeApplication(string[] pCommandLineArgs)
        {
            SQLiteCommon.Initialize(@"C:\source\CarMp\CarMp\database.db");
            MediaManager.Initialize();

            // Process any command line arguments
            for (int i = 0; i < pCommandLineArgs.Length; i++)
            {
                switch (pCommandLineArgs[i].ToUpper())
                {
                    case COMMANDLINE_DEBUG:
                        SessionSettings.Debug = true;
                        break;
                }
            }
           
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
    }
}
