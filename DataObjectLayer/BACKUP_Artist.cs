using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace DataObjectLayer
{
	public class Artists : List<Artist>
	{
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
		
		public bool Write()
		{
			using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
			{
				bool _commit;
				
				try
				{
					SQLiteCommand dbc = new SQLiteCommand();
					dbc.Connection = SQLiteCommon.GetConnection();
					foreach (Artist dbObj in this)
					{
						dbObj.GetWrite(dbc);
						dbc.ExecuteNonQuery();
					}
					dbc.Connection.Close();
				}
				catch (Exception e)
				{
					_error = e.Message;
				}
				
				if (!String.IsNullOrEmpty(_error))
				{
					return false;
				}
				else
				{
					ts.Complete();
					return true;
				}
			}
		}
		
		public bool Read(String query)
		{
			String SQL = "SELECT * FROM Artist WHERE " + query;
			DataSet ds = SQLiteCommon.RunQuery(SQL, out _error);
			
			if (_error != "")
			{
				return false;
			}
			else
			{
				// Success
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					Artist artist = new Artist(false);
					artist.index = Int32.Parse((dr["index"].ToString() == "" ? "0" : dr["index"].ToString()));
					artist.ArtistName = dr["ArtistName"].ToString();
					artist.genre = dr["genre"].ToString();
					this.Add(artist);
				}
				return true;
			}
			
		}
		
		public bool Read()
		{
			StringBuilder SQL = new StringBuilder("SELECT * FROM Artist WHERE ");
			Boolean first = true;
			
			if (_index_set)
			{
				if (first)
				{
					first = false;
					SQL.Append("[index] = " + _index);
				}
				else
				{
					SQL.Append(" AND [index] = " + _index);
				}
			}
			
			if (_ArtistName_set)
			{
				if (first)
				{
					first = false;
					SQL.Append("[ArtistName] = '" + SQLiteCommon.FixQuote(_ArtistName) + "'");
				}
				else
				{
					SQL.Append(" AND [ArtistName] = '" + SQLiteCommon.FixQuote(_ArtistName) + "'");
				}
			}
			
			if (_genre_set)
			{
				if (first)
				{
					first = false;
					SQL.Append("[genre] = '" + SQLiteCommon.FixQuote(_genre) + "'");
				}
				else
				{
					SQL.Append(" AND [genre] = '" + SQLiteCommon.FixQuote(_genre) + "'");
				}
			}
			
			
			DataSet ds = SQLiteCommon.RunQuery(SQL.ToString(), out _error);
			
			if (_error != "")
			{
				return false;
			}
			else
			{
				// Success
				foreach (DataRow dr in ds.Tables[0].Rows)
				{
					Artist artist = new Artist(false);
					artist.index = Int32.Parse((dr["index"].ToString() == "" ? "0" : dr["index"].ToString()));
					artist.ArtistName = dr["ArtistName"].ToString();
					artist.genre = dr["genre"].ToString();
					this.Add(artist);
				}
				return true;
			}
		}
		
		public bool Remove()
		{
			
			return true;
		}
	}
	
	public class Artist : DoTable
	{
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
		}
		
		public bool Write()
		{
			using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
			{
				bool _commit;
				
				SQLiteCommand dbc = new SQLiteCommand();
				try
				{
					dbc.Connection = SQLiteCommon.GetConnection();
					this.GetWrite(dbc);
					dbc.ExecuteNonQuery();
					dbc.Connection.Close();
				}
				catch (Exception e)
				{
					_error = e.Message;
				}
				
				if (!String.IsNullOrEmpty(_error))
				{
					return false;
				}
				else
				{
					ts.Complete();
					return true;
				}
			}
		}
		
		internal void GetWrite(SQLiteCommand db)
		{
			db.CommandText = WriteSQL();
			db.Parameters.Add("@index ", DbType.Int16, 4,"index").Value = _index.ToString();
			db.Parameters.Add("@ArtistName ", DbType.StringFixedLength, 100,"ArtistName").Value = _ArtistName.ToString();
			db.Parameters.Add("@genre ", DbType.StringFixedLength, 50,"genre").Value = _genre.ToString();
		}
	}
}

