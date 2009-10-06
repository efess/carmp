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
        /// Determines what type of item this is
        /// </summary>
        Root,
        Directory,
        Playlist,
        Artist,
        Album,
        Song
    }

    public class MediaListItem : DragableListItem
    {
        /// <summary>
        /// String shown to the user
        /// </summary>
        public string DisplayString;

        /// <summary>
        /// Id representing next item id when clicked.
        /// </summary>
        public int TargetId { get; set; }

        /// <summary>
        /// Type representing what type of item this item is
        /// </summary>
        public MediaItemType ItemType;

        /// <summary>
        /// Creates a list item from a Group Item object
        /// </summary>
        /// <param name="pGroupItem"></param>
        public MediaListItem(MediaGroupItem pGroupItem)
        {
            this.DisplayString = pGroupItem.ItemName;
            this.ItemType = (MediaItemType)pGroupItem.ItemType;

            if (ItemType == MediaItemType.Song)
            {
                if (pGroupItem.LibraryId == 0)
                    throw new Exception("Library Id must not be 0");
                TargetId = pGroupItem.LibraryId;
            }
            else
            {
                if(pGroupItem.NextGroupId == 0)
                    throw new Exception("Next Group must not be 0");
                TargetId = pGroupItem.NextGroupId;
            }
        }

        public MediaListItem(string pDisplayString, MediaItemType pItemType, int pItemTargetId)
        {
            DisplayString = pDisplayString;
            ItemType = pItemType;
            TargetId = pItemTargetId;
        }

        public override void DrawOnCanvas(System.Drawing.Graphics pCanvas)
        {
            //pCanvas.FillRectangle(Color.Gray,new Rectangle(0,0
            pCanvas.DrawString(
                DisplayString,
                new Font("Arial", 15F), 
                new LinearGradientBrush(new Point(0,0), new Point(0, ClientSize.Height), Color.White, Color.Gray), 
                new PointF(1,1)
                );
            pCanvas.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }
    }
}
