using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace CarMP
{
    public static class Database
    {
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

        public static ISession GetSession()
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
