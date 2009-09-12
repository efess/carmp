using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class Albums : DoTableCollection<Album>
	{
		protected override string TableName
		{
			get { return "Album"; }
		}
		
		private Int32 _index;
		private String _AlbumName;
		private String _AlbumArtist;
		private String _picturepath;
		private String _picturesmallpath;
		
		private Boolean _index_set;
		private Boolean _AlbumName_set;
		private Boolean _AlbumArtist_set;
		private Boolean _picturepath_set;
		private Boolean _picturesmallpath_set;
		
		private String _error;
		
		public Int32 index
		{
			get { return _index; }
			set { _index = value; _index_set = true; }
		}
		
		public String AlbumName
		{
			get { return _AlbumName; }
			set { _AlbumName = value; _AlbumName_set = true; }
		}
		
		public String AlbumArtist
		{
			get { return _AlbumArtist; }
			set { _AlbumArtist = value; _AlbumArtist_set = true; }
		}
		
		public String picturepath
		{
			get { return _picturepath; }
			set { _picturepath = value; _picturepath_set = true; }
		}
		
		public String picturesmallpath
		{
			get { return _picturesmallpath; }
			set { _picturesmallpath = value; _picturesmallpath_set = true; }
		}
		
		protected override Album ReadRecord(DataRow pRow)
		{
			Album album = new Album(false);
			album.index = Int32.Parse((pRow["index"].ToString() == "" ? "0" : pRow["index"].ToString()));
			album.AlbumName = pRow["AlbumName"].ToString();
			album.AlbumArtist = pRow["AlbumArtist"].ToString();
			album.picturepath = pRow["picturepath"].ToString();
			album.picturesmallpath = pRow["picturesmallpath"].ToString();
			
			return album;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class Album : DoTable
	{
		private Int32 _index;
		private String _AlbumName;
		private String _AlbumArtist;
		private String _picturepath;
		private String _picturesmallpath;
		
		private String _error;
		
		public Album()
		{
			InitializeBase(true);
		}
		
		public Album(bool _new)
		{
			InitializeBase(_new);
		}
		
		public Int32 index
		{
			get { return index; }
			set { _index = value; }
		}
		
		public String AlbumName
		{
			get { return AlbumName; }
			set { _AlbumName = value; }
		}
		
		public String AlbumArtist
		{
			get { return AlbumArtist; }
			set { _AlbumArtist = value; }
		}
		
		public String picturepath
		{
			get { return picturepath; }
			set { _picturepath = value; }
		}
		
		public String picturesmallpath
		{
			get { return picturesmallpath; }
			set { _picturesmallpath = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("index", DataType.Int, true, false, 4, false));
			base._cols.Add(new Column("AlbumName", DataType.String, false, false, 100, false));
			base._cols.Add(new Column("AlbumArtist", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("picturepath", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("picturesmallpath", DataType.String, false, false, 1024, false));
			base._table = "Album";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
			db.Parameters.Add("@AlbumName ", DbType.StringFixedLength, 100,"AlbumName").Value = _AlbumName.ToString();
			db.Parameters.Add("@AlbumArtist ", DbType.StringFixedLength, 50,"AlbumArtist").Value = _AlbumArtist.ToString();
			db.Parameters.Add("@picturepath ", DbType.StringFixedLength, 1024,"picturepath").Value = _picturepath.ToString();
			db.Parameters.Add("@picturesmallpath ", DbType.StringFixedLength, 1024,"picturesmallpath").Value = _picturesmallpath.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
		}
	}
}

