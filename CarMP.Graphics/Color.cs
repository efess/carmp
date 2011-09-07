using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;
using System.Runtime.InteropServices;

namespace CarMP.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public static Color Black
        {
            get { return new Color(0, 0, 0); }
        }

        public static Color Gray
        {
            get { return new Color(.5F, .5F, .5F); }
        }

        public static Color LightGray
        {
            get { return new Color(.7f, .7f, .7f); }
        }

        public static Color White
        {
            get { return new Color(1f, 1f, 1f); }
        }

        public static Color RoyalBlue
        {
            get { return new Color(0f, 0f, 1f); }
        }

        public float Red { get; private set; }
        public float Green { get; private set; }
        public float Blue { get; private set; }
        public float Alpha { get; private set; }

        public Color(float pRed, float pGreen, float pBlue)
            : this(pRed, pGreen, pBlue, 1.0f)
        {

        }

        public Color(float pRed, float pGreen, float pBlue, float pAlpha)
            : this()
        {
            Red = pRed;
            Green = pGreen;
            Blue = pBlue;
            Alpha = pAlpha;
        }
        
        public Color(Color pColor, float pAlpha)
            : this(pColor.Red, pColor.Green, pColor.Blue, pAlpha)
        {
        }
    }
}
