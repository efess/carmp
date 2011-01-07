using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarMP.Graphics.Interfaces;

namespace CarMP.Graphics
{
    public class Color
    {
        internal const string COLOR_BLACK = "Black";
        internal static IColorResolver ColorResolver { set; private get; }

        public static Color Black
        {
            get { return GetColor(COLOR_BLACK); }
        }

        protected static Color GetColor(string pColor)
        {
            if(ColorResolver != null)
            {
                Color color = ColorResolver.GetColor(pColor);
                if (color != null)
                    return color;
            }
            switch (pColor)
            {
                case COLOR_BLACK:
                    return new Color(0,0,0);
                default:
                    throw new InvalidProgramException("Invalid color used");
            }
        }

        public float Red { get; private set; }
        public float Blue { get; private set; }
        public float Green { get; private set; }
        public float Alpha { get; private set; }

        public Color(float pRed, float pBlue, float pGreen)
            : this(pRed, pBlue, pGreen, 1.0f)
        {

        }

        public Color(float pRed, float pBlue, float pGreen, float pAlpha)
        {
            Red = pRed;
            Green = pGreen;
            Blue = pBlue;
            Alpha = pAlpha;
        }
    }
}
