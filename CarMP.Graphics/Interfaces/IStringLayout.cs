using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Geometry;

namespace CarMP.Graphics.Interfaces
{
    public interface IStringLayout
    {
        float Size { get; set; }

        string Font { get; set; }

        bool WordWrap { get; set; }

        StringAlignment Alignment { get; set; }

        string String { get; set; }

        Point GetPointAtCharPosition(int pCharPosition);
        int GetCharPositionAtPoint(Point pPoint);
        Size GetStringSize();
    }
}
