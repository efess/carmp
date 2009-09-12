using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class XP_PROCs : DoTableCollection<XP_PROC>
	{
		protected override string TableName
		{
			get { return "XP_PROC"; }
		}
		
		
		
		private String _error;
		
		protected override XP_PROC ReadRecord(DataRow pRow)
		{
			XP_PROC xp_proc = new XP_PROC(false);
			
			return xp_proc;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class XP_PROC : DoTable
	{
		
		private String _error;
		
		public XP_PROC()
		{
			InitializeBase(true);
		}
		
		public XP_PROC(bool _new)
		{
			InitializeBase(_new);
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._table = "XP_PROC";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
		}
	}
}

