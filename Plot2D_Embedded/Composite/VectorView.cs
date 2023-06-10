using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Plot2D_Embedded
{
    public class VectorView : List<CanvasObject>
    {
        static List<Vector> StandardBasis = new  List<Vector> () {new Vector (1, 0), new Vector (0, 1)};

        //*************************************************************
        //
        // Instance Constructors
        //

        LineView ArrowView;
        List<CanvasObject> basisArrows = new List<CanvasObject> ();

        public VectorView (Point tail, Vector vect, List<Vector> basis) 
        {
            // must be in standard basis for drawing
            Point  p2 = (Point) (tail.X * basis [0] + tail.Y * basis [1]);
            Vector v2 =          vect.X * basis [0] + vect.Y * basis [1];

            ArrowView = new LineView (p2, p2 + v2);
            Add (ArrowView);
            ArrowView.ArrowheadAtEnd = true;

            ExpandBasisVector (p2, basis [0], vect.X, basisArrows);
            ExpandBasisVector (p2 + vect.X * basis [0], basis [1], vect.Y, basisArrows);

            foreach (CanvasObject co in basisArrows)
                Add (co);
        }

        //*************************************************************

        public VectorView (Point p1, Vector v1) : this (p1, v1, StandardBasis)
        {
        }

        //*************************************************************

        public VectorView (Vector v1) : this (new Point (0,0), v1)
        {
        }

        //*************************************************************

        public VectorView (Vector v1, List<Vector> basis) : this (new Point (0,0), v1, basis)
        {
        }

        //********************************************************************************************

        public Brush Color {get { return (this [0] as LineView).Color;} 
                            set {(this [0] as LineView).Color = value;}}

        public double Thickness {get { return (this [0] as LineView).Thickness;} 
                                 set { foreach (LineView lv in this) lv.Thickness = value;}}

        public LineView.DrawingStyle LineStyle {get { return (this [0] as LineView).LineStyle;} 
                                                set { foreach (LineView lv in this) lv.LineStyle = value;}}

        public bool ShowComponents
        {
            set
            {
                if (value == true) {foreach (CanvasObject co in basisArrows) (co as LineView).Color = Brushes.LightGray;}
                else               {foreach (CanvasObject co in basisArrows) (co as LineView).Color = null;}
            }
        }

        public double ArrowheadSize
        {
            set
            {
                ArrowView.ArrowheadScaleFactor = value;

                foreach (CanvasObject co in basisArrows)
                    (co as LineView).ArrowheadScaleFactor = value;
            }

            get
            {
                return ArrowView.ArrowheadScaleFactor;
            }
        }

        //*************************************************************************************

        private void ExpandBasisVector (Point startPoint, Vector unit, double length, List<CanvasObject> container)
        {
            if (length < 0)
            {
                length *= -1;
                unit *= -1;
            }

            int count = (int) Math.Floor (length);

            Point tail = startPoint;
            Point head = startPoint + unit;

            for (int i=0; i<count; i++)
            {
                LineView lv = new LineView (tail, head);
                lv.Color = null;
                lv.ArrowheadAtEnd = true;
                container.Add (lv);
                tail = head;
                head += unit;
            }

            double unitsRemaining = length - count; // "count" is number of unit vectors drawn

            if (unitsRemaining > 0.01)
            {
                LineView lv = new LineView (tail, tail + unitsRemaining * unit);
                lv.Color = null;
                lv.ArrowheadAtEnd = true;
                container.Add (lv);
            }
        }
    }
}
