using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	
	public class sqlite_sequences : DoTableCollection<sqlite_sequence>
	{
		protected override string TableName
		{
			get { return "sqlite_sequence"; }
		}
		
		private Int32 _seq;
		
		private Boolean _seq_set;
		
		private String _error;
		
		public Int32 seq
		{
			get { return _seq; }
			set { _seq = value; _seq_set = true; }
		}
		
		protected override sqlite_sequence ReadRecord(DataRow pRow)
		{
			sqlite_sequence sqlite_sequence = new sqlite_sequence(false);
			sqlite_sequence.seq = Int32.Parse((pRow["seq"].ToString() == "" ? "0" : pRow["seq"].ToString()));
			
			return sqlite_sequence;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class sqlite_sequence : DoTable
	{
		public static class Fields
		{
			public const string FIELD_SEQ = "seq";
		}
		
		private Int32 _seq;
		
		private String _error;
		
		public sqlite_sequence()
		{
			InitializeBase(true);
		}
		
		public sqlite_sequence(bool _new)
		{
			InitializeBase(_new);
		}
		
		public Int32 seq
		{
			get { return seq; }
			set { _seq = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("seq", DataType.Int, false, false, 4, false));
			base._table = "sqlite_sequence";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@seq ", DbType.Int16, 4,"seq").Value = _seq.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
		}
	}
}

