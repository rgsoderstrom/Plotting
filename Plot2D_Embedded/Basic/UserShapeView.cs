using System;
using System.Windows;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Plot2D_Embedded
{
    public abstract class UserShapeView : CanvasObject
    {
        protected class Template
        {
            public List<Point>     points = new List<Point> ();
            public List<List<int>> polyLines = new List<List<int>> (); 

            public Brush strokeColor = Brushes.Black;
            public Brush fillColor   = Brushes.Transparent;

            public BoundingBox boundingBox = new BoundingBox ();
            public List<Point> BBCorners = new List<Point> ();
        }

        //*************************************************************************

        static protected PathGeometry TemplateToGeometry (Template template)
        {
            PathGeometry geom = new PathGeometry ();

            foreach (List<int> indices in template.polyLines)
            {
                List<Point> figPoints = new List<Point> ();

                foreach (int index in indices)
                {
                    figPoints.Add (template.points [index]);
                }

                PathFigure fig = new PathFigure ();
                PolyLineSegment seg = new PolyLineSegment (figPoints, true);

                fig.StartPoint = figPoints [0];
                fig.Segments.Add (seg);

                geom.Figures.Add (fig);
            }

            return geom;
        }
    }
}
