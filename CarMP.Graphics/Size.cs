using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics
{
    public class Size
    {
        public float Width { get; set; }
        public float Height { get; set; }

        public Size(float pWidth, float pHeight)
        {
            Width = pWidth;
            Height = pHeight;
        }
    }
}
