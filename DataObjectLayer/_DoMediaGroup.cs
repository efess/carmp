using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	
	public class MediaGroups : DoTableCollection<MediaGroup>
	{
		protected override string TableName
		{
			get { return "MediaGroup"; }
		}
		
		
		
		private String _error;
		
		protected override MediaGroup ReadRecord(DataRow pRow)
		{
			MediaGroup mediagroup = new MediaGroup(false);
			
			return mediagroup;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class MediaGroup : DoTable
	{
		public static class Fields
		{
		}
		
		
		private String _error;
		
		public MediaGroup()
		{
			InitializeBase(true);
		}
		
		public MediaGroup(bool _new)
		{
			InitializeBase(_new);
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._table = "MediaGroup";
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

