using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Graphics.Geometry
{
    public struct RectangleI
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Left
        {
            get { return X; }
            set { X = value; }
        }
        public int Top
        {
            get { return Y; }
            set { Y = value; }
        }
        public int Right
        {
            get { return X + Width; }
            set
            {
                if (value > X)
                    Width = value - X;
            }
        }
        public int Bottom
        {
            get { return Y + Height; }
            set
            {
                if (value > Y)
                    Height = value - Y;
            }
        }

        public PointI Location
        {
            get { return new PointI(X, Y); }
        }

        public SizeI Size
        {
            get { return new SizeI(Width, Height); }
        }

        public RectangleI(int pX, int pY, int pWidth, int pHeight)
            : this()
        {
            X = pX;
            Y = pY;
            Width = pWidth;
            Height = pHeight;
        }

        public RectangleI(PointI pPoint, SizeI pSize)
            : this()
        {
            X = pPoint.X;
            Y = pPoint.Y;
            Width = pSize.Width;
            Height = pSize.Height;
        }

        public bool Contains(PointI pPoint)
        {
            return pPoint.X < this.Right && pPoint.X > this.Left
                && pPoint.Y > this.Top && pPoint.Y < this.Bottom;
        }

        public string ToString()
        {
            return (this.Left + ", " + this.Top + ", " + this.Right + ", " + this.Bottom);
        }

        public RectangleI Intersect(RectangleI pRectTwo)
        {
            RectangleI rect = new RectangleI(
                Math.Max(this.Left, pRectTwo.Left),
                Math.Max(this.Top, pRectTwo.Top),
                Math.Min(this.Right, pRectTwo.Right),
                Math.Min(this.Bottom, pRectTwo.Bottom));

            if (rect.Right > rect.Left
                && rect.Bottom > rect.Top)
                return rect;
            return new RectangleI();
        }

        public bool IsEmpty()
        {
            return this.Bottom < this.Top || this.Left > this.Right;
        }
    }

    public struct  Rectangle
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
            : this()
        {
            X = pX;
            Y = pY;
            Width = pWidth;
            Height = pHeight;
        }

        public Rectangle(Point pPoint, Size pSize)
            : this()
        {
            X = pPoint.X;
            Y = pPoint.Y;
            Width = pSize.Width;
            Height = pSize.Height;
        }

        public bool Contains(Point pPoint)
        {
            return pPoint.X < this.Right && pPoint.X > this.Left
                && pPoint.Y > this.Top && pPoint.Y < this.Bottom;
        }

        public string ToString()
        {
            return (this.Left + ", " + this.Top + ", " + this.Width + ", " + this.Height);
        }

        public Rectangle Intersect(Rectangle pRectTwo)
        {
            Rectangle rect = new Rectangle
            {
                Left = Math.Max(this.Left, pRectTwo.Left),
                Right = Math.Min(this.Right, pRectTwo.Right),
                Top = Math.Max(this.Top, pRectTwo.Top),
                Bottom = Math.Min(this.Bottom, pRectTwo.Bottom)
            };

            if (rect.Right > rect.Left
                && rect.Bottom > rect.Top)
                return rect;
            return new Rectangle();
        }

        public bool IsEmpty()
        {
            return this.Bottom < this.Top || this.Left > this.Right;
        }


        public override bool Equals(object obj)
        {
            if (obj is Rectangle)
            {
                var rect2 = (Rectangle)obj;
                return rect2.X == this.X 
                    && rect2.Y == this.Y 
                    && rect2.Height == this.Height 
                    && rect2.Width == this.Width;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
