using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public struct SizeI
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public SizeI(int pWidth, int pHeight)
            : this()
        {
            Width = pWidth;
            Height = pHeight;
        }
    }

    public struct Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size(float pWidth, float pHeight)
            :this()
        {
            Width = pWidth;
            Height = pHeight;
        }
    }
}
