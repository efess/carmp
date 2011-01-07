using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using Microsoft.WindowsAPICodePack.DirectX.Direct2D1;

namespace CarMP.Graphics.Implementation.Direct2D
{
    public class D2DBrush : IBrush
    {
        public Color Color { get; set; }

        internal Brush BrushResource { get; set; }

        internal D2DBrush(RenderTarget pRenderer, Color pColor)
        {
            BrushResource = pRenderer.CreateSolidColorBrush(
                new ColorF(pColor.Red, pColor.Green, pColor.Blue, pColor.Alpha));

            Color = pColor;
        }
    }
}
