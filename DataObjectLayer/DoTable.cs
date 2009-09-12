using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace DataObjectLayer
{
    public abstract class DoTable
    {
        protected string _table;
        protected Columns _cols;
        protected Boolean _new;
        private string _errorString;

        internal abstract void GetWriteParms(SQLiteCommand pCommand);
        internal abstract void GetDeleteParms(SQLiteCommand pCommand);

        public String ErrorString
        {
            get { return _errorString; }
        }

        // SQL
        protected string _insert;
        protected string _update;
        protected string _delete;

        internal Columns Cols
        {
            get { return _cols; }
        }

        protected Boolean New
        {
            get { return _new; }
        }

        internal string Table
        {
            get { return _table; }
        }

        internal string InsertSql
        {
            set { if (String.IsNullOrEmpty(_insert)) _insert = value; }
            get { return _insert; }
        }

        internal string UpdateSql
        {
            set { if (String.IsNullOrEmpty(_update)) _update = value; }
            get { return _update; }
        }

        internal string DeleteSql
        {
            set { if (String.IsNullOrEmpty(_delete)) _delete = value; }
            get { return _delete; }
        }

        protected string WriteSQL()
        {
            if (_new)
                return _insert;
            else
                return _update;
        }

        public bool Delete()
        {
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {

                SQLiteCommand dbc = new SQLiteCommand();
                try
                {
                    dbc.Connection = SQLiteCommon.GetConnection();
                    dbc.CommandText = _delete;
                    GetDeleteParms(dbc);
                    dbc.ExecuteNonQuery();
                    dbc.Connection.Close();
                }
                catch (Exception e)
                {
                    _errorString = e.Message;
                }

                if (!String.IsNullOrEmpty(_errorString))
                {

                    return false;
                }
                else
                {
                    ts.Complete();
                    return true;
                }
            }
        }

        public bool Write()
        {
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {

                SQLiteCommand dbc = new SQLiteCommand();
                try
                {
                    dbc.Connection = SQLiteCommon.GetConnection();
                    dbc.CommandText = WriteSQL();
                    GetWriteParms(dbc);
                    dbc.ExecuteNonQuery();
                    dbc.Connection.Close();
                }
                catch (Exception e)
                {
                    _errorString = e.Message;
                }

                if (!String.IsNullOrEmpty(_errorString))
                {
                    
                    return false;
                }
                else
                {
                    ts.Complete();
                    return true;
                }
            }
        }
    }
}
