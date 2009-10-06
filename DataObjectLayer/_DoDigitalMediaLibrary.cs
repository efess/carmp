using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	
	public class DigitalMediaLibrarys : DoTableCollection<DigitalMediaLibrary>
	{
		protected override string TableName
		{
			get { return "DigitalMediaLibrary"; }
		}
		
		private String _DeviceId;
		private String _Path;
		private String _FileName;
		private String _Artist;
		private String _Title;
		private String _Album;
		private Int32 _Kbps;
		private Int32 _Channels;
		private Int32 _Frequency;
		private String _Genre;
		private Int32 _Rating;
		private Int32 _Playcount;
		private String _FileExtension;
		private String _Track;
		
		private Boolean _DeviceId_set;
		private Boolean _Path_set;
		private Boolean _FileName_set;
		private Boolean _Artist_set;
		private Boolean _Title_set;
		private Boolean _Album_set;
		private Boolean _Kbps_set;
		private Boolean _Channels_set;
		private Boolean _Frequency_set;
		private Boolean _Genre_set;
		private Boolean _Rating_set;
		private Boolean _Playcount_set;
		private Boolean _FileExtension_set;
		private Boolean _Track_set;
		
		private String _error;
		
		public String DeviceId
		{
			get { return _DeviceId; }
			set { _DeviceId = value; _DeviceId_set = true; }
		}
		
		public String Path
		{
			get { return _Path; }
			set { _Path = value; _Path_set = true; }
		}
		
		public String FileName
		{
			get { return _FileName; }
			set { _FileName = value; _FileName_set = true; }
		}
		
		public String Artist
		{
			get { return _Artist; }
			set { _Artist = value; _Artist_set = true; }
		}
		
		public String Title
		{
			get { return _Title; }
			set { _Title = value; _Title_set = true; }
		}
		
		public String Album
		{
			get { return _Album; }
			set { _Album = value; _Album_set = true; }
		}
		
		public Int32 Kbps
		{
			get { return _Kbps; }
			set { _Kbps = value; _Kbps_set = true; }
		}
		
		public Int32 Channels
		{
			get { return _Channels; }
			set { _Channels = value; _Channels_set = true; }
		}
		
		public Int32 Frequency
		{
			get { return _Frequency; }
			set { _Frequency = value; _Frequency_set = true; }
		}
		
		public String Genre
		{
			get { return _Genre; }
			set { _Genre = value; _Genre_set = true; }
		}
		
		public Int32 Rating
		{
			get { return _Rating; }
			set { _Rating = value; _Rating_set = true; }
		}
		
		public Int32 Playcount
		{
			get { return _Playcount; }
			set { _Playcount = value; _Playcount_set = true; }
		}
		
		public String FileExtension
		{
			get { return _FileExtension; }
			set { _FileExtension = value; _FileExtension_set = true; }
		}
		
		public String Track
		{
			get { return _Track; }
			set { _Track = value; _Track_set = true; }
		}
		
		protected override DigitalMediaLibrary ReadRecord(DataRow pRow)
		{
			DigitalMediaLibrary digitalmedialibrary = new DigitalMediaLibrary(false);
			digitalmedialibrary.DeviceId = pRow["DeviceId"].ToString();
			digitalmedialibrary.Path = pRow["Path"].ToString();
			digitalmedialibrary.FileName = pRow["FileName"].ToString();
			digitalmedialibrary.Artist = pRow["Artist"].ToString();
			digitalmedialibrary.Title = pRow["Title"].ToString();
			digitalmedialibrary.Album = pRow["Album"].ToString();
			digitalmedialibrary.Kbps = Int32.Parse((pRow["Kbps"].ToString() == "" ? "0" : pRow["Kbps"].ToString()));
			digitalmedialibrary.Channels = Int32.Parse((pRow["Channels"].ToString() == "" ? "0" : pRow["Channels"].ToString()));
			digitalmedialibrary.Frequency = Int32.Parse((pRow["Frequency"].ToString() == "" ? "0" : pRow["Frequency"].ToString()));
			digitalmedialibrary.Genre = pRow["Genre"].ToString();
			digitalmedialibrary.Rating = Int32.Parse((pRow["Rating"].ToString() == "" ? "0" : pRow["Rating"].ToString()));
			digitalmedialibrary.Playcount = Int32.Parse((pRow["Playcount"].ToString() == "" ? "0" : pRow["Playcount"].ToString()));
			digitalmedialibrary.FileExtension = pRow["FileExtension"].ToString();
			digitalmedialibrary.Track = pRow["Track"].ToString();
			
			return digitalmedialibrary;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class DigitalMediaLibrary : DoTable
	{
		public static class Fields
		{
			public const string FIELD_DEVICEID = "DeviceId";
			public const string FIELD_PATH = "Path";
			public const string FIELD_FILENAME = "FileName";
			public const string FIELD_ARTIST = "Artist";
			public const string FIELD_TITLE = "Title";
			public const string FIELD_ALBUM = "Album";
			public const string FIELD_KBPS = "Kbps";
			public const string FIELD_CHANNELS = "Channels";
			public const string FIELD_FREQUENCY = "Frequency";
			public const string FIELD_GENRE = "Genre";
			public const string FIELD_RATING = "Rating";
			public const string FIELD_PLAYCOUNT = "Playcount";
			public const string FIELD_FILEEXTENSION = "FileExtension";
			public const string FIELD_TRACK = "Track";
		}
		
		private String _DeviceId;
		private String _Path;
		private String _FileName;
		private String _Artist;
		private String _Title;
		private String _Album;
		private Int32 _Kbps;
		private Int32 _Channels;
		private Int32 _Frequency;
		private String _Genre;
		private Int32 _Rating;
		private Int32 _Playcount;
		private String _FileExtension;
		private String _Track;
		
		private String _error;
		
		public DigitalMediaLibrary()
		{
			InitializeBase(true);
		}
		
		public DigitalMediaLibrary(bool _new)
		{
			InitializeBase(_new);
		}
		
		public String DeviceId
		{
			get { return DeviceId; }
			set { _DeviceId = value; }
		}
		
		public String Path
		{
			get { return Path; }
			set { _Path = value; }
		}
		
		public String FileName
		{
			get { return FileName; }
			set { _FileName = value; }
		}
		
		public String Artist
		{
			get { return Artist; }
			set { _Artist = value; }
		}
		
		public String Title
		{
			get { return Title; }
			set { _Title = value; }
		}
		
		public String Album
		{
			get { return Album; }
			set { _Album = value; }
		}
		
		public Int32 Kbps
		{
			get { return Kbps; }
			set { _Kbps = value; }
		}
		
		public Int32 Channels
		{
			get { return Channels; }
			set { _Channels = value; }
		}
		
		public Int32 Frequency
		{
			get { return Frequency; }
			set { _Frequency = value; }
		}
		
		public String Genre
		{
			get { return Genre; }
			set { _Genre = value; }
		}
		
		public Int32 Rating
		{
			get { return Rating; }
			set { _Rating = value; }
		}
		
		public Int32 Playcount
		{
			get { return Playcount; }
			set { _Playcount = value; }
		}
		
		public String FileExtension
		{
			get { return FileExtension; }
			set { _FileExtension = value; }
		}
		
		public String Track
		{
			get { return Track; }
			set { _Track = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("DeviceId", DataType.String, false, false, 256, false));
			base._cols.Add(new Column("Path", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("FileName", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("Artist", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("Title", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("Album", DataType.String, false, false, 1024, false));
			base._cols.Add(new Column("Kbps", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("Channels", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("Frequency", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("Genre", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("Rating", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("Playcount", DataType.Int, false, false, 4, false));
			base._cols.Add(new Column("FileExtension", DataType.String, false, false, 50, false));
			base._cols.Add(new Column("Track", DataType.String, false, false, 50, false));
			base._table = "DigitalMediaLibrary";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@DeviceId ", DbType.StringFixedLength, 256,"DeviceId").Value = _DeviceId.ToString();
			db.Parameters.Add("@Path ", DbType.StringFixedLength, 1024,"Path").Value = _Path.ToString();
			db.Parameters.Add("@FileName ", DbType.StringFixedLength, 1024,"FileName").Value = _FileName.ToString();
			db.Parameters.Add("@Artist ", DbType.StringFixedLength, 1024,"Artist").Value = _Artist.ToString();
			db.Parameters.Add("@Title ", DbType.StringFixedLength, 1024,"Title").Value = _Title.ToString();
			db.Parameters.Add("@Album ", DbType.StringFixedLength, 1024,"Album").Value = _Album.ToString();
			db.Parameters.Add("@Kbps ", DbType.Int16, 4,"Kbps").Value = _Kbps.ToString();
			db.Parameters.Add("@Channels ", DbType.Int16, 4,"Channels").Value = _Channels.ToString();
			db.Parameters.Add("@Frequency ", DbType.Int16, 4,"Frequency").Value = _Frequency.ToString();
			db.Parameters.Add("@Genre ", DbType.StringFixedLength, 50,"Genre").Value = _Genre.ToString();
			db.Parameters.Add("@Rating ", DbType.Int16, 4,"Rating").Value = _Rating.ToString();
			db.Parameters.Add("@Playcount ", DbType.Int16, 4,"Playcount").Value = _Playcount.ToString();
			db.Parameters.Add("@FileExtension ", DbType.StringFixedLength, 50,"FileExtension").Value = _FileExtension.ToString();
			db.Parameters.Add("@Track ", DbType.StringFixedLength, 50,"Track").Value = _Track.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
		}
	}
}

