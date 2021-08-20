using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    partial class CartesianAxes3DView
    {
        internal void RedrawX ()
        {
            if (geometry.localCoords.MinX <= geometry.localCoords.MaxX)
            {
                Children.Remove (xAxis.View);
                xAxis = new Line3D (geometry.worldCoords.MinX, geometry.worldCoords.MaxX);
                xAxis.LineView.SetDashParameters (dp);
                xAxis.LineView.Color = Colors.Red;
                Children.Add (xAxis.View);
            }
        }

        internal void RedrawY ()
        {
            if (geometry.localCoords.MinY <= geometry.localCoords.MaxY)
            {
                Children.Remove (yAxis.View);
                yAxis = new Line3D (geometry.worldCoords.MinY, geometry.worldCoords.MaxY);
                yAxis.LineView.SetDashParameters (dp);
                yAxis.LineView.Color = Colors.Green;
                Children.Add (yAxis.View);
            }
        }

        internal void RedrawZ ()
        {
            if (geometry.localCoords.MinZ <= geometry.localCoords.MaxZ)
            {
                Children.Remove (zAxis.View);
                zAxis = new Line3D (geometry.worldCoords.MinZ, geometry.worldCoords.MaxZ);
                zAxis.LineView.SetDashParameters (dp);
                zAxis.LineView.Color = Colors.Blue;
                Children.Add (zAxis.View);
            }
        }
    }
}
