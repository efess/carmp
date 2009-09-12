using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class librarys : DoTableCollection<library>
	{
		protected override string TableName
		{
			get { return "library"; }
		}
		
		private Int32 _index;
		private String _title;
		private String _relativepath;
		private String _fullpath;
		private String _root;
		private String _filename;
		private String _artist;
		private String _length;
		private Int32 _kbps;
		private String _monostereo;
		private String _album;
		private Int32 _frequency;
		private String _genre;
		private Int32 _rating;
		private Int32 _playcount;
		private String _filetype;
		private String _track;
		private String _containingfolder;
		
		private Boolean _index_set;
		private Boolean _title_set;
		private Boolean _relativepath_set;
		private Boolean _fullpath_set;
		private Boolean _root_set;
		private Boolean _filename_set;
		private Boolean _artist_set;
		private Boolean _length_set;
		private Boolean _kbps_set;
		private Boolean _monostereo_set;
		private Boolean _album_set;
		private Boolean _frequency_set;
		private Boolean _genre_set;
		private Boolean _rating_set;
		private Boolean _playcount_set;
		private Boolean _filetype_set;
		private Boolean _track_set;
		private Boolean _containingfolder_set;
		
		private String _error;
		
		public Int32 index
		{
			get { return _index; }
			set { _index = value; _index_set = true; }
		}
		
		public String title
		{
			get { return _title; }
			set { _title = value; _title_set = true; }
		}
		
		public String relativepath
		{
			get { return _relativepath; }
			set { _relativepath = value; _relativepath_set = true; }
		}
		
		public String fullpath
		{
			get { return _fullpath; }
			set { _fullpath = value; _fullpath_set = true; }
		}
		
		public String root
		{
			get { return _root; }
			set { _root = value; _root_set = true; }
		}
		
		public String filename
		{
			get { return _filename; }
			set { _filename = value; _filename_set = true; }
		}
		
		public String artist
		{
			get { return _artist; }
			set { _artist = value; _artist_set = true; }
		}
		
		public String length
		{
			get { return _length; }
			set { _length = value; _length_set = true; }
		}
		
		public Int32 kbps
		{
			get { return _kbps; }
			set { _kbps = value; _kbps_set = true; }
		}
		
		public String monostereo
		{
			get { return _monostereo; }
			set { _monostereo = value; _monostereo_set = true; }
		}
		
		public String album
		{
			get { return _album; }
			set { _album = value; _album_set = true; }
		}
		
		public Int32 frequency
		{
			get { return _frequency; }
			set { _frequency = value; _frequency_set = true; }
		}
		
		public String genre
		{
			get { return _genre; }
			set { _genre = value; _genre_set = true; }
		}
		
		public Int32 rating
		{
			get { return _rating; }
			set { _rating = value; _rating_set = true; }
		}
		
		public Int32 playcount
		{
			get { return _playcount; }
			set { _playcount = value; _playcount_set = true; }
		}
		
		public String filetype
		{
			get { return _filetype; }
			set { _filetype = value; _filetype_set = true; }
		}
		
		public String track
		{
			get { return _track; }
			set { _track = value; _track_set = true; }
		}
		
		public String containingfolder
		{
			get { return _containingfolder; }
			set { _containingfolder = value; _containingfolder_set = true; }
		}
		
		protected override library ReadRecord(DataRow pRow)
		{
			library library = new library(false);
			library.index = Int32.Parse((pRow["index"].ToString() == "" ? "0" : pRow["index"].ToString()));
			library.title = pRow["title"].ToString();
			library.relativepath = pRow["relativepath"].ToString();
			library.fullpath = pRow["fullpath"].ToString();
			library.root = pRow["root"].ToString();
			library.filename = pRow["filename"].ToString();
			library.artist = pRow["artist"].ToString();
			library.length = pRow["length"].ToString();
			library.kbps = Int32.Parse((pRow["kbps"].ToString() == "" ? "0" : pRow["kbps"].ToString()));
			library.monostereo = pRow["monostereo"].ToString();
			library.album = pRow["album"].ToString();
			library.frequency = Int32.Parse((pRow["frequency"].ToString() == "" ? "0" : pRow["frequency"].ToString()));
			library.genre = pRow["genre"].ToString();
			library.rating = Int32.Parse((pRow["rating"].ToString() == "" ? "0" : pRow["rating"].ToString()));
			library.playcount = Int32.Parse((pRow["playcount"].ToString() == "" ? "0" : pRow["playcount"].ToString()));
			library.filetype = pRow["filetype"].ToString();
			library.track = pRow["track"].ToString();
			library.containingfolder = pRow["containingfolder"].ToString();
			
			return library;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class library : DoTable
	{
		private Int32 _index;
		private String _title;
		private String _relativepath;
		private String _fullpath;
		private String _root;
		private String _filename;
		private String _artist;
		private String _length;
		private Int32 _kbps;
		private String _monostereo;
		private String _album;
		private Int32 _frequency;
		private String _genre;
		private Int32 _rating;
		private Int32 _playcount;
		private String _filetype;
		private String _track;
		private String _containingfolder;
		
		private String _error;
		
		public library()
		{
			InitializeBase(true);
		}
		
		public library(bool _new)
		{
			InitializeBase(_new);
		}
		
		public Int32 index
		{
			get { return index; }
			set { _index = value; }
		}
		
		public String title
		{
			get { return title; }
			set { _title = value; }
		}
		
		public String relativepath
		{
			get { return relativepath; }
			set { _relativepath = value; }
		}
		
		public String fullpath
		{
			get { return fullpath; }
			set { _fullpath = value; }
		}
		
		public String root
		{
			get { return root; }
			set { _root = value; }
		}
		
		public String filename
		{
			get { return filename; }
			set { _filename = value; }
		}
		
		public String artist
		{
			get { return artist; }
			set { _artist = value; }
		}
		
		public String length
		{
			get { return length; }
			set { _length = value; }
		}
		
		public Int32 kbps
		{
			get { return kbps; }
			set { _kbps = value; }
		}
		
		public String monostereo
		{
			get { return monostereo; }
			set { _monostereo = value; }
		}
		
		public String album
		{
			get { return album; }
			set { _album = value; }
		}
		
		public Int32 frequency
		{
			get { return frequency; }
			set { _frequency = value; }
		}
		
		public String genre
		{
			get { return genre; }
			set { _genre = value; }
		}
		
		public Int32 rating
		{
			get { return rating; }
			set { _rating = value; }
		}
		
		public Int32 playcount
		{
			get { return playcount; }
			set { _playcount = value; }
		}
		
		public String filetype
		{
			get { return filetype; }
			set { _filetype = value; }
		}
		
		public String track
		{
			get { return track; }
			set { _track = value; }
		}
		
		public String containingfolder
		{
			get { return containingfolder; }
			set { _containingfolder = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("index", DataType.Int, true, false, 4, false));
			base._cols.Add(new Column("title", DataType.String, false, false, 100, false));
			base._cols.Add(new Column("relativepath", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("fullpath", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("root", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("filename", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("artist", DataType.String, false, false, 100, false));
			base._cols.Add(new Column("length", DataType.String, false, false, 100, false));
			base._cols.Add(new Column("kbps", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("monostereo", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("album", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("frequency", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("genre", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("rating", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("playcount", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("filetype", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("track", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("containingfolder", DataType.String, false, false, 1024, false));
			base._table = "library";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
			db.Parameters.Add("@title ", DbType.StringFixedLength, 100,"title").Value = _title.ToString();
			db.Parameters.Add("@relativepath ", DbType.StringFixedLength, 1024,"relativepath").Value = _relativepath.ToString();
			db.Parameters.Add("@fullpath ", DbType.StringFixedLength, 1024,"fullpath").Value = _fullpath.ToString();
			db.Parameters.Add("@root ", DbType.StringFixedLength, 50,"root").Value = _root.ToString();
			db.Parameters.Add("@filename ", DbType.StringFixedLength, 1024,"filename").Value = _filename.ToString();
			db.Parameters.Add("@artist ", DbType.StringFixedLength, 100,"artist").Value = _artist.ToString();
			db.Parameters.Add("@length ", DbType.StringFixedLength, 100,"length").Value = _length.ToString();
			db.Parameters.Add("@kbps ", DbType.Int16, 4,"kbps").Value = _kbps.ToString();
			db.Parameters.Add("@monostereo ", DbType.StringFixedLength, 50,"monostereo").Value = _monostereo.ToString();
			db.Parameters.Add("@album ", DbType.StringFixedLength, 50,"album").Value = _album.ToString();
			db.Parameters.Add("@frequency ", DbType.Int16, 4,"frequency").Value = _frequency.ToString();
			db.Parameters.Add("@genre ", DbType.StringFixedLength, 50,"genre").Value = _genre.ToString();
			db.Parameters.Add("@rating ", DbType.Int16, 4,"rating").Value = _rating.ToString();
			db.Parameters.Add("@playcount ", DbType.Int16, 4,"playcount").Value = _playcount.ToString();
			db.Parameters.Add("@filetype ", DbType.StringFixedLength, 50,"filetype").Value = _filetype.ToString();
			db.Parameters.Add("@track ", DbType.StringFixedLength, 50,"track").Value = _track.ToString();
			db.Parameters.Add("@containingfolder ", DbType.StringFixedLength, 1024,"containingfolder").Value = _containingfolder.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
		}
	}
}

