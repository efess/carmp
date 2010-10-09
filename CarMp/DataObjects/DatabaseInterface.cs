using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                if (!_DataSessions.ContainsKey(threadId))
                {
                    dataSession = Database.GetSession();
                    _DataSessions.Add(threadId, dataSession);
                }
                else
                    dataSession = _DataSessions[threadId];

                return dataSession;
            }
        }
    }
}
