
// 
// CartesianRect - similar to Windows Rect structure but this assumes largest Y value is at the top
// BoundingBox2D - derived from CartesianRect
// Viewport2D    - derived from CartesianRect
//

using System;
using System.Collections.Generic;
using System.Windows;

namespace Plot2D_Embedded
{
    public class BoundingBox : CartesianRect
    {
        public bool Empty {get; protected set;} = true;

        public BoundingBox () : base ()
        {
            Empty = true;
        }

        public BoundingBox (Point p1, Point p2) : base (p1, p2)
        {
            Empty = false;
        }

        public BoundingBox (Point center, double dx, double dy) : base (center, dx, dy)
        {
            Empty = false;
        }

        public BoundingBox (BoundingBox src) : base (src)
        {
            Empty = src.Empty;
        }

        public void Union (BoundingBox other)
        {
            if (Empty)
            {
                 left   = other.Left;
                 right  = other.Right;
                 top    = other.Top;
                 bottom = other.Bottom;
            }
            else
                base.Union (other);

            Empty = false;
        }

        new public void Union (Point other)
        {
            if (Empty)
            {
                double xPad = double.Epsilon; // 0; // (other.X == 0 ? 0.01 : 0.01 * Math.Abs (other.X));
                double yPad = double.Epsilon; // 0; // (other.Y == 0 ? 0.01 : 0.01 * Math.Abs (other.Y));

                left   = other.X - xPad;
                right  = other.X + xPad;
                bottom = other.Y - yPad;
                top    = other.Y + yPad;
            }
            else
                base.Union (other);

            Empty = false;
        }
        
        internal void Union (List<Point> pts)  //**************** ASSUMES CARTESIAN
        {
            foreach (Point pt in pts)
                Union (pt);
        }
        
        public void Clear ()
        {
            left   = -1e-9;
            right  =  1e-9;
            bottom = -1e-9;
            top    =  1e-9;

            Empty = true;
        }

    //    public override string ToString ()
     //   {
    //        return string.Format ("MinX = {0:0.0}, MaxX = {1:0.0}, MinY = {2:0.0}, MaxY = {3:0.0}", MinX, MaxX, MinY, MaxY);
      //  }
    }

    //************************************************************************************************************
    //************************************************************************************************************
    //************************************************************************************************************

    public class Viewport2D : CartesianRect
    {
        public Viewport2D () : base ()
        {
            //Console.WriteLine ("new Viewport 1");
        }

        public Viewport2D (Point ctr, double w, double h) : base (ctr, w, h)
        {
            //Console.WriteLine ("new Viewport 2");
        }

        public Viewport2D (BoundingBox rect) : base (rect)
        {
            //Console.WriteLine ("new Viewport 3");
        }

        public Viewport2D (CartesianRect rect) : base (rect)
        {
            //Console.WriteLine ("new Viewport 4");
        }

        //************************************************************************************************************

        double TLC_Distance {get {return Math.Sqrt (Left  * Left  + Top    * Top);}}
        double TRC_Distance {get {return Math.Sqrt (Right * Right + Top    * Top);}}
        double BLC_Distance {get {return Math.Sqrt (Left  * Left  + Bottom * Bottom);}}
        double BRC_Distance {get {return Math.Sqrt (Right * Right + Bottom * Bottom);}}

        double TLC_Angle {get {double A = Math.Atan2 (Top, Left);     if (A < 0) A += 2 * Math.PI; return A;}}
        double TRC_Angle {get {double A = Math.Atan2 (Top, Right);    if (A < 0) A += 2 * Math.PI; return A;}}
        double BLC_Angle {get {double A = Math.Atan2 (Bottom, Left);  if (A < 0) A += 2 * Math.PI; return A;}}
        double BRC_Angle {get {double A = Math.Atan2 (Bottom, Right); if (A < 0) A += 2 * Math.PI; return A;}}

        double MaxCornerDistance {get {return (Math.Max (Math.Max (TLC_Distance, TRC_Distance), Math.Max (BLC_Distance, BRC_Distance)));}}

        public double MinDistanceToOrigin   {get; protected set;} = 0;
        public double MaxDistanceFromOrigin {get; protected set;} = 0;
        public double MinThetaFromOrigin    {get; protected set;} = 0;
        public double MaxThetaFromOrigin    {get; protected set;} = 0;

        public void PolarGridCalculations ()
        {
            // clear old values, set defaults that won't bomb
            MinDistanceToOrigin   = 0;
            MaxDistanceFromOrigin = 0.1;
            MinThetaFromOrigin    = 0;
            MaxThetaFromOrigin    = 0.1;

        // flag encoding:
        //   bit 3 == 1 if origin left of Viewport
        //   bit 2 == 1 if origin right of Viewport
        //   bit 1 == 1 if origin above Viewport
        //   bit 0 == 1 if origin below Viewport
        
            byte flag = 0; // origin relative to viewport

            if (Left   >= 0) flag |= (1 << 3);
            if (Right  <= 0) flag |= (1 << 2);
            if (Bottom >= 0) flag |= (1 << 1);
            if (Top    <= 0) flag |= (1 << 0);

            switch (flag)
            {
                case 0: // origin is inside viewport
                    MinDistanceToOrigin = 0;
                    MaxDistanceFromOrigin = MaxCornerDistance;
                    MinThetaFromOrigin = 0;
                    MaxThetaFromOrigin = 2 * Math.PI;
                    break;

                case 1: // binary 0001, origin above
                    MinDistanceToOrigin   = Math.Abs (Top);
                    MaxDistanceFromOrigin = Math.Max (BLC_Distance, BRC_Distance);
                    MinThetaFromOrigin    = TLC_Angle;      
                    MaxThetaFromOrigin    = TRC_Angle;
                    break;

                case 2: // binary 0010, origin below
                    MinDistanceToOrigin   = Math.Abs (Bottom);
                    MaxDistanceFromOrigin = Math.Max (TLC_Distance, TRC_Distance);
                    MinThetaFromOrigin    = BRC_Angle;      
                    MaxThetaFromOrigin    = BLC_Angle;
                    break;

                case 4: // binary 0100, origin right
                    MinDistanceToOrigin   = Math.Abs (Right);
                    MaxDistanceFromOrigin = Math.Max (TLC_Distance, BLC_Distance);
                    MinThetaFromOrigin    = TRC_Angle;      
                    MaxThetaFromOrigin    = BRC_Angle;
                    break;

                case 5: // binary 0101, origin above and right
                    MinDistanceToOrigin   = TRC_Distance;
                    MaxDistanceFromOrigin = BLC_Distance;
                    MinThetaFromOrigin    = TLC_Angle;      
                    MaxThetaFromOrigin    = BRC_Angle;
                    break;

                case 6: // binary 0110, origin below and right
                    MinDistanceToOrigin   = BRC_Distance;
                    MaxDistanceFromOrigin = TLC_Distance;
                    MinThetaFromOrigin    = TRC_Angle;      
                    MaxThetaFromOrigin    = BLC_Angle;
                    break;

                case 8: // binary 1000, origin left
                    MinDistanceToOrigin   = Math.Abs (Left);
                    MaxDistanceFromOrigin = Math.Max (TRC_Distance, BRC_Distance);
                    MinThetaFromOrigin    = BLC_Angle;
                    MaxThetaFromOrigin    = TLC_Angle;
                    break;

                case 9: // binary 1001, origin above and left
                    MinDistanceToOrigin   = TLC_Distance;
                    MaxDistanceFromOrigin = BRC_Distance;
                    MinThetaFromOrigin    = BLC_Angle;      
                    MaxThetaFromOrigin    = TRC_Angle;
                    break;

                case 10: // binary 1010, origin below and left
                    MinDistanceToOrigin   = BLC_Distance;
                    MaxDistanceFromOrigin = TRC_Distance;
                    MinThetaFromOrigin    = BRC_Angle;      
                    MaxThetaFromOrigin    = TLC_Angle;
                    break;

                default:
                    throw new Exception ("Invalid flag value, PolarGridCalculations");

            }

            if (MaxThetaFromOrigin <= MinThetaFromOrigin)
                MaxThetaFromOrigin += 2 * Math.PI;
        }
    }

    //************************************************************************************************************
    //************************************************************************************************************
    //************************************************************************************************************

    public class CartesianRect
    {
        protected double left;   // minimum X
        protected double right;
        protected double bottom; // minimum Y
        protected double top;

        public double Left   {get {return left;}}
        public double Right  {get {return right;}}
        public double Bottom {get {return bottom;}}
        public double Top    {get {return top;}}

        public Point TLC {get {return new Point (Left,  Top);}}
        public Point TRC {get {return new Point (Right, Top);}}
        public Point BRC {get {return new Point (Right, Bottom);}}
        public Point BLC {get {return new Point (Left,  Bottom);}}

        public double MinX {get {return left;}}
        public double MaxX {get {return right;}}
        public double MinY {get {return bottom;}}
        public double MaxY {get {return top;}}

        public double Width  {get {return right - left;}}
        public double Height {get {return top - bottom;}}

        public double AspectRatio {get {return Width / Height;}}

        public Point Center {get {return new Point ((left + right) / 2, (top + bottom) / 2);}}

        public bool Contains (Point pt)
        {
            if (pt.X < Left)   return false;
            if (pt.X > Right)  return false;
            if (pt.Y < bottom) return false;
            if (pt.Y > Top)    return false;

            return true;
        }

        public CartesianRect () : this (new Point (-1, -1), new Point (1,1))
        {
        }

        public CartesianRect (Point p1, Point p2)
        {
            left   = Math.Min (p1.X, p2.X);
            right  = Math.Max (p1.X, p2.X);
            bottom = Math.Min (p1.Y, p2.Y);
            top    = Math.Max (p1.Y, p2.Y);
        }

        public CartesianRect (Point center, double dx, double dy)
        {
            left   = center.X - dx / 2;
            right  = center.X + dx / 2;
            bottom = center.Y - dy / 2;
            top    = center.Y + dy / 2;
        }

        public CartesianRect (CartesianRect src)
        {
            left   = src.Left;
            right  = src.right;
            top    = src.top;
            bottom = src.bottom;
        }

        public void Union (CartesianRect other)
        {
            left   = Math.Min (Left,   other.Left);
            right  = Math.Max (Right,  other.right);
            top    = Math.Max (Top,    other.top);
            bottom = Math.Min (Bottom, other.bottom);
        }

        public void MoveBy (Vector delta)
        {
            left   += delta.X;
            right  += delta.X;
            top    += delta.Y;
            bottom += delta.Y;
        }

        public void MoveTo (Point center)
        {
            double w = Width;
            double h = Height;

            left   = center.X - w / 2;
            right  = center.X + w / 2;
            top    = center.Y + h / 2;
            bottom = center.Y - h / 2;
        }

        public void Union (Point other)
        {
            left   = Math.Min (Left,   other.X);
            right  = Math.Max (Right,  other.X);
            top    = Math.Max (Top,    other.Y);
            bottom = Math.Min (Bottom, other.Y);
        }

        public void Offset (double dx, double dy)
        {
            left   += dx;
            right  += dx;
            top    += dy;
            bottom += dy;
        }

        public void Inflate (double dx, double dy)
        {
            Point c = Center;

            left   -= c.X;
            right  -= c.X;
            top    -= c.Y;
            bottom -= c.Y;

            left   -= dx / 2;
            right  += dx / 2;
            top    += dy / 2;
            bottom -= dy / 2;

            left   += c.X;
            right  += c.X;
            top    += c.Y;
            bottom += c.Y;
        }

        public void Scale (double sx, double sy) // scale about center
        {
            Point c = Center;

            left   -= c.X;
            right  -= c.X;
            top    -= c.Y;
            bottom -= c.Y;

            left   /= sx;
            right  /= sx;
            top    /= sy;
            bottom /= sy;

            left   += c.X;
            right  += c.X;
            top    += c.Y;
            bottom += c.Y;
        }

        public override string ToString ()
        {
            return string.Format ("MinX = {0:E03}, MaxX = {1:E03}, MinY = {2:0.0}, MaxY = {3:0.0}", MinX, MaxX, MinY, MaxY);
        }
    }
}








