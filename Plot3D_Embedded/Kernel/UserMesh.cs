using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;

namespace Plot3D_Embedded
{
    public class UserMesh3DGeometry : ViewportObjectGeometry
    {
        public MeshGeometry3D mesh = new MeshGeometry3D ();

        public UserMesh3DGeometry (List<Point3D> points, List<int> triangleIndices)
        {
            mesh.Positions = new Point3DCollection (points);
            mesh.TriangleIndices = new Int32Collection (triangleIndices);
            SetBoundingBox ();
        }

        public UserMesh3DGeometry (MeshGeometry3D m)
        {
            mesh = m;
            SetBoundingBox ();
        }

        internal void SetBoundingBox ()
        {
            BoundingBox.Clear ();

            foreach (Point3D pt in mesh.Positions)
            {
                BoundingBox.Union (pt);
            }
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class UserMesh3DView : Surface3DView
    {
        protected UserMesh3DGeometry geometry;

        static Color DefaultPointColor = Colors.LightGray;

        public UserMesh3DView (UserMesh3DGeometry geom)
        {
            geometry = geom;
            GeometryModel3D gm = new GeometryModel3D ();

            gm.Material     = new DiffuseMaterial (Brushes.Gray);
            gm.BackMaterial = new DiffuseMaterial (Brushes.Green);
            gm.Geometry = geometry.mesh;
            Content = gm;

            Color = Colors.Gray;
        }
    }

    //**************************************************************************************************************
    //**************************************************************************************************************
    //**************************************************************************************************************

    public class UserMesh3D : ViewportObject
    {
        protected override ModelVisual3D BoundingBoxView { get { return Geometry.BoundingBox.View; } }
        public    override BoundingBox3D BoundingBox { get { return Geometry.BoundingBox; } }
        public    override ModelVisual3D View { get { return MeshView as ModelVisual3D; } }

        public UserMesh3DGeometry Geometry {get; protected set;}
        public UserMesh3DView     MeshView {get; protected set;}

        public UserMesh3D (List<Point3D> points, List<int> triangleIndices)
        {
            Geometry = new UserMesh3DGeometry (points, triangleIndices);
            MeshView = new UserMesh3DView (Geometry);
        }

        public UserMesh3D (MeshGeometry3D mesh) // like from BuildHorizontalMesh
        {
            Geometry = new UserMesh3DGeometry (mesh);
            MeshView = new UserMesh3DView (Geometry);
        }
    }
}
