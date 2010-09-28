using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;

namespace CarMP
{
    public class DigitalMediaLibraryMap : ClassMap<DigitalMediaLibrary>
    {
        public DigitalMediaLibraryMap()
        {
            Id(x => x.LibraryId);
            Map(x => x.DeviceId);
            Map(x => x.Path);
            Map(x => x.FileName);
            Map(x => x.Artist);
            Map(x => x.Title);
            Map(x => x.Album);
            Map(x => x.Length);
            Map(x => x.Kbps);
            Map(x => x.Channels);
            Map(x => x.Frequency);
            Map(x => x.Genre);
            Map(x => x.Rating);
            Map(x => x.PlayCount);
            Map(x => x.FileExtension);
            Map(x => x.Track);
        }

    }
}
