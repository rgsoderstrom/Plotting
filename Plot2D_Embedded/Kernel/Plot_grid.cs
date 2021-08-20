using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        protected void CalculateTicValues (ref List<double> values, double min, double max, int numberTics)
        {
           //EventLog.WriteLine (string.Format ("CalculateTicValues: {0:0.0}, {1:0.0}, {2}, {3}", min, max, numberTics, values.Count));

            if (min == max)
                throw new Exception ("Plot2D.CalculateTicValues: min == max in CalculateTicValues");

            if (values.Count == 0)
            {
                double start = (max + min) / 2;
                double range = max - min;
                double step = range / (numberTics + 1);

                // if 0 is in the range of values, this will ensure it appears
                if (max > 0 && min < 0) start = 0;

                if (step <= 0)                 throw new Exception ("Plot2D.CalculateTicValues: step error");
                if (double.IsNaN (range))      throw new Exception ("Plot2D.CalculateTicValues: Range NAN");
                if (double.IsInfinity (range)) throw new Exception ("Plot2D.CalculateTicValues: Range infinite");
                if (double.IsNaN (step))       throw new Exception ("Plot2D.CalculateTicValues: step NAN");
                if (double.IsInfinity (step))  throw new Exception ("Plot2D.CalculateTicValues: step infinite");

                for (double x = start; x <= max; x += step)
                    values.Add (x);

                for (double x = start - step; x >= min; x -= step)
                    values.Add (x);

                values.Sort ();
            }
            else
            {
                double step = 0;

                if (values.Count > 1)
                    step = values [1] - values [0];

                if (values [0] < min)
                {
                    values.RemoveAt (0);

                    double next = values [values.Count - 1] + step;

                    if (next <= max)
                        values.Add (next);
                }

                else if (values [values.Count - 1] > max)
                {
                    values.RemoveAt (values.Count - 1);

                    double prev = values [0] - step;

                    if (prev >= min)
                        values.Insert (0, prev);
                }
            }
        }        
    }
}
