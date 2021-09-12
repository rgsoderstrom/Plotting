using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

using Common;
using CommonMath;

namespace Plot2D_Embedded
{
    public partial class ContourPlotView : List<CanvasObject>
    {
        //*********************************************************************
        //
        // This is the object Plot () reads and draws
        //
        public readonly List<List<LineView>> lines = new List<List<LineView>> ();
        public BoundingBox boundingBox = new BoundingBox ();

        //******************************************************************************
        //
        // Copies of input parameters
        //
        public readonly ZFromXYFunction ZFunction;
        public readonly List<double> ContourValues;

        // for each requested level, a list of squares that level passes through
        List<List<ContourPlotSquare>> Squares;

        // ZFunction values at sample points
        public readonly List<double>      xValues;
        public readonly List<double>      yValues;
        public readonly CommonMath.Matrix zValues;

        //*********************************************************************************************

        static public bool DrawLines {get; set;} = true;
        static public bool DrawLinesInColors {get; set;} = false;

        static public bool LabelLines {get; set;} = false;
        static public double LabelFontSize {get; set;} = 0.15;

        static public bool ShowGradientArrows {get; set;} = false;
        static public double GradientArrowSize {get; set;} = 0.15; 

        static public bool ShowColoredBackground {get; set;} = false;

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        public ContourPlotView (ZFromXYFunction _z,       // calculate z from any (x, y)
                                List<double> contValues,  // draw these contours
                                double minX,              // plot limit
                                double maxX,              //  "     "
                                double minY,              //  "     "
                                double maxY,              //  "     "
                                int    numberXSamples,
                                int    numberYSamples)
        {
            ZFunction = _z;
            ContourValues = contValues;

            boundingBox.Union (new Point (minX, minY));
            boundingBox.Union (new Point (maxX, maxY));

            //
            // x, y and z values for the corners of all Contour Squares
            //

            xValues = new List<double> (numberXSamples);
            yValues = new List<double> (numberYSamples);
            zValues = new CommonMath.Matrix (numberYSamples, numberXSamples);

            double dx = (maxX - minX) / (numberXSamples - 1);
            double dy = (maxY - minY) / (numberYSamples - 1);

            for (int i = 0; i<numberXSamples; i++)
                xValues.Add (minX + i * dx);

            for (int i = 0; i<numberYSamples; i++)
                yValues.Add (minY + i * dy);

            for (int xi = 0; xi<numberXSamples; xi++)
            {
                for (int yi = 0; yi<numberYSamples; yi++)
                {
                    try
                    {
                        zValues [yi, xi] = ZFunction (xValues [xi], yValues [yi]);
                    }

                    catch (Exception)
                    {
                        zValues [yi, xi] = Double.NaN;
                    }
                }
            }

            CommonCtor ();
        }

        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        public ContourPlotView (List<double> _x,
                                List<double> _y,
                                CommonMath.Matrix _z,
                                List<double> contValues)  // draw these contours
        {
            xValues = _x;
            yValues = _y;
            zValues = _z;

            ContourValues = contValues;

            boundingBox.Union (new Point (xValues [0], yValues [0]));
            boundingBox.Union (new Point (xValues [xValues.Count - 1], yValues [yValues.Count - 1]));

            CommonCtor ();
        }


        //*********************************************************************************************
        //*********************************************************************************************
        //*********************************************************************************************

        protected void CommonCtor ()
        { 
            //
            // For each requested level, make a list of squares that level passes through
            //

            Squares = new List<List<ContourPlotSquare>> (ContourValues.Count);

            foreach (double level in ContourValues)
            {
                List<ContourPlotSquare> squares = new List<ContourPlotSquare> ();
                Squares.Add (squares);

                for (int xi = 0; xi<xValues.Count-1; xi++)
                {
                    for (int yi = 0; yi<yValues.Count-1; yi++)
                    {
                        double z1 = zValues [yi,   xi];
                        double z2 = zValues [yi+1, xi];
                        double z4 = zValues [yi+1, xi+1];
                        double z8 = zValues [yi,   xi+1];

                        int _case = 0;
                        if (z1 > level) _case += 1;
                        if (z2 > level) _case += 2;
                        if (z4 > level) _case += 4;
                        if (z8 > level) _case += 8;

                        // if some corners above contour and some below then contour passes through, so add this square to list
                        if (_case != 0 && _case != 15 && _case != 5 && _case != 10)
                            squares.Add (new ContourPlotSquare (xi, yi, _case, level, this));
                    }
                }
            }

            //*************************************************************************************************
            //
            // Make a List-of-Lists of polylines
            //  - one List for each contour level
            //  - that List contains a List of all the lines at that level
            //

            foreach (List<ContourPlotSquare> squaresForOneLevel in Squares)
            {
                List<LineView> oneLevel = new List<LineView> ();

                while (squaresForOneLevel.Count > 0)
                {
                    ContourPlotLine cp = new ContourPlotLine (squaresForOneLevel);
                    LineView line = new LineView (cp.contourPolyline);
                    oneLevel.Add (line);
                }

                lines.Add (oneLevel);
            }

            CommonCtor2 ();
        }

        //************************************************************************************************************************************

        // each contour will be drawn with one contourColor and one contourStyle
        private readonly SolidColorBrush [] contourColor  = new SolidColorBrush [] {Brushes.Black, Brushes.Red, Brushes.DarkOrange, Brushes.Gold, Brushes.Green, Brushes.Blue, Brushes.Indigo, Brushes.Violet};
        private readonly LineView.DrawingStyle [] lineStyle = new LineView.DrawingStyle [] {LineView.DrawingStyle.Solid, LineView.DrawingStyle.Dashes, LineView.DrawingStyle.Dots};

        private void CommonCtor2 ()
        {
            // Box around contours

            double width  = (boundingBox.TRC - boundingBox.TLC).Length;
            double height = (boundingBox.BLC - boundingBox.TLC).Length;
            Point  center = boundingBox.TLC + (boundingBox.BRC - boundingBox.TLC) / 2;

            RectangleView rv = new RectangleView (center, width, height);
            Add (rv);
           
            if (ShowColoredBackground)
            {
                DrawColoredBackground (rv);
            }

            if (DrawLines)
            {
                DrawContourLines ();
            }

            if (DrawLines && ShowGradientArrows)
            {
                DrawGradientArrows ();
            }

            if (DrawLines && LabelLines == true)
            {
                ShowContourLevelText ();
            }
        }

        //********************************************************************************************

        private void DrawContourLines ()
        {
            int k = 0, l = 0;

            for (int i = 0; i<lines.Count; i++)
            {
                foreach (LineView h in lines [i])
                {
                    Add (h);
                    h.Color = contourColor [k];
                    h.LineStyle = lineStyle [l];
                }

                if (DrawLinesInColors) 
                {
                    // next color and style. cycle through all colors, then change style and go through colors again
                    if (++k == contourColor.Length)
                    {
                        k = 0;
                        if (++l == lineStyle.Length)
                        {
                            l = 0;
                        }
                    }
                }
            }
        }

        //***********************************************************************************************************
        //
        // label each contour line level
        //

        private void ShowContourLevelText ()
        {
            for (int i=0; i<lines.Count; i++)
            {
                if (lines [i].Count > 0)
                {
                    // to reduce clutter just label the first line segment at each level
                    TextView txt = new TextView (lines [i][0].StartPoint, string.Format (" {0:0.0}", ContourValues [i]));
                    Add (txt);

                    txt.FontSizeAppInUnits = LabelFontSize;
                    txt.Color = lines [i][0].Color;
                }
            }
        }

        //***********************************************************************************************************
        //
        // Draw arrows near each gradient line to show direction of increasing value
        //

        private void DrawGradientArrows ()
        {
            //List<double> gradientArrowsAt = new List<double> () {1/6.0, 3/6.0, 5/6.0};
            List<double> gradientArrowsAt = new List<double> () { 0.5 };

            for (int i=0; i<lines.Count; i++)
            {
                if (lines [i].Count > 0)
                {
                    for (int j = 0; j<lines [i].Count; j++)
                    {
                        foreach (double offset in gradientArrowsAt)
                        {
                            Point P = lines [i][j].PointAtOffset (offset);

                            try
                            {
                                Vector gradient = ZFunction != null
                                    ? CommonMath.LinearAlgebra.Gradient (P, ZFunction)
                                    : CommonMath.LinearAlgebra.Gradient (P, xValues, yValues, zValues);

                                gradient.Normalize ();
                                gradient *= 0.1;

                                VectorView v = new VectorView (P, gradient);
                                v.Color = lines [i][0].Color;
                                v.ArrowheadSize = GradientArrowSize;
                                AddRange (v);
                                boundingBox.Union (P + gradient); // these can cause gap between border and contours
                            }

                            catch (Exception _)
                            {

                            }
                        }
                    }
                }
            }
        }

        //***********************************************************************************************************
        //
        // Color background proportional to function value
        //      - Red-White-Blue loaded from file and reversed to blue-white-red
        //          - negative values blue,
        //          - zero white
        //          - positive red
        //

        private void DrawColoredBackground (RectangleView rv)
        {
            Colormap cm = new Colormap ("ColormapRWB", true);

            if (cm.colors.Count == 0)
                throw new Exception ("Error loading colormap");

            BitmapPalette palette = new BitmapPalette (cm.colors);

            // bitmap bits
            int numberRows = yValues.Count;
            int numberCols = xValues.Count;

            byte[] array = new byte [numberRows * numberCols];

            double minZ = double.MaxValue;
            double maxZ = double.MinValue;

            for (int x=0; x<numberCols; x++)
            {
                for (int y=0; y<numberRows; y++)
                {
                    if (minZ > zValues [y, x]) {minZ = zValues [y, x];}
                    if (maxZ < zValues [y, x]) {maxZ = zValues [y, x];}
                }
            }

            for (int x=0; x<numberCols; x++)
            {
                for (int y=0; y<numberRows; y++)
                {
                    double val = zValues [y, x];
                    int scaled;

                    if (val < 0)
                    {
                        double d = (val - minZ) / (0 - minZ);
                        scaled = (int) (128 * d);
                    }

                    else
                    {
                        double d = val / maxZ;
                        scaled = (int) (127 + 128 * d);
                    }

                    int row = numberRows - 1 - y;
                    array [numberCols * row + x] = (byte) scaled;
                }
            }

            BitmapSource bitmap = BitmapSource.Create (numberCols, numberRows, 96, 96, PixelFormats.Indexed8, palette, array, numberCols);

            // Image
            Image img = new Image ();
            img.Source = bitmap;
            img.Stretch = Stretch.None;

            ImageBrush ib = new ImageBrush (img.Source);
            rv.path.Fill = ib;
        }
    }
}








