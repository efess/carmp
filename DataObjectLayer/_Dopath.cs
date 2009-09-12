using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class paths : DoTableCollection<path>
	{
		protected override string TableName
		{
			get { return "path"; }
		}
		
		private String _Directory;
		private String _SubDirectory;
		
		private Boolean _Directory_set;
		private Boolean _SubDirectory_set;
		
		private String _error;
		
		public String Directory
		{
			get { return _Directory; }
			set { _Directory = value; _Directory_set = true; }
		}
		
		public String SubDirectory
		{
			get { return _SubDirectory; }
			set { _SubDirectory = value; _SubDirectory_set = true; }
		}
		
		protected override path ReadRecord(DataRow pRow)
		{
			path path = new path(false);
			path.Directory = pRow["Directory"].ToString();
			path.SubDirectory = pRow["SubDirectory"].ToString();
			
			return path;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class path : DoTable
	{
		private String _Directory;
		private String _SubDirectory;
		
		private String _error;
		
		public path()
		{
			InitializeBase(true);
		}
		
		public path(bool _new)
		{
			InitializeBase(_new);
		}
		
		public String Directory
		{
			get { return Directory; }
			set { _Directory = value; }
		}
		
		public String SubDirectory
		{
			get { return SubDirectory; }
			set { _SubDirectory = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("Directory", DataType.String, true, false, 1024, false));
			base._cols.Add(new Column("SubDirectory", DataType.String, true, false, 1024, false));
			base._table = "path";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@Directory ", DbType.StringFixedLength, 1024,"Directory").Value = _Directory.ToString();
			db.Parameters.Add("@SubDirectory ", DbType.StringFixedLength, 1024,"SubDirectory").Value = _SubDirectory.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@Directory ", DbType.StringFixedLength, 1024,"Directory").Value = _Directory.ToString();
			db.Parameters.Add("@SubDirectory ", DbType.StringFixedLength, 1024,"SubDirectory").Value = _SubDirectory.ToString();
		}
	}
}

