using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	
	public class Artists : DoTableCollection<Artist>
	{
		protected override string TableName
		{
			get { return "Artist"; }
		}
		
		private Int32 _index;
		private String _ArtistName;
		private String _genre;
		
		private Boolean _index_set;
		private Boolean _ArtistName_set;
		private Boolean _genre_set;
		
		private String _error;
		
		public Int32 index
		{
			get { return _index; }
			set { _index = value; _index_set = true; }
		}
		
		public String ArtistName
		{
			get { return _ArtistName; }
			set { _ArtistName = value; _ArtistName_set = true; }
		}
		
		public String genre
		{
			get { return _genre; }
			set { _genre = value; _genre_set = true; }
		}
		
		protected override Artist ReadRecord(DataRow pRow)
		{
			Artist artist = new Artist(false);
			artist.index = Int32.Parse((pRow["index"].ToString() == "" ? "0" : pRow["index"].ToString()));
			artist.ArtistName = pRow["ArtistName"].ToString();
			artist.genre = pRow["genre"].ToString();
			
			return artist;
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class Artist : DoTable
	{
		public static class Fields
		{
			public const string FIELD_INDEX = "index";
			public const string FIELD_ARTISTNAME = "ArtistName";
			public const string FIELD_GENRE = "genre";
		}
		
		private Int32 _index;
		private String _ArtistName;
		private String _genre;
		
		private String _error;
		
		public Artist()
		{
			InitializeBase(true);
		}
		
		public Artist(bool _new)
		{
			InitializeBase(_new);
		}
		
		public Int32 index
		{
			get { return index; }
			set { _index = value; }
		}
		
		public String ArtistName
		{
			get { return ArtistName; }
			set { _ArtistName = value; }
		}
		
		public String genre
		{
			get { return genre; }
			set { _genre = value; }
		}
		
		private void InitializeBase(bool _new)
		{
			base._cols = new Columns();
			base._cols.Add(new Column("index", DataType.Int, true, false, 4, false));
			base._cols.Add(new Column("ArtistName", DataType.String, false, false, 100, false));
			base._cols.Add(new Column("genre", DataType.String, false, false, 50, false));
			base._table = "Artist";
			base._new = _new;
			
			//Generate SQL strings
			SQLGen.CreateInsertParms(this);
			SQLGen.CreateUpdateParms(this);
			SQLGen.CreateDeleteParms(this);
		}
		
		internal override void GetWriteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
			db.Parameters.Add("@ArtistName ", DbType.StringFixedLength, 100,"ArtistName").Value = _ArtistName.ToString();
			db.Parameters.Add("@genre ", DbType.StringFixedLength, 50,"genre").Value = _genre.ToString();
		}
		internal override void GetDeleteParms(SQLiteCommand db)
		{
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
		}
	}
}

