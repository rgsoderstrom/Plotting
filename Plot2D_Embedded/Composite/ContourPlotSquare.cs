using System;
using System.Collections.Generic;
using System.Windows;

using CommonMath;

//
// ContourPlotSquare - implement functionality requred by each square in the
//                     "Marching Squares" contour plot generation algorithm
//                     See https://en.wikipedia.org/wiki/Marching_squares
//

namespace Plot2D_Embedded
{   
    internal class ContourPlotSquare
    {
        public enum Edge {Unknown, Left, Top, Right, Bottom};

        public struct Crossing
        {
            public Crossing (Edge ed, Point pt)
            {
                edge = ed;
                point = pt;
            }

            public readonly Edge edge;
            public readonly Point point;

            public override string ToString ()
            {
                return edge.ToString ();
            }
        }
        readonly public Crossing [] crossings = new Crossing [2];

        public int xi; // index of blc
        public int yi;
        public int _case; // see Wikipedia article

        //*****************************************************************************************

        public Crossing GetExitCrossing (Edge enteringEdge)
        {
            if (crossings [0].edge == enteringEdge) return crossings [1];
            if (crossings [1].edge == enteringEdge) return crossings [0];

            throw new Exception ("ContourPlotSquare.GetExitCrossing failed");
        }

        public void GetNeighborIndices (Edge thisSquareExitEdge, out int nextXi, out int nextYi)
        {
            nextXi = nextYi = -1;

            switch (thisSquareExitEdge)
            {
                case Edge.Left:
                    nextXi = xi - 1;
                    nextYi = yi;
                    break;

                case Edge.Right:
                    nextXi = xi + 1;
                    nextYi = yi;
                    break;

                case Edge.Top:
                    nextXi = xi;
                    nextYi = yi + 1;
                    break;

                case Edge.Bottom:
                    nextXi = xi;
                    nextYi = yi - 1;
                    break;
            }
        }

        //*****************************************************************************************

        public ContourPlotSquare (int x, int y, int c, double contourLevel, ContourPlotView top)
        {
            xi = x;
            yi = y;
            _case = c;

            Point blc = new Point (xi, yi);
            Point brc = new Point (xi+1, yi);
            Point tlc = new Point (xi,   yi+1);
            Point trc = new Point (xi+1, yi+1);
            //Point blc = new Point (top.xValues [xi],   top.yValues [yi]);
            //Point brc = new Point (top.xValues [xi+1], top.yValues [yi]);
            //Point tlc = new Point (top.xValues [xi],   top.yValues [yi+1]);
            //Point trc = new Point (top.xValues [xi+1], top.yValues [yi+1]);

            switch (_case)
            {
                case 0:
                case 15: throw new Exception ("Error in ContourPlotSquare"); 

                case 1: 
                case 14:
                    crossings [0] = new Crossing (Edge.Left,   Interpolate (blc, tlc, contourLevel, top));
                    crossings [1] = new Crossing (Edge.Bottom, Interpolate (blc, brc, contourLevel, top));
                    break; 

                case 2:
                case 13:
                    crossings [0] = new Crossing (Edge.Left, Interpolate (blc, tlc, contourLevel, top));
                    crossings [1] = new Crossing (Edge.Top,  Interpolate (tlc, trc, contourLevel, top));
                    break; 

                case 3:
                case 12:
                    crossings [0] = new Crossing (Edge.Top,    Interpolate (tlc, trc, contourLevel, top));
                    crossings [1] = new Crossing (Edge.Bottom, Interpolate (blc, brc, contourLevel, top));
                    break; 

                case 4:
                case 11:
                    crossings [0] = new Crossing (Edge.Top,   Interpolate (tlc, trc, contourLevel, top));
                    crossings [1] = new Crossing (Edge.Right, Interpolate (brc, trc, contourLevel, top));
                    break;

                case 5: 
                case 10: throw new Exception ("Contour Plot failed - try increasing resolution");
                  //break;  // could happen but not supported. Avoid these cases by increasing plot resolution 
                            // (i.e. increase numberXSamples and numberYSamples)

                case 6:
                case 9:
                    crossings [0] = new Crossing (Edge.Left,  Interpolate (tlc, blc, contourLevel, top));
                    crossings [1] = new Crossing (Edge.Right, Interpolate (trc, brc, contourLevel, top));
                    break; 

                case 7: 
                case 8: 
                    crossings [0] = new Crossing (Edge.Bottom, Interpolate (blc, brc, contourLevel, top));
                    crossings [1] = new Crossing (Edge.Right,  Interpolate (brc, trc, contourLevel, top));
                    break; 

                default: break;
            }
        }
     
        //************************************************************************************

        // linear interpolation

        //static Point Interpolate (Point p1, Point p2, double z, ZFromXYFunction func)
        //{
        //    double z1 = func (p1.X, p1.Y);
        //    double z2 = func (p2.X, p2.Y);
        //    Vector v = p2 - p1;

        //    return p1 + v * (z - z1) / (z2 - z1);
        //}

        Point Interpolate (Point p1, Point p2, double z, ContourPlotView top)
        {
            Point pt1 = new Point (top.xValues [(int)p1.X], top.yValues [(int)p1.Y]);
            Point pt2 = new Point (top.xValues [(int)p2.X], top.yValues [(int)p2.Y]);

            double z1 = top.zValues [(int)p1.Y, (int)p1.X];
            double z2 = top.zValues [(int)p2.Y, (int)p2.X];
            Vector v = pt2 - pt1;

            return pt1 + v * (z - z1) / (z2 - z1);
        }

        //************************************************************************************

        public override string ToString ()
        {
            return string.Format ("xi = {0}, yi = {1}, case = {2}, {3}, {4}", xi, yi, _case, crossings [0], crossings [1]);
        }
    }
}
