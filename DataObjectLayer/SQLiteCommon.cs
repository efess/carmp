using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
    public class SQLiteCommon
    {
        private static Boolean m_Initialized;
        private static string ConnectionString;
        public static SQLiteConnection oc;
        private static object lockobject = new object();

        public static Boolean Initialized
        {
            get
            {
                return m_Initialized;
            }
        }

        public static bool Initialize(string DBPath)
        {
            try
            {
                ConnectionString = "Version=3;Compress=True;Data Source=" + DBPath;
                oc = new SQLiteConnection(ConnectionString);
                oc.Open();
                oc.Close();
                m_Initialized = true;
                
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static SQLiteConnection GetConnection()
        {
            oc.Open();
            return oc;
        }



        public static DataSet RunQuery(string Query, out string error)
        {
            lock (lockobject)
            {
                try
                {
                    SQLiteCommand Comm = new SQLiteCommand(Query, oc);
                    SQLiteDataAdapter Adapter = new SQLiteDataAdapter(Comm);
                    DataSet ds = new DataSet();
                    Adapter.Fill(ds);
                    oc.Close();

                    error = "";
                    return ds;
                }
                catch(Exception e)
                {
                    error = e.Message;
                    oc.Close();
                    return null;
                }
            }
        }

        public static string FixQuote(string pString)
        {
            if (pString != null)
            {
                return pString.Replace("'", "''");
            }
            return null;
        }

        public static string Quoted(string pString) {return "'" + pString + "'";}

    }

}
