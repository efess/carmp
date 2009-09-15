using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	
	public class MediaGroupItems : DoTableCollection<MediaGroupItem>
	{
		protected override string TableName
		{
			get { return "MediaGroupItem"; }
		}
		
		
		
		private String _error;
		
		protected override MediaGroupItem ReadRecord(DataRow pRow)
		{
			MediaGroupItem mediagroupitem = new MediaGroupItem(false);
			
			return mediagroupitem;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class MediaGroupItem : DoTable
	{
		public static class Fields
		{
		}
		
		
		private String _error;
		
		public MediaGroupItem()
		{
			InitializeBase(true);
		}
		
		public MediaGroupItem(bool _new)
		{
			InitializeBase(_new);
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._table = "MediaGroupItem";
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

