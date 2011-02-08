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
        internal const string COLOR_GRAY = "Gray";
        internal const string COLOR_LIGHT_GRAY = "LightGray";
        internal const string COLOR_WHITE = "White";


        internal const string COLOR_BLUE = "Blue";

        internal static IColorResolver ColorResolver { set; private get; }

        public static Color Black
        {
            get { return GetColor(COLOR_BLACK); }
        }

        public static Color Gray
        {
            get { return GetColor(COLOR_GRAY); }
        }

        public static Color LightGray
        {
            get { return GetColor(COLOR_LIGHT_GRAY); }
        }

        public static Color White
        {
            get { return GetColor(COLOR_WHITE); }
        }

        public static Color RoyalBlue
        {
            get { return GetColor(COLOR_BLUE); }
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
                case COLOR_GRAY:
                    return new Color(.5F, .5F, .5F);
                case COLOR_WHITE:
                    return new Color(1f, 1f, 1f);
                case COLOR_LIGHT_GRAY:
                    return new Color(.7f, .7f, .7f);
                case COLOR_BLUE:
                    return new Color(0f, 0f, 1f);
                default:
                    throw new InvalidProgramException("Invalid color used");
            }
        }

        public float Red { get; private set; }
        public float Blue { get; private set; }
        public float Green { get; private set; }
        public float Alpha { get; private set; }

        public Color(float pRed, float pGreen, float pBlue)
            : this(pRed, pGreen, pBlue, 1.0f)
        {

        }

        public Color(float pRed, float pGreen, float pBlue, float pAlpha)
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
