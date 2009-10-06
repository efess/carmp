using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	
	public class DigitalMediaSources : DoTableCollection<DigitalMediaSource>
	{
		protected override string TableName
		{
			get { return "DigitalMediaSource"; }
		}
		
		private String _DeviceId;
		private String _DeviceDescription;
		private String _Path;
		
		private Boolean _DeviceId_set;
		private Boolean _DeviceDescription_set;
		private Boolean _Path_set;
		
		private String _error;
		
		public String DeviceId
		{
			get { return _DeviceId; }
			set { _DeviceId = value; _DeviceId_set = true; }
		}
		
		public String DeviceDescription
		{
			get { return _DeviceDescription; }
			set { _DeviceDescription = value; _DeviceDescription_set = true; }
		}
		
		public String Path
		{
			get { return _Path; }
			set { _Path = value; _Path_set = true; }
		}
		
		protected override DigitalMediaSource ReadRecord(DataRow pRow)
		{
			DigitalMediaSource digitalmediasource = new DigitalMediaSource(false);
			digitalmediasource.DeviceId = pRow["DeviceId"].ToString();
			digitalmediasource.DeviceDescription = pRow["DeviceDescription"].ToString();
			digitalmediasource.Path = pRow["Path"].ToString();
			
			return digitalmediasource;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class DigitalMediaSource : DoTable
	{
		public static class Fields
		{
			public const string FIELD_DEVICEID = "DeviceId";
			public const string FIELD_DEVICEDESCRIPTION = "DeviceDescription";
			public const string FIELD_PATH = "Path";
		}
		
		private String _DeviceId;
		private String _DeviceDescription;
		private String _Path;
		
		private String _error;
		
		public DigitalMediaSource()
		{
			InitializeBase(true);
		}
		
		public DigitalMediaSource(bool _new)
		{
			InitializeBase(_new);
		}
		
		public String DeviceId
		{
			get { return DeviceId; }
			set { _DeviceId = value; }
		}
		
		public String DeviceDescription
		{
			get { return DeviceDescription; }
			set { _DeviceDescription = value; }
		}
		
		public String Path
		{
			get { return Path; }
			set { _Path = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("DeviceId", DataType.String, false, false, 256, false));
			base._cols.Add(new Column("DeviceDescription", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("Path", DataType.String, false, false, 1024, false));
			base._table = "DigitalMediaSource";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@DeviceId ", DbType.StringFixedLength, 256,"DeviceId").Value = _DeviceId.ToString();
			db.Parameters.Add("@DeviceDescription ", DbType.StringFixedLength, 1024,"DeviceDescription").Value = _DeviceDescription.ToString();
			db.Parameters.Add("@Path ", DbType.StringFixedLength, 1024,"Path").Value = _Path.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
		}
	}
}

