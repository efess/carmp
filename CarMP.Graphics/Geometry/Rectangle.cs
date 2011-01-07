using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public class Rectangle
    {
        public float X { get; set;}
        public float Y { get; set;}
        public float Width { get; set;}
        public float Height { get; set;}
        public float Left
        {
            get { return X;}
            set { X = value;}
        }
        public float Top
        {
            get { return Y; }
            set { Y = value;}
        }
        public float Right 
        {
            get { return X + Width;}
            set { 
                if(value > X)
                    Width = value - X;
            }
        }
        public float Bottom
        {
            get { return Y + Height;}
            set { 
                if(value > Y)
                    Height = value - Y;
            }
        }

        public Point Location
        {
            get { return new Point(X, Y); }
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
        }

        public Rectangle(float pX, float pY, float pWidth, float pHeight)
        {
            X = pX;
            Y = pY;
            Width = pWidth;
            Height = pHeight;
        }

        public Rectangle(Point pPoint, Size pSize)
        {
            X = pPoint.X;
            Y = pPoint.Y;
            Width = pSize.Width;
            Height = pSize.Height;
        }
    }
}
