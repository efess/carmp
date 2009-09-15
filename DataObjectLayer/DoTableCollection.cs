using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace DataObjectLayer
{
    public abstract class DoTableCollection<T> : List<T>
        where T : DoTable
    {
        protected string _errorString;
        public string ErrorString
        {
            get { return _errorString; }
        }
        protected abstract string TableName { get; }
        protected abstract T ReadRecord(DataRow pRow);

        /// <summary>
        /// Read without specifying query
        /// </summary>
        /// <returns>Entire contents of table</returns>
        public bool Read()
        {
            return Read(string.Empty);
        }
        /// <summary>
        /// Read using a DoQuery object
        /// </summary>
        /// <param name="pQueryExpression">Query containing constraints on what to return</param>
        /// <returns>Dataz</returns>
        public bool Read(DoQuery pQuery)
        {
            return Read(pQuery.GetSql());
        }
        /// <summary>
        /// Read using a string query
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Dataz</returns>
        public bool Read(String query)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT * FROM " + TableName);

            if (!string.IsNullOrEmpty(query))
            {
                sql.Append(" WHERE ")
                    .Append(query);
            }

            DataSet ds = SQLiteCommon.RunQuery(sql.ToString(), out _errorString);

            if (_errorString != "")
            {
                return false;
            }
            else
            {
                // Success
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    this.Add(ReadRecord(dr));
                }
                return true;
            }
        }        

        /// <summary>
        /// Attempts to commit all objects in collection
        /// </summary>
        /// <returns>True if successful, false if not with error stored in
        /// ErrorString</returns>
        public bool Write()
        {
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {
                try
                {
                    SQLiteCommand dbc = new SQLiteCommand();
                    dbc.Connection = SQLiteCommon.GetConnection();
                    foreach (DoTable dbObj in this)
                    {
                        dbObj.GetWriteParms(dbc);
                        dbc.ExecuteNonQuery();
                    }
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

        public bool Delete()
        {
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {
                try
                {
                    SQLiteCommand dbc = new SQLiteCommand();
                    dbc.Connection = SQLiteCommon.GetConnection();
                    foreach (DoTable dbObj in this)
                    {
                        dbc.CommandText = dbObj.DeleteSql;
                        dbObj.GetDeleteParms(dbc);
                        dbc.ExecuteNonQuery();
                    }
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
