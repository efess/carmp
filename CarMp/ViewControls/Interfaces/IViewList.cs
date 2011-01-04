using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.ViewControls
{
    public interface IViewList
    {
        SizeF ItemSize { get; set; }
        void InsertNext(DragableListItem listitem);
        void InsertNext(IEnumerable<DragableListItem> listitems);
    }
}
