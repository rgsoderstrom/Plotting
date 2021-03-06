using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    public partial class Bare3DPlot
    {
        protected bool hold = true;
        public bool Hold
        {get {return hold;}
         set {hold = value;}}

        protected bool axesTight = true;
        public bool AxesTight
        {get {return axesTight;}
         set {axesTight = value;}}

        public string DataAreaTitle
        {get {return DataTitle.Text;}
         set {DataTitle.Text = value; Draw ();}}

        public void MatlabStyle ()
        {
            AnotationBackground = new SolidColorBrush (Color.FromRgb (0xa8, 0xa8, 0xa8));
            OuterBorder.BorderThickness = new Thickness (0);
            OuterBorder.Margin = new Thickness (0);
            InnerBorder.BorderBrush = Brushes.Black;
            InnerBorder.BorderThickness = new Thickness (1);
        }

        //************************************************************************************************************
        //
        // Plot anything derived from ViewportObject
        //  - includes most pre-defined objects
        //

        public ModelVisual3D Plot (ViewportObject viewObj)
        {
            AddToViewport (viewObj);
            return viewObj.View;
        }

        public bool Remove (ViewportObject viewObj)
        {
            return RemoveFromViewport (viewObj);
        }

        public void Refresh ()
        {
            Draw ();
        }

        //************************************************************************************************************

        public void CenterOn (Point3D point)
        {
            Camera3D.ViewCenter = point;
        }


        //public double CenterDistance
        //{
        //    get {return RhoScrollbar.Value;}

        //    set
        //    {
        //        //RhoScrollbar.Minimum = value / 4;
        //        //RhoScrollbar.Maximum = value * 2;
        //        RhoScrollbar.Value = value;
        //    }
        //}

        //public double CenterDistance
        //{
        //    get { return Camera3D.Distance; }

        //    set
        //    {
        //        //RhoScrollbar.Minimum = value / 4;
        //        //RhoScrollbar.Maximum = value * 2;
        //        Camera3D.Distance = value;
        //    }
        //}

        public Point3D CameraPosition
        {
            get { return Camera3D.AbsPosition; }
            //set {Camera3D.Position = value;}
        }

        //************************************************************************************************************



        struct ID
        {
            public int index;
            public double distance;
        }

        List<ID> ids;

        public void SortByDistance ()
        {
            try
            {
                ids = new List<ID> ();

                for (int i = 0; i<displayObjects.Count; i++)
                {
                    ID id = new ID ();
                    id.index = i;
                    id.distance = (displayObjects [i].BoundingBox.Center - CameraPosition).Length;
                    ids.Add (id);
                }

                ids.Sort (delegate (ID p1, ID p2) { return p2.distance.CompareTo (p1.distance); });

                List<ViewportObject> dlCopy = new List<ViewportObject> ();

                foreach (ID id in ids)
                {
                    dlCopy.Add (displayObjects [id.index]);
                }

                foreach (ViewportObject vo in dlCopy)
                {
                    Viewport.Children.Remove (vo.View);
                }

                foreach (ViewportObject vo in dlCopy)
                {
                    Viewport.Children.Add (vo.View);
                }
            }

            catch (Exception ex)
            {

            }
        }


        /***
        public void SortByDistance ()
        {
            ids = new List<ID> ();

            for (int i=0; i<displayObjects.Count; i++)
            {
                ID id = new ID ();
                id.index = i;
                id.distance = (displayObjects [i].BoundingBox.Center - CameraPosition).Length;
                ids.Add (id);
            }

            ids.Sort (delegate (ID p1, ID p2) {return p2.distance.CompareTo (p1.distance);});

            List<ViewportObject> dlCopy = new List<ViewportObject> ();

            foreach (ID id in ids)
            {
                dlCopy.Add (displayObjects [id.index]);
            }

            foreach (ViewportObject vo in dlCopy)
            {
                displayObjects.Remove (vo);
            }

            foreach (ViewportObject vo in dlCopy)
            {
                displayObjects.Add (vo);
            }

            Draw ();
        }
        ***/



        //************************************************************************************************************
        //
        // Clear ()
        //

        public void Clear ()
        {
            displayObjects.Clear ();
            ViewportBoundingBox.Clear ();
            Draw ();
        }

    }
}
