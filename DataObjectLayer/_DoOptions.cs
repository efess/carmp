using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class Optionss : DoTableCollection<Options>
	{
		protected override string TableName
		{
			get { return "Options"; }
		}
		
		private String _key;
		private String _keyvalue;
		
		private Boolean _key_set;
		private Boolean _keyvalue_set;
		
		private String _error;
		
		public String key
		{
			get { return _key; }
			set { _key = value; _key_set = true; }
		}
		
		public String keyvalue
		{
			get { return _keyvalue; }
			set { _keyvalue = value; _keyvalue_set = true; }
		}
		
		protected override Options ReadRecord(DataRow pRow)
		{
			Options options = new Options(false);
			options.key = pRow["key"].ToString();
			options.keyvalue = pRow["keyvalue"].ToString();
			
			return options;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class Options : DoTable
	{
		private String _key;
		private String _keyvalue;
		
		private String _error;
		
		public Options()
		{
			InitializeBase(true);
		}
		
		public Options(bool _new)
		{
			InitializeBase(_new);
		}
		
		public String key
		{
			get { return key; }
			set { _key = value; }
		}
		
		public String keyvalue
		{
			get { return keyvalue; }
			set { _keyvalue = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("key", DataType.String, true, false, 1024, false));
			base._cols.Add(new Column("keyvalue", DataType.String, false, false, 1024, false));
			base._table = "Options";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@key ", DbType.StringFixedLength, 1024,"key").Value = _key.ToString();
			db.Parameters.Add("@keyvalue ", DbType.StringFixedLength, 1024,"keyvalue").Value = _keyvalue.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@key ", DbType.StringFixedLength, 1024,"key").Value = _key.ToString();
		}
	}
}

