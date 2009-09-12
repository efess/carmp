using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataObjectLayer
{
    public class SQLGen
    {
        public static void CreateInsert(DoTable pTable)
        {
            StringBuilder SQL = new StringBuilder("");
            StringBuilder Values = new StringBuilder("");
            bool first = true;
            int count = 0;
            SQL.Append("INSERT INTO " + pTable.Table + "(");
            Values.Append(" VALUES (");
            foreach (Column _c in pTable.Cols)
            {
                if(!_c.AutoNumber)
                {
                    if(first)
                    { 
                        first = false; 
                        SQL.Append("[" + _c.Name + "]");
                        Values.Append("{" + count++ + "}");
                    }
                    else 
                    { 
                        SQL.Append(", [" + _c.Name + "]");
                        Values.Append(",{" + count++ + "}");
                    }
                }
                              
            }
            SQL.Append(")");
            Values.Append( ")");

            pTable.InsertSql = SQL.Append(Values.ToString()).ToString();
        }

        public static void CreateUpdate(DoTable pTable)
        {
            StringBuilder SQL = new StringBuilder("");
            StringBuilder Where = new StringBuilder(" WHERE ");
            bool cfirst = true;
            bool kfirst = true;
            int count = 0;
            SQL.Append("UPDATE " + pTable.Table + " SET ");            
            foreach (Column _c in pTable.Cols)
            {
                if (!_c.AutoNumber && !_c.Key)
                {
                    if (cfirst)
                    {
                        cfirst = false;
                        SQL.Append("[" + _c.Name + "] = {" + count++ + "}");
                    }
                    else
                    {
                        SQL.Append(", [" + _c.Name + "] = {" + count++ + "}");
                    }
                }
                else
                {
                    if (kfirst)
                    {
                        kfirst = false;
                        Where.Append("[" + _c.Name + "] = {" + count++ + "}");
                    }
                    else
                    {
                        SQL.Append(" AND [" + _c.Name + "] = {" + count++ + "}");
                    }

                }
            }
            
            pTable.UpdateSql = SQL.Append(Where.ToString()).ToString();
        }

        public static void CreateInsertParms(DoTable pTable)
        {
            StringBuilder SQL = new StringBuilder("");
            StringBuilder Values = new StringBuilder("");
            bool first = true;
            int count = 0;
            SQL.Append("INSERT INTO " + pTable.Table + "(");
            Values.Append(" VALUES (");
            foreach (Column _c in pTable.Cols)
            {
                if (!_c.AutoNumber)
                {
                    if (first)
                    {
                        first = false;
                        SQL.Append("[" + _c.Name + "]");
                        Values.Append("@" + _c.Name);
                    }
                    else
                    {
                        SQL.Append(", [" + _c.Name + "]");
                        Values.Append(", @" + _c.Name);
                    }
                }

            }
            SQL.Append(")");
            Values.Append(")");

            pTable.InsertSql = SQL.Append(Values.ToString()).ToString();
        }

        public static void CreateUpdateParms(DoTable pTable)
        {
            StringBuilder SQL = new StringBuilder("UPDATE " + pTable.Table + " SET ");
            StringBuilder Where = new StringBuilder(" WHERE ");
            bool cfirst = true;
            bool kfirst = true;
            foreach (Column _c in pTable.Cols)
            {
                if (!_c.AutoNumber && !_c.Key)
                {
                    if (cfirst)
                    {
                        cfirst = false;
                    }
                    else
                    {
                        SQL.Append(", ");
                    }
                    SQL.Append("[" + _c.Name + "] = @" + _c.Name);
                }
                else
                {
                    if (kfirst)
                    {
                        kfirst = false;
                    }
                    else
                    {
                        Where.Append(" AND ");
                    }
                    Where.Append("[" + _c.Name + "] = @" + _c.Name);

                }
            }
            pTable.UpdateSql = SQL.Append(Where.ToString()).ToString();
        }

        public static void CreateDeleteParms(DoTable pTable)
        {
            StringBuilder SQL = new StringBuilder("DELETE FROM " + pTable.Table);
            StringBuilder where = new StringBuilder(" WHERE ");
            bool kfirst = true;
            foreach (Column _c in pTable.Cols.GetKeys())
            {
                if (kfirst)
                {
                    kfirst = false;
                }
                else
                {
                    where.Append(" AND ");
                }
                where.Append("[" + _c.Name + "] = @" + _c.Name);
            }
            pTable.DeleteSql = where.ToString();
        }

        public static string FormateSQL(DataType pType, object pStr)
        {
            switch (pType)
            {
                case DataType.String:
                    return "'" + FixQuote(pStr.ToString()) + "'";
                case DataType.Int:
                    return pStr.ToString();
                case DataType.Boolean:
                    return pStr.ToString();
                case DataType.Blob:
                    //TODO, implement this
                    return pStr.ToString();
                case DataType.DateTime:
                    return "'" + pStr.ToString() + "'";
                default:
                    return pStr.ToString();
            }
        }

        private static string FixQuote(string pStr)
        {
            return pStr.Replace("'","''");
        }
    }
}
