using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CarMP.MediaInfo;

namespace CarMP.IO
{
    public class FileMediaInfo
    {
        public static MediaItem GetInfo(FileInfo pFile)
        {
            switch (pFile.Extension.ToUpper())
            {
                case ".MP3":
                    return GetMp3Info(pFile);
                default:
                    throw new NotSupportedException("Extension not supported");
            }
        }

        private static MediaItem GetMp3Info(FileInfo pFile)
        {
            Id3Read reader = new Id3Read(pFile.FullName);
            MediaItem item = new MediaItem();
            if (string.IsNullOrEmpty(reader.Title) && string.IsNullOrEmpty(reader.Artist))
            {
                FilenameInfo fileParser = new FilenameInfo();
                fileParser.Parse(pFile.Name);

                item.Track = fileParser.Track;
                item.Artist = fileParser.Artist;
                item.Title = fileParser.Title;
            }
            else
            {
                item.Album = reader.Album;
                item.Artist = reader.Artist;
                item.Title = reader.Title;
            }
            item.FileName = pFile.Name;
            item.Path = pFile.FullName;
            item.Track = reader.Track;
            item.Genre = reader.Genre;
            item.Kbps = reader.BitRate;

            // Clean up item records
            if (string.IsNullOrEmpty(item.Album)) item.Album = "NoAlbum";
            if (string.IsNullOrEmpty(item.Artist)) item.Artist = "NoArtist";
            if (string.IsNullOrEmpty(item.Title)) item.Title = "Untitled";

            //item.Album = reader.A
            return item;
        }

    }
}
