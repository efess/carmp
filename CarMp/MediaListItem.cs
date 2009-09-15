using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
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

    public class MediaListItem : DragableListItem
    {
        private string m_itemDisplayString;
        private MediaItemType m_itemType;
        private MediaItemSpecialTarget m_itemSpecialTarget;
        private string m_itemTarget;
        private MediaItemType m_itemTargetType;
        
        public MediaListItem(string pDisplay, string pTarget, MediaItemType pItemType, MediaItemType pItemTargetType)
        {
            m_itemDisplayString = pDisplay;
            m_itemType = pItemType;
            m_itemTarget = pTarget;
            m_itemTargetType = pItemTargetType;
        }

        public MediaListItem(string pDisplay, MediaItemSpecialTarget pTarget, MediaItemType pItemType, MediaItemType pItemTargetType)
        {
            m_itemDisplayString = pDisplay;
            m_itemType = pItemType;
            m_itemSpecialTarget = pTarget;
            m_itemTargetType = pItemTargetType;
        }

        /// <summary>
        /// Type of item tyis is
        /// </summary>
        public MediaItemType ItemType
        {
            get { return m_itemType; }
            set { m_itemType = value; }
        }

        /// <summary>
        /// If this StringDefined, ItemTarget is used
        /// Otherwise, this is used to refer to a static target
        /// </summary>
        public MediaItemSpecialTarget ItemSpecialTarget
        {
            get { return m_itemSpecialTarget; }
            set { m_itemSpecialTarget = value; }
        }

        /// <summary>
        /// string representing this item
        /// </summary>
        public string ItemTarget
        {
            get { return m_itemTarget; }
            set { m_itemTarget = value; }
        }

        /// <summary>
        /// human readable representation
        /// </summary>
        public string ItemDisplayString
        {
            get { return m_itemDisplayString; }
            set { m_itemDisplayString = value; }
        }

        public override void DrawOnCanvas(System.Drawing.Graphics pCanvas)
        {
            //pCanvas.FillRectangle(Color.Gray,new Rectangle(0,0
            pCanvas.DrawString(
                m_itemDisplayString,
                new Font("Arial", 15F), 
                new LinearGradientBrush(new Point(0,0), new Point(0, ClientSize.Height), Color.White, Color.Gray), 
                new PointF(1,1)
                );
            pCanvas.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }
    }
}
