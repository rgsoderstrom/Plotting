using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Plot2D_Embedded
{
    internal class ContourPlotLine // actually a polyline
    {
        public List<Point> contourPolyline = new List<Point> ();

        public ContourPlotLine (List<ContourPlotSquare> squares)
        {
            try
            {
                int index = 0; // start with first in the list
                int nextXi = -1, nextYi = -1;

                // remember where and how we started
                ContourPlotSquare startingSquare = squares [index];

                //
                // from startingSquare, search first in direction of crossings [1]
                //
                ContourPlotSquare currentSquare = startingSquare;
                ContourPlotSquare.Edge entryEdge = currentSquare.crossings [0].edge;

                while (true)
                {
                    squares.Remove (currentSquare);

                    ContourPlotSquare.Crossing exit = currentSquare.GetExitCrossing (entryEdge);
                    contourPolyline.Add (exit.point);

                    currentSquare.GetNeighborIndices (exit.edge, out nextXi, out nextYi);
                    int ii = squares.FindIndex (delegate (ContourPlotSquare s) { return s.xi == nextXi && s.yi == nextYi; });

                    if ((ii >= 0) && (ii < squares.Count))
                    {
                        currentSquare = squares [ii];
                        entryEdge = NextEntryEdge (exit.edge);
                    }
                    else
                        break;
                }

                //
                // go back to start and search the other direction
                //
                currentSquare = startingSquare;
                entryEdge = currentSquare.crossings [1].edge;

                while (true)
                {
                    squares.Remove (currentSquare);

                    ContourPlotSquare.Crossing exit = currentSquare.GetExitCrossing (entryEdge);
                    contourPolyline.Insert (0, exit.point); // note inserted at front of polyline

                    currentSquare.GetNeighborIndices (exit.edge, out nextXi, out nextYi);
                    int ii = squares.FindIndex (delegate (ContourPlotSquare s) { return s.xi == nextXi && s.yi == nextYi; });

                    if ((ii >= 0) && (ii < squares.Count))
                    {
                        currentSquare = squares [ii];
                        entryEdge = NextEntryEdge (exit.edge);
                    }
                    else
                        break;
                }
            }

            catch (Exception ex)
            {
                throw new Exception ("ContourPolyline ctor: " + ex.Message);
            }
        }

        //*********************************************************************************************

        ContourPlotSquare.Edge NextEntryEdge (ContourPlotSquare.Edge thisExitEdge)
        {
            ContourPlotSquare.Edge nextEntryEdge = ContourPlotSquare.Edge.Unknown;

            switch (thisExitEdge)
            {
                case ContourPlotSquare.Edge.Left:   nextEntryEdge = ContourPlotSquare.Edge.Right;  break;
                case ContourPlotSquare.Edge.Right:  nextEntryEdge = ContourPlotSquare.Edge.Left;   break;
                case ContourPlotSquare.Edge.Top:    nextEntryEdge = ContourPlotSquare.Edge.Bottom; break;
                case ContourPlotSquare.Edge.Bottom: nextEntryEdge = ContourPlotSquare.Edge.Top;    break;
            }

            return nextEntryEdge;
        }
    }
}









/****************

            contourPolyline.Add (startingCrossing.point);
            ContourPlotSquare.Edge entryEdge = entryEdge;

            FollowContour (startingSquare, entryEdge, squares, contourPolyline);

        }

        void FollowContour (ContourPlotSquare startingSquare, ContourPlotSquare.Edge entryEdge, List<ContourPlotSquare> squares, List<Point> contourPolyline)
        {
            while (currentSquare != null)
            {
                ContourPlotSquare.Crossing exitEdge = currentSquare.GetExitCrossing (entryEdge);
                contourPolyline.Add (exitEdge.point);


                int xi = currentSquare.xi;
                int yi = currentSquare.yi;
                int nextXi = 0, nextYi = 0;

                switch (exitEdge.edge)
                {
                    case ContourPlotSquare.Edge.Left:
                        nextXi = xi - 1;
                        nextYi = yi;
                        entryEdge = ContourPlotSquare.Edge.Right; // entry edge of next square
                        break;

                    case ContourPlotSquare.Edge.Right:
                        nextXi = xi + 1;
                        nextYi = yi;
                        entryEdge = ContourPlotSquare.Edge.Left;
                        break;

                    case ContourPlotSquare.Edge.Top:
                        nextXi = xi;
                        nextYi = yi + 1;
                        entryEdge = ContourPlotSquare.Edge.Bottom;
                        break;

                    case ContourPlotSquare.Edge.Bottom:
                        nextXi = xi;
                        nextYi = yi - 1;
                        entryEdge = ContourPlotSquare.Edge.Top;
                        break;
                }

                bool xValid = (nextXi >= 0) && (nextXi < ContourPlotView.numberXSamples);
                bool yValid = (nextYi >= 0) && (nextYi < ContourPlotView.numberYSamples);

                if (xValid && yValid)
                {
                    int ii = squares.FindIndex (delegate (ContourPlotSquare s) { return s.xi == nextXi && s.yi == nextYi; });

                    if (ii >= 0 && ii < squares.Count)
                    {
                        currentSquare = squares [ii];
                        squares.Remove (currentSquare);
                    }
                    else
                    {
                        currentSquare = null;
                    }
                }
            }
        
        }**************/
