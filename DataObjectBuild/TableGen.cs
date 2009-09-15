using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using DataObjectLayer;

namespace DataObjectBuild
{
    public class TableGen
    {
        private ArrayList Tables = new ArrayList();
        private StringBuilder c;
        private string _path = "";
        private int tab;
        
        public TableGen()
        {
            SQLiteCommon.Initialize(@"C:\source\CarMp\trunk\database.db");
        }

        public void Generate()
        {
            try
            {
                _path = @"C:\source\CarMp\trunk\DataObjectLayer\";
                string error;
                if (!SQLiteCommon.Initialized)
                    return;
                DataSet dsTables = SQLiteCommon.RunQuery("select * from sqlite_master where type = 'table'", out error);
                foreach (DataRow drTable in dsTables.Tables[0].Rows)
                {
                    Table tab = new Table();
                    tab.Name = drTable["name"].ToString();
                    DataSet dsRows = SQLiteCommon.RunQuery("PRAGMA Table_Info('" + tab.Name + "')", out error);
                    foreach (DataRow dsRow in dsRows.Tables[0].Rows)
                    {
                        String type = dsRow["type"].ToString();
                        Int32 size = -1;
                        if(type.IndexOf("varchar(")>-1)
                        {
                            size = Int32.Parse(type.Substring(type.IndexOf('(') +1,type.Length - 2 - type.IndexOf('(')));
                            tab.Cols.Add(new Column(dsRow["name"].ToString(), DataType.String, dsRow["pk"].ToString() == "1" ? true : false, false, size, dsRow["notnull"].ToString() == "1" ? true : false));
                        }
                        else if(type == "integer")
                        {
                            tab.Cols.Add(new Column(dsRow["name"].ToString(), DataType.Int, dsRow["pk"].ToString() == "1" ? true : false, false, 4, dsRow["notnull"].ToString() == "1" ? true : false));
                        }

                    }
                    Tables.Add(tab);
                }
                foreach (Table tb in Tables)
                {
                    c = new StringBuilder();
                    c.Append(T("using System;"));
                    c.Append(T("using System.Collections.Generic;"));
                    c.Append(T("using System.Linq;"));
                    c.Append(T("using System.Text;"));
                    c.Append(T("using System.Data;"));
                    c.Append(T("using System.Data.SQLite;"));
                    c.Append(T(""));
                    c.Append(T("namespace DataObjectLayer"));
                    c.Append(T("{"));
                    c.Append(T(""));
                    c.Append(T("public class " + tb.Name + "s : DoTableCollection<" + tb.Name + ">"));
                    c.Append(T("{"));
                    c.Append(T("protected override string TableName"));
                    c.Append(T("{"));
                    c.Append(T("get { return \"" + tb.Name + "\"; }"));
                    c.Append(T("}"));
                    c.Append(T(""));
                    foreach (Column col in tb.Cols)
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T("private String _" + col.Name + ";"));
                                break;
                            case DataType.Int:
                                c.Append(T("private Int32 _" + col.Name + ";"));
                                break;
                            case DataType.DateTime:
                                c.Append(T("private DateTime _" + col.Name + ";"));
                                break;
                            case DataType.Blob:
                                c.Append(T("private Byte[] _" + col.Name + ";"));
                                break;
                            case DataType.Long:
                                c.Append(T("private Double _" + col.Name + ";"));
                                break;
                            case DataType.Boolean:
                                c.Append(T("private Boolean _" + col.Name + ";"));
                                break;
                            default:
                                break;
                        }
                    }
                    c.Append(T(""));
                    foreach(Column col in tb.Cols)
                    {
                        c.Append(T("private Boolean _" + col.Name + "_set;"));
                    }
                    c.Append(T(""));
                    c.Append(T("private String _error;"));
                    c.Append(T(""));
                    foreach (Column col in tb.Cols)
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T("public String " + col.Name));
                                break;
                            case DataType.Int:
                                c.Append(T("public Int32 " + col.Name));
                                break;
                            case DataType.DateTime:
                                c.Append(T("public DateTime " + col.Name));
                                break;
                            case DataType.Blob:
                                c.Append(T("public Byte[] " + col.Name));
                                break;
                            case DataType.Long:
                                c.Append(T("public Double " + col.Name));
                                break;
                            case DataType.Boolean:
                                c.Append(T("public Boolean " + col.Name));
                                break;
                            default:
                                break;
                        }
                        c.Append(T("{"));
                        c.Append(T("get { return _" + col.Name + "; }"));
                        c.Append(T("set { _" + col.Name + " = value; _" + col.Name + "_set = true; }"));
                        c.Append(T("}"));
                        c.Append(T(""));
                    }
                    c.Append(T("protected override " + tb.Name + " ReadRecord(DataRow pRow)"));
                    c.Append(T("{"));
                    c.Append(T("" + tb.Name + " " + tb.Name.ToLower() + " = new " + tb.Name + "(false);"));
                    foreach (Column col in tb.Cols)
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T(tb.Name.ToLower() + "." + col.Name + " = pRow[\"" + col.Name + "\"].ToString();"));
                                break;
                            case DataType.Int:
                                c.Append(T(tb.Name.ToLower() + "." + col.Name + " = Int32.Parse((pRow[\"" + col.Name + "\"].ToString() == \"\" ? \"0\" : pRow[\"" + col.Name + "\"].ToString()));"));
                                break;
                            case DataType.DateTime:
                                c.Append(T(tb.Name.ToLower() + "." + col.Name + " = DateTime.Parse(pRow[\"" + col.Name + "\"].ToString());"));
                                break;
                            case DataType.Blob:
                                c.Append(T("public Byte[] " + col.Name + ";"));
                                throw new NotImplementedException();
                            case DataType.Long:
                                c.Append(T("public Double " + col.Name + ";"));
                                throw new NotImplementedException();
                            case DataType.Boolean:
                                c.Append(T("public Boolean " + col.Name + ";"));
                                throw new NotImplementedException();
                            default:
                                break;
                        }
                    }
                    c.Append(T(""));
                    c.Append(T("return " + tb.Name.ToLower() + ";"));
                    c.Append(T("}"));
                    c.Append(T(""));
                    c.Append(T("public bool Remove()"));
                    c.Append(T("{"));
                    c.Append(T(""));
                    c.Append(T("return true;"));
                    c.Append(T("}"));
                    c.Append(T("}"));
                    c.Append(T(""));



///////////////////////////////////////////////////////////////////////////////
/*****************************************************************************/
///////////////////////////////////////////////////////////////////////////////



                    c.Append(T("public class " + tb.Name + " : DoTable"));
                    c.Append(T("{"));
                    c.Append(T("public static class Fields"));
                    c.Append(T("{"));
                    foreach (Column col in tb.Cols)
                    {
                        c.Append(T("public const string FIELD_" + col.Name.ToUpper() + " = \"" + col.Name + "\";"));
                    }
                    c.Append(T("}"));
                    c.Append(T(""));
                    foreach (Column col in tb.Cols)
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T("private String _" + col.Name + ";"));
                                break;
                            case DataType.Int:
                                c.Append(T("private Int32 _" + col.Name + ";"));
                                break;
                            case DataType.DateTime:
                                c.Append(T("private DateTime _" + col.Name + ";"));
                                break;
                            case DataType.Blob:
                                c.Append(T("private Byte[] _" + col.Name + ";"));
                                break;
                            case DataType.Long:
                                c.Append(T("private Double _" + col.Name + ";"));
                                break;
                            case DataType.Boolean:
                                c.Append(T("private Boolean _" + col.Name + ";"));
                                break;
                            default:
                                break;
                        }
                    }
                    c.Append(T(""));
                    c.Append(T("private String _error;"));
                    c.Append(T(""));
                    c.Append(T("public " + tb.Name + "()"));
                    c.Append(T("{"));
                    c.Append(T("InitializeBase(true);"));
                    c.Append(T("}"));
                    c.Append(T(""));
                    c.Append(T("public " + tb.Name + "(bool _new)"));
                    c.Append(T("{"));
                    c.Append(T("InitializeBase(_new);"));
                    c.Append(T("}"));
                    c.Append(T(""));
                    foreach (Column col in tb.Cols)
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T("public String " + col.Name));
                                break;
                            case DataType.Int:
                                c.Append(T("public Int32 " + col.Name));
                                break;
                            case DataType.DateTime:
                                c.Append(T("public DateTime " + col.Name));
                                break;
                            case DataType.Blob:
                                c.Append(T("public Byte[] " + col.Name));
                                break;
                            case DataType.Long:
                                c.Append(T("public Double " + col.Name));
                                break;
                            case DataType.Boolean:
                                c.Append(T("public Boolean " + col.Name));
                                break;
                            default:
                                break;
                        }
                        c.Append(T("{"));
                        c.Append(T("get { return " + col.Name + "; }"));
                        c.Append(T("set { _" + col.Name + " = value; }"));
                        c.Append(T("}"));
                        c.Append(T(""));
                    }
                    c.Append(T("private void InitializeBase(bool _new)"));
                    c.Append(T("{"));
                    c.Append(T("base._cols = new Columns();"));
                    foreach (Column col in tb.Cols)
                    {
                        c.Append(T("base._cols.Add(new Column(\"" + col.Name + "\", DataType." + col.Type.ToString() + ", " + col.Key.ToString().ToLower() + ", " + col.AutoNumber.ToString().ToLower() + ", " + col.Length.ToString() + ", " + col.AllowNull.ToString().ToLower() + "));"));
                    }
                    c.Append(T("base._table = \"" + tb.Name + "\";"));
                    c.Append(T("base._new = _new;"));
                    c.Append(T(""));
                    c.Append(T("//Generate SQL strings"));
                    c.Append(T("SQLGen.CreateInsertParms(this);"));
                    c.Append(T("SQLGen.CreateUpdateParms(this);"));
                    c.Append(T("SQLGen.CreateDeleteParms(this);"));
                    c.Append(T("}"));
                    c.Append(T(""));
                    c.Append(T("internal override void GetWriteParms(SQLiteCommand db)"));
                    c.Append(T("{"));
                    foreach (Column col in tb.Cols)
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.StringFixedLength, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ".ToString();"));
                                break;
                            case DataType.Int:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Int16, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ".ToString();"));
                                break;
                            case DataType.DateTime:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.DateTime, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ".ToString();"));
                                break;
                            case DataType.Blob:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Byte, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ";"));
                                break;
                            case DataType.Long:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Double, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ";"));
                                break;
                            case DataType.Boolean:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Boolean, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ";"));
                                break;
                            default:
                                break;
                        }
                    }
                    
                    c.Append(T("}"));
                    c.Append(T("internal override void GetDeleteParms(SQLiteCommand db)"));
                    c.Append(T("{"));
                    foreach (Column col in tb.Cols.GetKeys())
                    {
                        switch (col.Type)
                        {
                            case DataType.String:
                            case DataType.Text:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.StringFixedLength, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ".ToString();"));
                                break;
                            case DataType.Int:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Int16, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ".ToString();"));
                                break;
                            case DataType.DateTime:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.DateTime, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ".ToString();"));
                                break;
                            case DataType.Blob:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Byte, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ";"));
                                break;
                            case DataType.Long:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Double, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ";"));
                                break;
                            case DataType.Boolean:
                                c.Append(T("db.Parameters.Add(\"@" + col.Name + " \", DbType.Boolean, " + col.Length + ",\"" + col.Name + "\").Value = _" + col.Name + ";"));
                                break;
                            default:
                                break;
                        }
                    }
                    c.Append(T("}"));
                    c.Append(T("}"));
                    c.Append(T("}"));
                    c.Append(T(""));

                    string stringPath = _path + "_Do" + tb.Name + ".cs";
                    
                    FileStream fs = new FileStream(stringPath, FileMode.Create);
                    fs.Write(Encoding.ASCII.GetBytes(c.ToString()), 0, Encoding.ASCII.GetByteCount(c.ToString()));
                    fs.Close();

                    Console.WriteLine("Wrote table class - " + stringPath);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private string T(string str)
        {
            int __tab = tab;
            str =  str + "\n";
            string __str = str.PadLeft(str.Length + tab, '\t');
            if (str.IndexOf('}') > -1)
                tab--;
            
            if (str.IndexOf('{') > -1)
                tab++;

            if (tab + 1 == __tab)
            {
                __str = str.PadLeft(str.Length + tab, '\t');
            }
            return __str;

        }

        public class Table
        {
            public string Name;
            public Columns Cols;
            public Table()
            {
                Cols = new Columns();
            }
        }
   
    }
}
