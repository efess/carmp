using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace CarMp
{
    public class DigitalMediaLibrary
    {
        public virtual int LibraryId { get; private set; }
        public virtual string DeviceId { get; set; }
        public virtual string Path { get; set; }
        public virtual string FileName { get; set; }
        public virtual string Artist { get; set; }
        public virtual string Title { get; set; }
        public virtual string Album { get; set; }
        public virtual int Length { get; set; }
        public virtual int Kbps { get; set; }
        public virtual int Channels { get; set; }
        public virtual int Frequency { get; set; }
        public virtual string Genre { get; set; }
        public virtual int Rating { get; set; }
        public virtual int PlayCount { get; set; }
        public virtual string FileExtension { get; set; }
        public virtual string Track { get; set; }
        
    }
}
