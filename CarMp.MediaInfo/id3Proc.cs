using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace CarMP.MediaInfo
{
    public class Id3Read
    {
        public string infile;

        public Id3v1 Id3v1 { get; private set;}
        public Id3v2 Id3v2 { get; private set;}

        public Mp3Header mp3Header;

        public string Album
        {
            get { if (!string.IsNullOrEmpty(Id3v2.Album)) return Id3v2.Album; return Id3v1.Album; }
        }
        public string Artist
        {
            get { if (!string.IsNullOrEmpty(Id3v2.Artist)) return Id3v2.Artist; return Id3v1.Artist; }
        }
        public string Title
        {
            get { if (!string.IsNullOrEmpty(Id3v2.Title)) return Id3v2.Title; return Id3v1.Title; }
        }
        public string Track
        {
            get { if (!string.IsNullOrEmpty(Id3v2.TrackNum)) return Id3v2.TrackNum; return Id3v1.Track; }
        }
        public int BitRate
        {
            get { if (!(Id3v2.BitRate == 0)) return Id3v2.BitRate; return Id3v1.BitRate; }
        }
        public string Genre
        {
            get { if (!string.IsNullOrEmpty(Id3v2.Genre)) return Id3v2.Genre; return Id3v1.Genre; }
        }
        public string Year
        {
            get { if (!string.IsNullOrEmpty(Id3v2.Year)) return Id3v2.Year; return Id3v1.Year; }
        }
        public byte[] AlbumPic
        {
            get {  return Id3v2.AlbumPic; }
        }

        #region GenreConv
        public static string[] GenreConv = 
        { 
	        "Blues",		    //0
	        "Classic Rock",	    //1
	        "Country",		    //2
	        "Dance",		    //3
	        "Disco",		    //4
	        "Funk",		        //5
	        "Grunge",		    //6
	        "Hip-Hop",		    //7
	        "Jazz",		        //8
	        "Metal",		 //9
	        "New Age",		 //10
	        "Oldies",		 //11
	        "Other",		 //12
	        "Pop",		 //13
	        "R&B",		 //14
	        "Rap",		 //15
	        "Reggae",		 //16
	        "Rock",		 //17
	        "Techno",		 //18
	        "Industrial",		 //19
	        "Alternative",		 //20
	        "Ska",		 //21
	        "Death Metal",		 //22
	        "Pranks",		 //23
	        "Soundtrack",		 //24
	        "Euro-Techno",		 //25
	        "Ambient",		 //26
	        "Trip-Hop",		 //27
	        "Vocal",		 //28
	        "Jazz+Funk",		 //29
	        "Fusion",		 //30
	        "Trance",		 //31
	        "Classical",		 //32
	        "Instrumental",		 //33
	        "Acid",		 //34
	        "House",		 //35
	        "Game",		 //36
	        "Sound Clip",		 //37
	        "Gospel",		 //38
	        "Noise",		 //39
	        "AlternRock",		 //40
	        "Bass",		 //41
	        "Soul",		 //42
	        "Punk",		 //43
	        "Space",		 //44
	        "Meditative",		 //45
	        "Instrumental Pop",		 //46
	        "Instrumental Rock",		 //47
	        "Ethnic",		 //48
	        "Gothic",		 //49
	        "Darkwave",		 //50
	        "Techno-Industrial",		 //51
	        "Electronic",		 //52
	        "Pop-Folk",		 //53
	        "Eurodance",		 //54
	        "Dream",		 //55
	        "Southern Rock",		 //56
	        "Comedy",		 //57
	        "Cult",		 //58
	        "Gangsta",		 //59
	        "Top 40",		 //60
	        "Christian Rap",		 //61
	        "Pop/Funk",		 //62
	        "Jungle",		 //63
	        "Native American",		 //64
	        "Cabaret",		 //65
	        "New Wave",		 //66
	        "Psychadelic",		 //67
	        "Rave",		 //68
	        "Showtunes",		 //69
	        "Trailer",		 //70
	        "Lo-Fi",		 //71
	        "Tribal",		 //72
	        "Acid Punk",		 //73
	        "Acid Jazz",		 //74
	        "Polka",		 //75
	        "Retro",		 //76
	        "Musical",		 //77
	        "Rock & Roll",		 //78
	        "Hard Rock",		 //79
	        "Folk",		 //80
	        "Folk-Rock",		 //81
	        "National Folk",		 //82
	        "Swing",		 //83
	        "Fast Fusion",		 //84
	        "Bebob",		 //85
	        "Latin",		 //86
	        "Revival",		 //87
	        "Celtic",		 //88
	        "Bluegrass",		 //89
	        "Avantgarde",		 //90
	        "Gothic Rock",		 //91
	        "Progressive Rock",		 //92
	        "Psychedelic Rock",		 //93
	        "Symphonic Rock",		 //94
	        "Slow Rock",		 //95
	        "Big Band",		 //96
	        "Chorus",		 //97
	        "Easy Listening",		 //98
	        "Acoustic",		 //99
	        "Humour",		 //100
	        "Speech",		 //101
	        "Chanson",		 //102
	        "Opera",		 //103
	        "Chamber Music",		 //104
	        "Sonata",		 //105
	        "Symphony",		 //106
	        "Booty Bass",		 //107
	        "Primus",		 //108
	        "Porn Groove",		 //109
	        "Satire",		 //110
	        "Slow Jam",		 //111
	        "Club",		 //112
	        "Tango",		 //113
	        "Samba",		 //114
	        "Folklore",		 //115
	        "Ballad",		 //116
	        "Power Ballad",		 //117
	        "Rhythmic Soul",		 //118
	        "Freestyle",		 //119
	        "Duet",		 //120
	        "Punk Rock",		 //121
	        "Drum Solo",		 //122
	        "A capella",		 //123
	        "Euro-House",		 //124
	        "Dance Hall"		 //125
        };
        #endregion

        public BinaryReader br;
        public Id3Read(string pFile)
        {
            Id3v1 = new Id3v1();
            Id3v2 = new Id3v2();
            mp3Header = new Mp3Header();
            infile = pFile;

           
            try
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(pFile, FileMode.Open, FileAccess.Read);//File.OpenRead(pFile);
                }
                catch { return; }///ignore
                br = new BinaryReader(fs);
                //Check for ID3v2
                if (ASCIIByteArrayToStr(br.ReadBytes(3)) == "ID3")
                {
                    Id3v2.read(fs);
                }

                Id3v1.read(fs);

                mp3Header.read(fs, Id3v2.TagSize);


                //Get basic MP3 file info (non id3)

            }
            catch(Exception e)
            {
                
            }
            
        }

        public static int BytesToInt(byte[] pBytes)
        {
            // If version is 4, use synchsafe
            if (id3v2Header.Version == 4)
            {
                return (pBytes[3]) |
                        (pBytes[2] << 7) |
                        (pBytes[1] << 14) |
                        (pBytes[0] << 21);
            }
            else
            {
                
                    return (pBytes[3]) |
                            (pBytes[2] << 8) |
                            (pBytes[1] << 16) |
                            (pBytes[0] << 24);
                

            }
        }
        public static string ASCIIByteArrayToStr(byte[] array)
        {
            Encoding enc = Encoding.GetEncoding(1252);
        //       System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //       System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
            return enc.GetString(array);
        }

        public static string UNICODEByteArrayToStr(byte[] array)
        {
            Encoding enc = Encoding.Unicode;
            //       System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            //       System.Text.UnicodeEncoding enc = new System.Text.UnicodeEncoding();
            return enc.GetString(array);
        }
    }
    public class Mp3Header
    {
                
        public string VersionID;
        public int Layer;
        public bool Protection;
        public int Bitrate;
        public int Frequency;
        public string Mode;
        public long durationSecs;
        //Conversion
        private int[] v1l1 = { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, 0 };
        private int[] v1l2 = { 0, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 0 };
        private int[] v1l3 = { 0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0 };
        private int[] v2l1 = { 0, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 0 };
        private int[] v2l2l3 = { 0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0 };
        private int[,] FreqConv = new int[,] { { 11025, 12000, 8000, 0 }, { 0, 0, 0,0 },  { 22050, 24000, 16000, 0 }, { 44100, 48000, 3200, 0 }};
        private int[,] BitModeChk = new int[,] {{1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
                                                {1,0,0,0,1,0,1,1,1,1,1,1,1,1,1,1},
                                                {1,0,0,0,1,0,1,1,1,1,1,1,1,1,1,1},
                                                {1,0,0,0,1,0,1,1,1,1,1,1,1,1,1,1}};
        public void read(FileStream fs)
        {
            read(fs, 0);

        }
        public void read(FileStream pfs, int pStart)
        {
            //conversion variables
            int bytVersionID;
            int bytLayer;
            int bytBitrate;
            int bytFrequency;
            int bytMode;
            int bytModeExtension;
            int bytEmphasis;
            bool bad = true;


            byte[] header;
            pfs.Position = pStart;
            if (pfs.Length > 10)
            {
                int counter = 0 ;
                BinaryReader br = new BinaryReader(pfs);

                header = br.ReadBytes(4);
                long tempPosition = pfs.Length;
                //If there are a ridiculous amount of zeros, just goto the middle of the stream
                while (header[0] == 0 && header[1] == 0 && header[2] == 0 && header[3] == 0) 
                {
                    if (counter > 100)
                    {
                        pfs.Position += tempPosition / 2;
                        tempPosition = tempPosition / 2;
                    }
                    pfs.Position -= 3;
                    header = br.ReadBytes(4);
                    if (header.Length < 4)
                        return;
                    counter = counter + 1;
                }
                
                
                //Slide through data 1 byte at a time until we find the sync bits.
                for (int i = pStart; ((header[0] != 255) || (header[1] == 255) || ((header[1] & 240) != 240)) || bad; pfs.Position -= 3)
                {
                    //Make sure we actualy found them...
                    if ((header[0] == 255) && ((header[1] & 240) == 240 && (header[1] != 255)))
                    {
                        bad = false;
                        bytVersionID = (header[1] & 24) >> 3;
                        bytLayer = (header[1] & 6) >> 1;
                        Protection = (header[1] & 1) == 1 ? true : false;
                        bytBitrate = header[2] >> 4;
                        bytFrequency = (header[2] & 12) >> 2;
                        bytMode = (header[3] & 192) >> 6;
                        bytModeExtension = (header[3] & 48) >> 4;
                        bytEmphasis = (header[3] & 3);

                        switch (bytVersionID)
                        {
                            case 0:
                                VersionID = "MPEG Version 2.5";
                                break;
                            case 2:
                                VersionID = "MPEG Version 2";
                                break;
                            case 3:
                                VersionID = "MPEG Version 1";
                                break;
                            default:
                                bad = true;
                                break;
                        }

                        switch (bytLayer)
                        {
                            case 1:
                                Layer = 3;
                                break;
                            case 2:
                                bad = true;//Layer = 2;
                                break;
                            case 3:
                                bad = true;//Layer = 1;
                                break;
                            default:
                                bad = true;
                                break;
                        }

                        switch (VersionID)
                        {
                            case "MPEG Version 1":
                                switch (Layer)
                                {
                                    case 1:
                                        Bitrate = v1l1[bytBitrate];
                                        break;
                                    case 2:
                                        Bitrate = v1l2[bytBitrate];
                                        break;
                                    case 3:
                                        Bitrate = v1l3[bytBitrate];
                                        break;

                                }
                                break;
                            case "MPEG Version 2":
                            case "MPEG Version 2.5":
                                switch (Layer)
                                {
                                    case 1:
                                        Bitrate = v2l1[bytBitrate];
                                        break;
                                    case 2:
                                    case 3:
                                        Bitrate = v2l2l3[bytBitrate];
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        Frequency = FreqConv[bytVersionID, bytFrequency];

                        switch (bytMode)
                        {
                            case 0:
                                Mode = "Stereo";
                                break;
                            case 1:
                                Mode = "Joint Stereo";
                                break;
                            case 2:
                                Mode = "Dual Channel";
                                break;
                            case 3:
                                Mode = "Mono";
                                break;
                            default:
                                break;
                        }
                        if (bytEmphasis != 0 || bad == true || Bitrate == 0 || Frequency == 0 || (Mode != "Joint Stereo" && bytModeExtension != 0))
                        {
                            bad = true;
                            header = br.ReadBytes(4);
                        }

                    }
                    else
                    {
                        header = br.ReadBytes(4);
                    }
                }


                durationSecs = (br.BaseStream.Length * 8) / (1000 * Bitrate);
            }
        }
    }

    public class Id3v1
    {

        public bool exists;
        
        public string FileName;
        public string Title;
        public string Artist;
        public string Album;
        public string Year;
        public string Comment;
        public string Genre;
        public string Track;
        public int BitRate;
        
        public Id3v1()
        {
            exists = false;
        }
        public void read(FileStream fs)
        {
            try                
            {
                if (fs.Length < 128)
                    return;
                //Check for ID3v1 tag...
                byte[] temp = new byte[30];
                fs.Position = fs.Length - 128;
                BinaryReader br = new BinaryReader(fs);
                char[] paddingchars = {' ','0','\0'};

                if (Id3Read.ASCIIByteArrayToStr(br.ReadBytes(3)) == "TAG")
                {
                    //Read Title
                    temp = br.ReadBytes(30);
                    Title = Id3Read.ASCIIByteArrayToStr(temp).TrimEnd(paddingchars);

                    //Read Artist
                    temp = br.ReadBytes(30);
                    Artist = Id3Read.ASCIIByteArrayToStr(temp).TrimEnd(paddingchars);

                    //Read Album
                    temp = br.ReadBytes(30);
                    Album = Id3Read.ASCIIByteArrayToStr(temp).TrimEnd(paddingchars);

                    //Read Year
                    temp = br.ReadBytes(4);
                    Year = Id3Read.ASCIIByteArrayToStr(temp).TrimEnd(paddingchars);

                    //Read Comment
                    temp = br.ReadBytes(30);
                    Comment = Id3Read.ASCIIByteArrayToStr(temp).TrimEnd(paddingchars);

                    //Read Track (Id3v1.1)
                    //temp = br.ReadBytes(2);
                    if (temp[28] == 0)
                    {
                        Track = temp[29].ToString();
                    }

                    //Read Genre
                    temp[0] = br.ReadByte();
                    if (temp[0] < 81)
                    {
                        try
                        {
                            Genre = Id3Read.GenreConv[temp[0]];
                        }
                        catch (Exception e)
                        {
                            throw (e);
                        }
                    }
                }
            }
            catch
            {

            }
        }
        
    }
    

    public class Id3v2
    {
        public bool exists;
        public int TagSize;
        public string EncoderInfo;
        public string Title;
        public string Artist;
        public string Album;
        public string TrackNum;
        public string Publisher;
        public string Year;
        public string Composer;
        public string Genre;
        public byte[] AlbumPic;
        public int BitRate;
        private ArrayList Frames;

        public Id3v2()
        {
            
            exists = false;
        }
        public void read(FileStream fs)
        {
            id3v2Header Header;
            try
            {
                fs.Position = 0;
                BinaryReader br = new BinaryReader(fs);
                Header = new id3v2Header(br.ReadBytes(10));
                TagSize = Header.Hsize+10;
                //Make sure this is version 4 or below.
                if (id3v2Header.Version <= 4)
                {
                    Frames = new ArrayList();
                    while(fs.Position < TagSize)
                    {
                        id3v2Frame newframe = new id3v2Frame(br);
                        if (newframe.FrameOk)
                        {
                            Frames.Add(newframe);
                        }
                    }
                    foreach (id3v2Frame frame in Frames)
                    {
                        switch (frame.FrameID)
                        {
                            case "TENC":
                                EncoderInfo = frame.FrameData;
                                break;
                            case "TIT1":
                                break;

                            case "TIT2":
                                Title = frame.FrameData;
                                break;
                            case "TIT3":
                                break;
                            case "TALB":
                                Album = frame.FrameData;
                                break;
                            case "TRCK":
                                TrackNum = frame.FrameData;
                                break;
                            case "TPUB":
                                Publisher = frame.FrameData;
                                break;
                            case "TCON":
                                int tryParser = 0;
                                string frameData = frame.FrameData.Replace("(", "").Replace(")", "");
                                if (int.TryParse(frameData, out tryParser) && tryParser < Id3Read.GenreConv.Length && tryParser > -1)
                                {
                                    Genre = Id3Read.GenreConv[tryParser];
                                }
                                else
                                {
                                    Genre = frameData;
                                }

                                break;
                            case "TYER":
                                Year = frame.FrameData;
                                break;
                            case "TPE1":
                            case "TPE2":
                                Artist = frame.FrameData;
                                break;
                            case "TCOM":
                                Composer = frame.FrameData;
                                break;
                            case "APIC":
                                AlbumPic = System.Text.Encoding.ASCII.GetBytes(frame.FrameData);
                                break;
                            default:
                                break;
                        }
                    }
                    //byte[] rawmp3 = new byte[4];
                    //rawmp3 = br.ReadBytes(4);

                    exists = true;
                }

            }
            catch (Exception e)
            {
                exists = false;
            }
        }

    }

    public class id3v2Frame
    {
        private id3v2FrameHeader FrameHeader;
        public string FrameID;
        public string FrameData;
        public bool FrameOk;
        byte[] data;

        char[] paddingchars = { ' ', '0', '\0' };
        public id3v2Frame(BinaryReader br)
        {
            FrameHeader = new id3v2FrameHeader(br.ReadBytes(10));
            if(FrameHeader.FrameOk)
            {
                FrameID = FrameHeader.FrameID;
                try
                {
                    //byte enc = br.ReadByte();
                    //if (enc == 0 || 1 == 1)
                    //{
                    byte encoding = br.ReadByte();
                    switch (encoding)
                    {
                        case 0:

                            data = br.ReadBytes(FrameHeader.FrameSize-1);
                            FrameData = Id3Read.ASCIIByteArrayToStr(data).TrimEnd(paddingchars).TrimStart(paddingchars);
                            break;
                        case 1:

                            data = br.ReadBytes(FrameHeader.FrameSize-1);
                            FrameData = Id3Read.UNICODEByteArrayToStr(data).TrimEnd(paddingchars).TrimStart(paddingchars);
                            break;
                        default:
                            data = br.ReadBytes(FrameHeader.FrameSize - 1);
                            FrameData = Id3Read.ASCIIByteArrayToStr(data).TrimEnd(paddingchars).TrimStart(paddingchars);
                            break;


                    }
                    FrameOk = true;
                    /*
                    int c = 0;
                    byte[] temp_data = new byte[data.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (!(data[i] == 0 && i == 0) && !(data[i]== 0 && i == data.Length-1))
                        {
                            temp_data[c] = data[i];
                            c++;
                                                      
                        }
                    }

                    if (c == data.Length)
                    {
                        FrameData = Id3Read.ByteArrayToStr(data);
                        FrameOk = true;
                    }
                    else
                    {

                        byte[] temp2_data = new byte[c];
                        for (int i = 0; i < c; i++)
                        {
                            temp2_data[i] = temp_data[i];
                        }

                        FrameData = Id3Read.ByteArrayToStr(temp2_data);
                        FrameOk = true;
                    }
                     * */
                    //}
                    
                }
                catch(Exception e)
                {
                    FrameOk = false;
                }


            }
        }        
    }
    public class id3v2FrameHeader
    {
        
        public string FrameID;
        public int FrameSize;
        public bool FrameAlterPreservation;
        public bool FileAlterPreservation;
        public bool ReadOnly;
        public bool GroupIdentity;
        public bool Compressed;
        public bool Encrypted;
        public bool Unsynchronised;
        public bool DataLengthIdentifier;
        public bool FrameOk;

        public id3v2FrameHeader(byte[] header)
        {
            if (header.Length == 10)
            {
                try
                {
                    byte[] t_id = new byte[4];
                    byte[] t_size = new byte[4];

                    for (int i = 0; i < 4; i++)
                    {
                        t_id[i] = header[i];
                        t_size[i] = header[i + 4];
                    }
                    if (t_size[0] == 0)
                    {

                        FrameSize = Id3Read.BytesToInt(t_size);

                        if (FrameSize != 0 && FrameSize < 1000000)
                        {
                            FrameID = Id3Read.ASCIIByteArrayToStr(t_id);
                            //Flags
                            FrameAlterPreservation = (header[8] & 64) == 64 ? true : false;
                            FileAlterPreservation = (header[8] & 32) == 32 ? true : false;
                            ReadOnly = (header[8] & 16) == 16 ? true : false;
                            GroupIdentity = (header[9] & 64) == 64 ? true : false;
                            Compressed = (header[9] & 8) == 8 ? true : false;
                            Encrypted = (header[9] & 4) == 4 ? true : false;
                            Unsynchronised = (header[9] & 2) == 2 ? true : false;
                            DataLengthIdentifier = (header[9] & 1) == 1 ? true : false;
                            FrameOk = true;
                        }
                        else
                        {
                            FrameOk = false;
                        }
                    }
                    else
                    {
                        FrameOk = false;
                    }

                }
                catch (Exception e)
                {
                    FrameOk = false;
                }
            }
            else
            {
                FrameOk = false;
            }

        }

    }
    public class id3v2Header
    {
        public static int Version;
        public int Revision;
        public bool Unsynchronisation;
        public bool ExtendedHeader;
        public bool ExperimentalIndicator;
        public bool FooterPresent;
        public int Hsize;
        
        public id3v2Header(byte[] header)
        {
            if(header.Length == 10)
            {
                byte[] fourbytes = new byte[4];
                Version = Convert.ToInt16(header[3]);
                Revision = Convert.ToInt16(header[4]);
                Unsynchronisation = (header[5] & 128) == 128 ? true : false;
                ExtendedHeader = (header[5] & 64) == 64 ? true : false;
                ExperimentalIndicator = (header[5] & 32) == 32 ? true : false;
                FooterPresent = (header[5] & 16) == 16 ? true : false;
                
                Hsize = (header[9]) |
                        (header[8] << 7) |
                        (header[7] << 14) |
                        (header[6] << 21); ; 
            }
        }
    }    
}
