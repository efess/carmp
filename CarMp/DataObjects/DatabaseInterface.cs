using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

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
            _SessionFactory = Fluently.Configure()
              .Database(
                SQLiteConfiguration.Standard
                  .UsingFile(pDatabaseFile)
              )
            .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<AppMain>())
            .BuildSessionFactory();
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
