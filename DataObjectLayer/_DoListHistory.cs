using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class ListHistorys : DoTableCollection<ListHistory>
	{
		protected override string TableName
		{
			get { return "ListHistory"; }
		}
		
		private Int32 _HistoryId;
		private Int32 _ListIndex;
		private String _DisplayString;
		private Int32 _ItemType;
		private Int32 _ItemSpecialTarget;
		private String _ItemTarget;
		
		private Boolean _HistoryId_set;
		private Boolean _ListIndex_set;
		private Boolean _DisplayString_set;
		private Boolean _ItemType_set;
		private Boolean _ItemSpecialTarget_set;
		private Boolean _ItemTarget_set;
		
		private String _error;
		
		public Int32 HistoryId
		{
			get { return _HistoryId; }
			set { _HistoryId = value; _HistoryId_set = true; }
		}
		
		public Int32 ListIndex
		{
			get { return _ListIndex; }
			set { _ListIndex = value; _ListIndex_set = true; }
		}
		
		public String DisplayString
		{
			get { return _DisplayString; }
			set { _DisplayString = value; _DisplayString_set = true; }
		}
		
		public Int32 ItemType
		{
			get { return _ItemType; }
			set { _ItemType = value; _ItemType_set = true; }
		}
		
		public Int32 ItemSpecialTarget
		{
			get { return _ItemSpecialTarget; }
			set { _ItemSpecialTarget = value; _ItemSpecialTarget_set = true; }
		}
		
		public String ItemTarget
		{
			get { return _ItemTarget; }
			set { _ItemTarget = value; _ItemTarget_set = true; }
		}
		
		protected override ListHistory ReadRecord(DataRow pRow)
		{
			ListHistory listhistory = new ListHistory(false);
			listhistory.HistoryId = Int32.Parse((pRow["HistoryId"].ToString() == "" ? "0" : pRow["HistoryId"].ToString()));
			listhistory.ListIndex = Int32.Parse((pRow["ListIndex"].ToString() == "" ? "0" : pRow["ListIndex"].ToString()));
			listhistory.DisplayString = pRow["DisplayString"].ToString();
			listhistory.ItemType = Int32.Parse((pRow["ItemType"].ToString() == "" ? "0" : pRow["ItemType"].ToString()));
			listhistory.ItemSpecialTarget = Int32.Parse((pRow["ItemSpecialTarget"].ToString() == "" ? "0" : pRow["ItemSpecialTarget"].ToString()));
			listhistory.ItemTarget = pRow["ItemTarget"].ToString();
			
			return listhistory;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class ListHistory : DoTable
	{
		private Int32 _HistoryId;
		private Int32 _ListIndex;
		private String _DisplayString;
		private Int32 _ItemType;
		private Int32 _ItemSpecialTarget;
		private String _ItemTarget;
		
		private String _error;
		
		public ListHistory()
		{
			InitializeBase(true);
		}
		
		public ListHistory(bool _new)
		{
			InitializeBase(_new);
		}
		
		public Int32 HistoryId
		{
			get { return HistoryId; }
			set { _HistoryId = value; }
		}
		
		public Int32 ListIndex
		{
			get { return ListIndex; }
			set { _ListIndex = value; }
		}
		
		public String DisplayString
		{
			get { return DisplayString; }
			set { _DisplayString = value; }
		}
		
		public Int32 ItemType
		{
			get { return ItemType; }
			set { _ItemType = value; }
		}
		
		public Int32 ItemSpecialTarget
		{
			get { return ItemSpecialTarget; }
			set { _ItemSpecialTarget = value; }
		}
		
		public String ItemTarget
		{
			get { return ItemTarget; }
			set { _ItemTarget = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("HistoryId", DataType.Int, true, false, 4, false));
			base._cols.Add(new Column("ListIndex", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("DisplayString", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("ItemType", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("ItemSpecialTarget", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("ItemTarget", DataType.String, false, false, 1024, false));
			base._table = "ListHistory";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@HistoryId ", DbType.Int16, 4,"HistoryId").Value = _HistoryId.ToString();
			db.Parameters.Add("@ListIndex ", DbType.Int16, 4,"ListIndex").Value = _ListIndex.ToString();
			db.Parameters.Add("@DisplayString ", DbType.StringFixedLength, 1024,"DisplayString").Value = _DisplayString.ToString();
			db.Parameters.Add("@ItemType ", DbType.Int16, 4,"ItemType").Value = _ItemType.ToString();
			db.Parameters.Add("@ItemSpecialTarget ", DbType.Int16, 4,"ItemSpecialTarget").Value = _ItemSpecialTarget.ToString();
			db.Parameters.Add("@ItemTarget ", DbType.StringFixedLength, 1024,"ItemTarget").Value = _ItemTarget.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@HistoryId ", DbType.Int16, 4,"HistoryId").Value = _HistoryId.ToString();
		}
	}
}

