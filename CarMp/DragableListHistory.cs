using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMpControls;

namespace CarMp
{
    public enum MediaItemType
    {
        /// <summary>
        /// Determines what type of search
        /// </summary>
        Root,
        Directory,
        Playlist,
        Artist,
        Album,
        Song
    }

    /// <summary>
    /// Defines some static Targets
    /// </summary>
    public enum MediaItemSpecialTarget
    {
        /// <summary>
        /// 
        /// </summary>
        StringDefined,
        AllArtists,
        AllAlbums,
        AllPlaylists,
        AllSongs,
        RootDirectories
    }

    public class DragableListSelectHistory : Stack<DragableListItem>
    {
        
    }
}
