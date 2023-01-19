using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plot3D_Embedded
{
    public partial class Bare3DPlot
    {
        List<ViewportObject> displayObjects = new List<ViewportObject> ();

        protected BoundingBox3D viewportBoundingBox = new BoundingBox3D ();
        public    BoundingBox3D ViewportBoundingBox {get {return viewportBoundingBox;} protected set {viewportBoundingBox = value;}}

        protected void AddToViewport (ViewportObject vo)
        {
            if (Hold == false)
                displayObjects.Clear ();

            displayObjects.Add (vo);
            Draw ();
        }

        bool RemoveFromViewport (ViewportObject vo)
        {
            bool found = displayObjects.Remove (vo);
            if (found) Draw ();
            return found;
        }

        //***********************************************************************************************

        private void OrientTextToCamera ()
        {
            Vector3D Up = Camera3D.Up;
            Vector3D Right = Camera3D.Right;

            foreach (ModelVisual3D a in Viewport.Children)
            {
                if (a is Text3DView)
                {
                    if ((a as Text3DView).OrientationFrozen == false)
                        (a as Text3DView).Orientation (Up, Right);
                }
            }
        }

        //***********************************************************************************************

        // debug flag, not intended for client use
        readonly bool ShowCompositeBoundingBox = true;

        private void Draw ()
        {
            /***/
            Vector3D Up = Camera3D.Up;
            Vector3D Right = Camera3D.Right;

            foreach (ViewportObject viewObj in displayObjects)
            {
                if (viewObj is Text3D)
                {
                    if ((viewObj as Text3D).TextView.OrientationFrozen == false)
                        (viewObj as Text3D).Orientation (Up, Right);
                }
            }
            /***/

            Viewport.Children.Clear ();
            ViewportBoundingBox.Clear ();

            Viewport.Children.Add (lights.Visual);

            foreach (ViewportObject viewObj in displayObjects)
            {
                Viewport.Children.Add (viewObj.View);
                ViewportBoundingBox.Union (viewObj.BoundingBox);
            }
            
            if (ShowCompositeBoundingBox)
                Viewport.Children.Add (ViewportBoundingBox.View);



            //Camera3D.CenterOn = ViewportBoundingBox.Center;
            //Camera3D.Rho = ViewportBoundingBox.DiagonalSize * 3;   






            //if (RectangularGridOn == true)
            //{
            //    if (axes3D == null)
            //        axes3D = new CartesianAxes3D (); // new Point3D (), CartesianAxes3DGeometry.EulerAngleConventions.Fixed,0,0,0);


            //    //axes3D.XMin = -1;
            //    //axes3D.YMin = -1;
            //   // axes3D.ZMin = -1;

            //    Viewport.Children.Add (axes3D.View);
            //}
            //else
            //{
            //    if (axes3D != null)
            //    {
            //        Viewport.Children.Remove (axes3D.View);
            //        axes3D = null;
            //    }
            //}

            //if (AxesTight == true)
            //{
            //    CenterOn (ViewportBoundingBox.Center);
            //    CenterDistance = ViewportBoundingBox.DiagonalSize * 2;
            //}
        }

    }
}
