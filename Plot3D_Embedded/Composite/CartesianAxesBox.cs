using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Petzold.Media3D;

namespace Plot3D_Embedded
{
    //*********************************************************************************************

    public partial class CartesianAxesBoxGeometry : ViewportObjectGeometry
    {
        public Point3D Minimums;
        public Point3D Maximums;

        public double MaxX {get {return Maximums.X;}}
        public double MinX {get {return Minimums.X;}}

        public double MaxY {get {return Maximums.Y;}}
        public double MinY {get {return Minimums.Y;}}

        public double MaxZ {get {return Maximums.Z;}}
        public double MinZ {get {return Minimums.Z;}}

        public CartesianAxesBoxGeometry (Point3D min, Point3D max)
        {
            Minimums = min;
            Maximums = max;

            BoundingBox.Clear();
            BoundingBox.Union (Minimums);
            BoundingBox.Union (Maximums);
        }
    }

    //****************************************************************************************************

    public partial class CartesianAxesBoxView : ModelVisual3D
    {
        CartesianAxesBoxGeometry geometry;

        public CartesianAxesBoxView (CartesianAxesBoxGeometry geom)
        {
            geometry = geom; 

            // determine where to draw tic marks
            int numberTics = 5; // along longest axis
            double dx = geometry.MaxX - geometry.MinX;
            double dy = geometry.MaxY - geometry.MinY;
            double dz = geometry.MaxZ - geometry.MinZ;

            double maxSpan = Math.Max (dx, dy);
            maxSpan = Math.Max (maxSpan, dz);
            double step = maxSpan / (numberTics + 1);

            double anchorX = (geometry.MaxX + geometry.MinX) / 2;
            double anchorY = (geometry.MaxY + geometry.MinY) / 2;
            double anchorZ = (geometry.MaxZ + geometry.MinZ) / 2;


            if (geometry.MaxX > 0 && geometry.MinX < 0) anchorX = 0;
            if (geometry.MaxY > 0 && geometry.MinY < 0) anchorY = 0;
            if (geometry.MaxZ > 0 && geometry.MinZ < 0) anchorZ = 0;


            List<double> xTics = new List<double> () {anchorX};
            List<double> yTics = new List<double> () {anchorY};
            List<double> zTics = new List<double> () {anchorZ};

            double q1 = anchorX + step;
            double q2 = anchorX - step;

            while (true)
            {
                if (xTics.Count < numberTics && q1 < geometry.MaxX) xTics.Add (q1);
                if (xTics.Count < numberTics && q2 > geometry.MinX) xTics.Add (q2);
                q1 += step;
                q2 -= step;

                if (xTics.Count >= numberTics) break;                
                if (q1 > geometry.MaxX && q2 < geometry.MinX) break;
            }



            List<double> commonTicsAt = new List<double> () {-3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8};
            AxisLineView.TicTextDisplayOptions commonTicTextDisplay = AxisLineView.TicTextDisplayOptions.Numbers;

            double commonTicSize = maxSpan / 25; // 0.2;
            double commonTicTextSize = commonTicSize; // 0.25;
            double commonTicTextOffsetDistance = commonTicSize / 2; // 0.1;

            XAxisLine xAxis1 = new XAxisLine ()
            {
                ZeroPoint = new Point3D (0, geometry.MinY, geometry.MinZ), // spot where this line would pierce the x = 0 plane
                TicsAt                = xTics, // commonTicsAt,
                //TicsAt                = commonTicsAt,
                TicTextDisplay        = commonTicTextDisplay,
                TicSize               = commonTicSize,
                TicTextSize           = commonTicTextSize,
                TicTextOffsetDistance = commonTicTextOffsetDistance,
                Color = Colors.LightGray,
                TailCoordinate = geometry.MinX,
                HeadCoordinate = geometry.MaxX,
            };

            YAxisLine yAxis1 = new YAxisLine ()
            {
                ZeroPoint = new Point3D (geometry.MinX, 0, geometry.MinZ),
                TicsAt                = yTics,
                //TicsAt                = commonTicsAt,
                TicTextDisplay        = commonTicTextDisplay,
                TicSize               = commonTicSize,
                TicTextSize           = commonTicTextSize,
                TicTextOffsetDistance = commonTicTextOffsetDistance,
                Color = Colors.LightGray,
                TailCoordinate = geometry.MinY,
                HeadCoordinate = geometry.MaxY,
            };

            ZAxisLine zAxis1 = new ZAxisLine ()
            {
                ZeroPoint = new Point3D (geometry.MinX, geometry.MinY, 0),
                TicsAt                = zTics, // commonTicsAt,
                TicTextDisplay        = commonTicTextDisplay,
                TicSize               = commonTicSize,
                TicTextSize           = commonTicTextSize,
                TicTextOffsetDistance = commonTicTextOffsetDistance,
                Color = Colors.LightGray,
                TailCoordinate = geometry.MinZ,
                HeadCoordinate = geometry.MaxZ,
            };

            XAxisLine xAxis2 = new XAxisLine (xAxis1) {ZeroPoint = new Point3D (0, geometry.MaxY, geometry.MinZ), TicsAt = null,};
            XAxisLine xAxis3 = new XAxisLine (xAxis2) {ZeroPoint = new Point3D (0, geometry.MaxY, geometry.MaxZ)};
            XAxisLine xAxis4 = new XAxisLine (xAxis2) {ZeroPoint = new Point3D (0, geometry.MinY, geometry.MaxZ)};

            YAxisLine yAxis2 = new YAxisLine (yAxis1) {ZeroPoint = new Point3D (geometry.MaxX, 0, geometry.MinZ), TicsAt = null,};
            YAxisLine yAxis3 = new YAxisLine (yAxis2) {ZeroPoint = new Point3D (geometry.MaxX, 0, geometry.MaxZ)};
            YAxisLine yAxis4 = new YAxisLine (yAxis2) {ZeroPoint = new Point3D (geometry.MinX, 0, geometry.MaxZ)};

            ZAxisLine zAxis2 = new ZAxisLine (zAxis1) {ZeroPoint = new Point3D (geometry.MaxX, geometry.MinY, 0), TicsAt = null};
            ZAxisLine zAxis3 = new ZAxisLine (zAxis2) {ZeroPoint = new Point3D (geometry.MaxX, geometry.MaxY, 0)};
            ZAxisLine zAxis4 = new ZAxisLine (zAxis2) {ZeroPoint = new Point3D (geometry.MinX, geometry.MaxY, 0)};

            Children.Add (xAxis1.View);
            Children.Add (xAxis2.View);
            Children.Add (xAxis3.View);
            Children.Add (xAxis4.View);

            Children.Add (yAxis1.View);
            Children.Add (yAxis2.View);
            Children.Add (yAxis3.View);
            Children.Add (yAxis4.View);

            Children.Add (zAxis1.View);
            Children.Add (zAxis2.View);
            Children.Add (zAxis3.View);
            Children.Add (zAxis4.View);
        }
    }

    //*******************************************************************************************

    public class CartesianAxesBox : ViewportObject
    {
        public CartesianAxesBoxGeometry Geometry;

        public override ModelVisual3D View {get {return BoxView as ModelVisual3D;}}
        public CartesianAxesBoxView BoxView {get; protected set;}

        override protected ModelVisual3D BoundingBoxView {get {return Geometry.BoundingBox.View;}}
        override public    BoundingBox3D BoundingBox     {get {return Geometry.BoundingBox;}}

        public CartesianAxesBox (Point3D minimums, Point3D maximums)
        {
            Geometry = new CartesianAxesBoxGeometry (minimums, maximums);
            BoxView = new CartesianAxesBoxView (Geometry);
        }

        public CartesianAxesBox (BoundingBox3D box)
        {
            Geometry = new CartesianAxesBoxGeometry (new Point3D (box.MinX, box.MinY, box.MinZ), new Point3D (box.MaxX, box.MaxY, box.MaxZ));
            BoxView = new CartesianAxesBoxView (Geometry);
        }
    }
}




