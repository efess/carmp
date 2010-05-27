using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using NHibernate;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using System.Threading;

namespace CarMp
{
    public class ApplicationMain
    {
        private const int TIMER_GRANULARITY = 10;
        public static Forms.FormHost AppFormHost;

        public const string COMMANDLINE_DEBUG = "-DEBUG";
        public const string COMMANDLINE_XML_SETTINGS_PATH = "-settings";
        private readonly System.Timers.Timer _appTimer = new System.Timers.Timer(TIMER_GRANULARITY);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] pArgs)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(HandleLowLevelException);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);

            DebugHandler.De += new DebugHandler.DebugException(dr => Console.WriteLine("EXCEPTION> " + dr.Message));
            DebugHandler.Ds += new DebugHandler.DebugString(Console.WriteLine);

            if (CheckDuplicateProcess(Process.GetCurrentProcess().ProcessName))
            {
                MessageBox.Show("Only one copy of CarMp can run at a time!");
                return;
            }

            InitializeApplication(pArgs);

            AppFormHost = new Forms.FormHost();
            AppFormHost.ShowView(CarMp.Views.D2DViewFactory.HOME);
            AppFormHost.StartPosition = FormStartPosition.Manual;

            Application.Run(AppFormHost);
        }

        private static void InitializeApplication(string[] pCommandLineArgs)
        {
            Forms.FormSplash formSplash = new CarMp.Forms.FormSplash();
            formSplash.StartPosition = FormStartPosition.CenterScreen;

            Thread splashThread = new Thread(new ThreadStart(formSplash.ShowSplash));
            splashThread.Start();

            formSplash.IncreaseProgress(0, "Parsing command line args...");

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

            System.Threading.Thread.Sleep(100);
            formSplash.IncreaseProgress(10, "Loading settings XML...");

            SessionSettings.LoadFromXml(xmlSettings);

            System.Threading.Thread.Sleep(100);
            formSplash.IncreaseProgress(30, "Initializing Database...");
            Database.InitializeDatabase(SessionSettings.DatabaseLocation);

            formSplash.IncreaseProgress(80, "Initializing Media Manager...");
            System.Threading.Thread.Sleep(100);
            MediaManager.Initialize(new WinampController());

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
                m.FluentMappings.AddFromAssemblyOf<ApplicationMain>())
            .BuildSessionFactory();

            return sessionFactory.OpenSession();
        }

        public static bool CheckDuplicateProcess(string pProcessName)
        {
            // get the list of all processes by that name

            Process[] processes = Process.GetProcessesByName(pProcessName);

            // if there is more than one process...

            if (processes.Length > 1)
            {
                return true;
            }
            return false;
        }

        private static void HandleLowLevelException(object sender, UnhandledExceptionEventArgs e)
        {
            DebugHandler.HandleException((Exception)e.ExceptionObject);
        }

        private static void ProcessExit(object sender, EventArgs e)
        {
            MediaManager.Close();
        }
    }
}
