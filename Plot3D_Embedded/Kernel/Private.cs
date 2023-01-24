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

        protected bool RemoveFromViewport (ViewportObject vo)
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
        readonly bool ShowCompositeBoundingBox = false;

        private void Draw ()
        {
            // Orient text to the camera
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

            Viewport.Children.Clear ();
            ViewportBoundingBox.Clear ();

            Viewport.Children.Add (lights.Visual);

            bool hasTranslucent = false;

            foreach (ViewportObject viewObj in displayObjects)
            {
                if (viewObj.View is Surface3DView)
                {
                    if ((viewObj.View as Surface3DView).Opacity != 1)
                    {
                        hasTranslucent = true;
                        break;
                    }
                }

                Viewport.Children.Add (viewObj.View);
                ViewportBoundingBox.Union (viewObj.BoundingBox);
            }

            if (ShowCompositeBoundingBox)
                Viewport.Children.Add (ViewportBoundingBox.View);


            if (AxesTight == true)
            {
                Camera3D.CenterOn = ViewportBoundingBox.Center;
                Camera3D.Rho = ViewportBoundingBox.DiagonalSize * 3;   
            }

            if (AxesBoxOn == true)
            {
                CartesianAxesBox axes = new CartesianAxesBox (ViewportBoundingBox);  //<<<<<<<<<< SLOW
                Viewport.Children.Add (axes.View);
            }

            if (hasTranslucent) 
                SortByDistance ();
        }
    }
}
