using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;

namespace CarMP.ViewControls
{
    public interface IViewList
    {
        Size ItemSize { get; set; }
        void InsertNext(DragableListItem listitem);
        void InsertNext(IEnumerable<DragableListItem> listitems);
    }
}
