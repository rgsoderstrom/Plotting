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
        // Instance Variables
        //

        List<Vector> Basis; // X1 = Basis [0], X2 =Basis [1]
        Point  p1;          // location of tail, in thisBasis
        Vector v1;


        LineView ArrowView;
        LineView X1ComponentView; // projection on X1 basis vector
        LineView X2ComponentView;

        //*************************************************************
        //
        // Instance Constructors
        //

        public VectorView (Point p, Vector v, List<Vector> basis) 
        {
            Basis = basis;
            p1 = p;
            v1 = v;

            // must be in standard basis for drawing
            Point  p2 = (Point) (p1.X * Basis [0] + p1.Y * Basis [1]);
            Vector v2 =          v1.X * Basis [0] + v1.Y * Basis [1];

            ArrowView = new LineView (p2, p2 + v2);
            Add (ArrowView);
            ArrowView.ArrowheadAtEnd = true;

            X1ComponentView = new LineView (p2, p2 + v1.X * Basis [0]);
            X2ComponentView = new LineView (p2 + v1.X * Basis [0], p2 + v1.X * Basis [0] + v1.Y * Basis [1]);
            X1ComponentView.ArrowheadAtEnd = X2ComponentView.ArrowheadAtEnd = true;

            Add (X1ComponentView);
            Add (X2ComponentView);

            X1ComponentView.Color = null;
            X2ComponentView.Color = null;
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
                if (value == true) X1ComponentView.Color = X2ComponentView.Color = Brushes.LightGray;
                else               X1ComponentView.Color = X2ComponentView.Color = null;
            }

            get {return X1ComponentView.Color != null;}
        }

        public double ArrowheadSize
        {
            set
            {
                ArrowView.ArrowheadScaleFactor = value;
                X1ComponentView.ArrowheadScaleFactor = value;
                X2ComponentView.ArrowheadScaleFactor = value;
            }

            get
            {
                return ArrowView.ArrowheadScaleFactor;
            }
        }
    }
}
