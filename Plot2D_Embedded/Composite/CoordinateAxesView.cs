using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Plot2D_Embedded
{
    internal class AxisLine
    {
        //*************************************************************************

        RotateTransform rot;
        ScaleTransform stretch;
        TranslateTransform move;
        TransformGroup xform;
        Point Start, End;
        //bool drawArrowhead = false;
        //double arrowHeadScaleFactor = 1;
        LineView axisView;
        List<double> ticValues = new List<double> ();


        internal AxisLine (Point origin, 
                           double lengthLeft, double lengthRight, 
                           double firstTic, double ticStep, bool absCoords, double ticScaleFactor,
                           double angle, double scale, Brush color, List<CanvasObject> lst)
        {
            double ticHalfLength = lengthRight / 40;

            rot = new RotateTransform (angle);
            stretch = new ScaleTransform (scale, scale);
            move = new TranslateTransform (origin.X, origin.Y);
            xform = new TransformGroup ();
            xform.Children.Add (stretch);
            xform.Children.Add (rot);
            xform.Children.Add (move);

            Vector dir = new Vector (1, 0); // all are initially drawn in this orientation,
                                            // then rotated to final

            Start = xform.Transform (new Point (0, 0) - dir * lengthLeft);
            End   = xform.Transform (new Point (0, 0) + dir * lengthRight);
            axisView = new LineView (Start, End);
            axisView.ArrowheadAtEnd = true;
            axisView.ArrowheadScaleFactor = ticHalfLength; // scale;
            axisView.Color = color;
            lst.Add (axisView);

            for (double x = firstTic; x < lengthRight; x += ticStep)
                ticValues.Add (x);
            
            for (double x = -firstTic; x > -lengthLeft; x -= ticStep)
                ticValues.Add (x);

            TicMarksAndLabels (origin, ticValues, ticHalfLength * ticScaleFactor, absCoords, xform, color, lst);
        }

        //*************************************************************************************************************************

        private void TicMarksAndLabels (Point origin, List<double> values, double ticHalfLength, bool absCoords, TransformGroup xform, Brush color, List<CanvasObject> lst)
        {
            foreach (double x in values)
            {
                LineView tic = new LineView (xform.Transform (new Point (x, ticHalfLength)), xform.Transform (new Point (x, -ticHalfLength)));
                tic.Color = color;
                lst.Add (tic);

                TextView lbl = absCoords ? new TextView (xform.Transform (new Point (x, -ticHalfLength)), (origin.X + x).ToString ())
                                         : new TextView (xform.Transform (new Point (x, -ticHalfLength)), x.ToString ());

                lbl.FontSizeAppInUnits = 2 * ticHalfLength;
                lbl.Color = color;
                lst.Add (lbl);
            }
        }
    }

    //**************************************************************************************************
    //**************************************************************************************************
    //**************************************************************************************************
    //**************************************************************************************************

    public class CoordinateAxesView : List<CanvasObject>
    {
        //******************************************************************

        // Copy of all parameters needed to draw

        Point  localOrigin;
        bool   absoluteCoordinates; // tic mark values can be displayed as absolute or relative
        double negAxisLength;
        double posAxisLength;
        double ticScaleFactor;
        double firstTic;
        double ticStep;

        //******************************************************************

        private void Draw ()
        {
            AxisLine X = new AxisLine (localOrigin, negAxisLength, posAxisLength, firstTic, ticStep, absoluteCoordinates, ticScaleFactor, 0, 1, Brushes.Red, this);
            AxisLine Y = new AxisLine (localOrigin, negAxisLength, posAxisLength, firstTic, ticStep, absoluteCoordinates, ticScaleFactor, 90, 1, Brushes.Green, this);
        }

        //******************************************************************

        public double FirstTic
        {
            get {return firstTic;}
            set {firstTic = value; Clear (); Draw ();}
        }

        //******************************************************************

        public double TicStep
        {
            get {return ticStep;}
            set {ticStep = value; Clear (); Draw ();}
        }

        //******************************************************************

        public bool PositiveOnly {
            set 
            {   
                Clear ();

                if (value == true) negAxisLength = 0;
                else               negAxisLength = posAxisLength;

                Draw ();
            }
        }

        //******************************************************************

        public CoordinateAxesView (Point  _localOrigin,
                                   bool   _absoluteCoordinates, // tic mark values can be displayed as absolute or relative
                                   double _negAxisLength,
                                   double _posAxisLength,
                                   double _ticScaleFactor,  // affects drawing size only
                                   double _firstTic, 
                                   double _ticStep)
        {
            localOrigin         = _localOrigin;
            absoluteCoordinates = _absoluteCoordinates;
            negAxisLength       = _negAxisLength;      
            posAxisLength       = _posAxisLength;      
            ticScaleFactor      = _ticScaleFactor;     
            firstTic            = _firstTic;           
            ticStep             = _ticStep;

            Draw ();

           // AxisLine X1 = new AxisLine (localOrigin, negAxisLength, posAxisLength, firstTic, ticStep, absoluteCoordinates, ticScaleFactor, 20, 1.14, Brushes.LightPink, this);
           // AxisLine Y1 = new AxisLine (localOrigin, negAxisLength, posAxisLength, firstTic, ticStep, absoluteCoordinates, ticScaleFactor, 70, 1.14, Brushes.LightGreen, this);
        }

        //public CoordinateAxesView (double axisLength, double firstTic, double ticStep) : this (new Point (3,2), axisLength, firstTic, ticStep)
        //{
        //}

        //public CoordinateAxesView ()
        //{
        //}
    }
}
