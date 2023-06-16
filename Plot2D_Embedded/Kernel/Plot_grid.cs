using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace Plot2D_Embedded
{
    abstract internal class Plot2DGrid
    {
        protected readonly  Bare2DPlot HostPlot;
        protected Canvas InnerCanvas {get {return HostPlot.InnerCanvas;}}
        protected Canvas OuterCanvas {get {return HostPlot.OuterCanvas;}}

        public delegate double OneDXform (double d);
        protected OneDXform WorldXToCanvasX {get {return HostPlot.WorldXToCanvasX;}}
        protected OneDXform WorldYToCanvasY {get {return HostPlot.WorldYToCanvasY;}}

        protected int ticMarkLength = 7;
        protected double ticMarkThickness = 1.5;

        protected Plot2DGrid (Bare2DPlot p)
        {
            HostPlot = p;
        }

        //******************************************************************************************
        //
        // Axis Markers
        //
        protected void CalculateTicValues (ref List<double> values, double min, double max, double anchor, double ticStep)
        {
           //EventLog.WriteLine (string.Format ("CalculateTicValues: {0:0.0}, {1:0.0}, {2}, {3}", min, max, numberTics, values.Count));

            if (min == max)
                throw new Exception ("Plot2D.CalculateTicValues: min == max in CalculateTicValues");

            int numberTics = (int) ((max - min) / ticStep); // approximate

            if (numberTics < 0)
                return;



            //EventLog.WriteLine ("number tics " + numberTics.ToString ());
            //EventLog.WriteLine (max.ToString ());
            //EventLog.WriteLine (min.ToString ());
            //EventLog.WriteLine (ticStep.ToString ());

            //return;




            while (numberTics > 10)
            {
                ticStep *= 2;
                numberTics = (int) ((max - min) / ticStep);
            }

            while (numberTics < 3)
            {
                ticStep /= 2;
                numberTics = (int) ((max - min) / ticStep);
            }

             // if list is empty build the whole list
            if (values.Count == 0)
            {
                double NMin = (min - anchor) / ticStep;
                double NMax = (max - anchor) / ticStep;

                int N0 = (int)(NMin - 1);
                int N1 = (int)(NMax + 1);

                for (int N = N0; N <= N1; N++)
                {
                    double val = anchor + N * ticStep;

                    if (val >= min && val <= max)
                        values.Add (val);
                }
            }
            else
            {
                if (values [0] < min)
                {
                    values.RemoveAt (0);
                }

                if (min + ticStep < values [0])
                {
                    values.Insert (0, values [0] - ticStep);
                }

                if (values [values.Count - 1] > max)
                {
                    values.RemoveAt (values.Count - 1);
                }

                if (values [values.Count - 1] < max - ticStep)
                {
                    values.Add (values [values.Count - 1] + ticStep);
                }
            }
        }        
    }
}
