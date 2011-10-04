using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.MediaEntities;

namespace CarMP
{
    public interface IMediaSelection
    {
        string DisplayString { get; }
        MediaListItemType MediaType { get; }
        string Key { get; }
        int ItemSpecificType { get; }
        string ObjectType { get; }
    }
}
