using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using NHibernate;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using System.Threading;
using CarMP.MediaController;
using CarMP.Settings;
using CarMP.Background;

namespace CarMP
{
    public class AppMain
    {
        private const int TIMER_GRANULARITY = 10;

        public static Forms.FormHost AppFormHost;
        public static BackgroundTasks BackgroundTasks { get; private set; }
        public static MediaManager MediaManager { get; private set; }
        public static SessionSettings Settings { get; private set; }
        private static Mutex singleAppMutex;
        public const string COMMANDLINE_DEBUG = "-DEBUG";
        public const string COMMANDLINE_XML_SETTINGS_PATH = "-settings";
        private readonly System.Timers.Timer _appTimer = new System.Timers.Timer(TIMER_GRANULARITY);
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main(string[] pArgs)
        {
            bool isNew = false;
            singleAppMutex = new Mutex(true, Application.ProductName, out isNew);

            BackgroundTasks = new Background.BackgroundTasks();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += HandleLowLevelException;
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;

            DebugHandler.De += new DebugHandler.DebugException(dr => Console.WriteLine("EXCEPTION> " + dr.Message));
            DebugHandler.Ds += new DebugHandler.DebugString(Console.WriteLine);

            if (!isNew)
            {
                MessageBox.Show("Only one copy of CarMP can run at a time!");
                return;
            }

            InitializeApplication(pArgs);

            AppFormHost = new Forms.FormHost();
            AppFormHost.ShowView(CarMP.Views.D2DViewFactory.HOME);
            AppFormHost.StartPosition = FormStartPosition.Manual;

            try
            {
                Application.Run(AppFormHost);
            }
            finally
            {
                singleAppMutex.ReleaseMutex();
            }
        }

        private static void InitializeApplication(string[] pCommandLineArgs)
        {
            Forms.FormSplash formSplash = new CarMP.Forms.FormSplash();
            formSplash.StartPosition = FormStartPosition.CenterScreen;

            Thread splashThread = new Thread(new ThreadStart(formSplash.ShowSplash));
            splashThread.Start();

            Settings = SessionSettings.GetSettingsObject();
            Settings.SetDefaults();
            string xmlSettings = Settings.SettingsXmlLocation;

            formSplash.IncreaseProgress(0, "Parsing command line args...");


            // Process any command line arguments
            for (int i = 0; i < pCommandLineArgs.Length; i++)
            {
                bool secondParm = i != pCommandLineArgs.Length - 1
                    && !pCommandLineArgs[i + 1].StartsWith("-");

                switch (pCommandLineArgs[i].ToUpper())
                {
                    case COMMANDLINE_DEBUG:
                        AppMain.Settings.Debug = true;
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

            System.Threading.Thread.Sleep(100);
            formSplash.IncreaseProgress(10, "Loading settings XML...");

            AppMain.Settings.LoadFromXml(xmlSettings);

            System.Threading.Thread.Sleep(100);
            formSplash.IncreaseProgress(30, "Initializing Database...");
            Database.InitializeDatabase(AppMain.Settings.DatabaseLocation);

            formSplash.IncreaseProgress(80, "Initializing Media Manager...");
            System.Threading.Thread.Sleep(100);
            MediaManager = new CarMP.MediaManager(new WinampController());

            formSplash.IncreaseProgress(100, "Done");
            formSplash.CloseSplash();
        }

        private static ISession CreateDbSession(string pDatabaseLocation)
        {
            ISessionFactory sessionFactory = Fluently.Configure()
              .Database(
                SQLiteConfiguration.Standard
                  .UsingFile(pDatabaseLocation)
              )
            .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<AppMain>())
            .BuildSessionFactory();

            return sessionFactory.OpenSession();
        }
        
        private static void HandleLowLevelException(object sender, UnhandledExceptionEventArgs e)
        {
            DebugHandler.HandleException((Exception)e.ExceptionObject);
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            AppMain.MediaManager.Close();
        }
    }
}
