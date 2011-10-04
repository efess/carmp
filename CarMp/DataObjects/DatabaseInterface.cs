using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NHibernate.Tool.hbm2ddl;

namespace CarMP.DataObjects
{
    public static class DatabaseInterface
    {

        // This may be dangerous... Throughout the lifetime of the application 
        // datasessions will be created for each threadid, and never closed.
        // Will have to see what happens...

        // This started out as bad design, and this is mostly a bandaid.

        private static Dictionary<int, NHibernate.ISession> _DataSessions
             = new Dictionary<int, NHibernate.ISession>();

        public static NHibernate.ISession DataSession
        {
            get
            {
                NHibernate.ISession dataSession;
                int threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
                if (!_DataSessions.ContainsKey(threadId)
                    || !_DataSessions[threadId].IsOpen)
                {
                    dataSession = GetSession();
                    _DataSessions[threadId]=  dataSession;
                }
                else
                    dataSession = _DataSessions[threadId];

                return dataSession;
            }
        }
        private static ISessionFactory _SessionFactory;
        public static void InitializeDatabase(string pDatabaseFile)
        {
            string configurationFile = "Configuration.serialized";
            
            Configuration cfg = null;
            IFormatter serializer = new BinaryFormatter();
            
            if(File.Exists(configurationFile))
            {
                //other times
                using (Stream stream = File.OpenRead(configurationFile))
                {
                    cfg = serializer.Deserialize(stream) as Configuration;
                }
            }
            else
            {
                cfg = Fluently.Configure()
                      .Database(
                        SQLiteConfiguration.Standard
                          .UsingFile(pDatabaseFile)
                      )
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<AppMain>())
                        .BuildConfiguration();
                
                using (Stream stream = File.OpenWrite(configurationFile))
                {
                    serializer.Serialize(stream, cfg);
                }
            }
             

            _SessionFactory = 
                cfg.BuildSessionFactory();

            // new SchemaUpdate(cfg).Execute(false, true);
        }

        private static void CreateSchema()
        { 
        }

        private static bool InitializedCheck()
        {
            if (_SessionFactory == null)
            {
                throw new Exception("Database access attempt before initializing database");
            }
            return true;
        }

        private static ISession GetSession()
        {
            if (InitializedCheck())
            {
                return _SessionFactory.OpenSession();
            }
            else
                throw new Exception("Database access attempt before initializing database");
        }
    }
}
