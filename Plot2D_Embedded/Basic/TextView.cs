using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

using System.Windows.Controls;

namespace Plot2D_Embedded
{
    public class TextView : CanvasObject
    {
        public override UIElement View {get {return textBlock;}}
        private readonly TextBlock textBlock;

        //*************************************************************
        //
        // Instance Constructors
        //

        public TextView (Point p1, string txt)
        {
            Position = p1;

            List<Point> bbCorners = new List<Point> () {p1, p1 + new Vector (txt.Length * FontSizeAppInUnits, -FontSizeAppInUnits)};
            CalculateBB (bbCorners);


            path = null;


            textBlock = new TextBlock ();
            textBlock.Text = txt;
            textBlock.RenderTransform = rot;

            textBlock.Foreground = Brushes.Red; // TextColor;
            TextBlock.SetFontSize (textBlock, 12);

            Angle = 0;
        }

        //**************************************************************

        public Point Position {get; set;}

        double fontSizeAppInUnits = 1;

        public double FontSizeAppInUnits
        {
            get {return fontSizeAppInUnits;}
            set {fontSizeAppInUnits = value; /* SetBoundingBox (); */}
        }

        public Brush Color {get {return textBlock.Foreground;} set {textBlock.Foreground = value;}}

        //**************************************************************

        private RotateTransform rot = new RotateTransform (0);

        public double Angle 
        {
            get 
            {
                return -1 * rot.Angle; // "-1" makes passed-in angle increase CCW
            }   
                             
            set 
            {
                rot.Angle = -1 * value;
                SetBoundingBox ();
            }
        }

        //**************************************************************

        public void SetFontSizeWPF (double ySlope)
        {
            TextBlock.SetFontSize (textBlock, FontSizeAppInUnits * Math.Abs (ySlope));
        }

        //**************************************************************


        private void SetBoundingBox ()
        {
            // both theses sizes are very approximate
            double xs = FontSizeAppInUnits;
            double ys = FontSizeAppInUnits;
            //double xs = textBlock.Text.Length * FontSizeAppInUnits;
            //double ys = FontSizeAppInUnits * 1.2;

            RotateTransform bbRot = new RotateTransform (Angle, Position.X, Position.Y);

            List<Point> bbCorners = new List<Point> (); 
            bbCorners.Add (bbRot.Transform (Position));
            bbCorners.Add (bbRot.Transform (Position + new Vector (xs, 0)));
            bbCorners.Add (bbRot.Transform (Position + new Vector (0, -ys)));
            bbCorners.Add (bbRot.Transform (Position + new Vector (xs, -ys)));

            BoundingBox.Clear ();
            BoundingBox.Union (bbCorners [0]);
            BoundingBox.Union (bbCorners [1]);
            BoundingBox.Union (bbCorners [2]);
            BoundingBox.Union (bbCorners [3]);
        }
    }
}
