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
         set {axesTight = value; Draw (); }}

        public string DataAreaTitle
        {get {return DataTitle.Text;}
         set {DataTitle.Text = value; Draw ();}}

        public bool axesBoxOn = false;
        public bool AxesBoxOn
        {get {return axesBoxOn;} 
         set {axesBoxOn = value; Draw ();}}

        //public bool containsTranslucent = false;
        //public bool ContainsTranslucent
        //{get {return containsTranslucent;} 
        // set {containsTranslucent = value; if (value == true) Draw (); }}

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

        // Callbacks for Camera parameters

        public delegate void RhoChanged_Callback         (object sender, double rho); // Signature of application callbacks looks like this
        public delegate void ThetaChanged_Callback       (object sender, double rho); 
        public delegate void PhiChanged_Callback         (object sender, double rho); 
        public delegate void CenterChanged_Callback      (object sender, Point3D center); 
        public delegate void AbsPositionChanged_Callback (object sender, Point3D cameraPosition); 
        public delegate void RelPositionChanged_Callback (object sender, Point3D cameraPosition); 

        private event RhoChanged_Callback         RhoChangedCallbacks;    // list of all the callbacks
        private event ThetaChanged_Callback       ThetaChangedCallbacks; 
        private event PhiChanged_Callback         PhiChangedCallbacks;
        private event CenterChanged_Callback      CenterChangedCallbacks;
        private event AbsPositionChanged_Callback AbsPositionChangedCallbacks;
        private event RelPositionChanged_Callback RelPositionChangedCallbacks;

        public void Register_RhoChanged_Callback         (RhoChanged_Callback    cb)      {RhoChangedCallbacks         += cb; cb (RhoScrollbar,   RhoScrollbar.Value); }
        public void Register_ThetaChanged_Callback       (ThetaChanged_Callback  cb)      {ThetaChangedCallbacks       += cb; cb (ThetaScrollbar, ThetaScrollbar.Value); }
        public void Register_PhiChanged_Callback         (PhiChanged_Callback    cb)      {PhiChangedCallbacks         += cb; cb (PhiScrollbar,   PhiScrollbar.Value); }
        public void Register_CenterChanged_Callback      (CenterChanged_Callback cb)      {CenterChangedCallbacks      += cb; cb (Camera3D,       Camera3D.CenterOn); }
        public void Register_AbsPositionChanged_Callback (AbsPositionChanged_Callback cb) {AbsPositionChangedCallbacks += cb; cb (Camera3D,       Camera3D.AbsPosition); }
        public void Register_RelPositionChanged_Callback (RelPositionChanged_Callback cb) {RelPositionChangedCallbacks += cb; cb (Camera3D,       Camera3D.RelPosition); }

        public void Unregister_RhoChanged_Callback         (RhoChanged_Callback    cb)      {RhoChangedCallbacks         -= cb;}
        public void Unregister_ThetaChanged_Callback       (ThetaChanged_Callback  cb)      {ThetaChangedCallbacks       -= cb;}
        public void Unregister_PhiChanged_Callback         (PhiChanged_Callback    cb)      {PhiChangedCallbacks         -= cb;}
        public void UnRegister_CenterChanged_Callback      (CenterChanged_Callback cb)      {CenterChangedCallbacks      -= cb;}
        public void Unregister_AbsPositionChanged_Callback (AbsPositionChanged_Callback cb) {AbsPositionChangedCallbacks -= cb;}
        public void Unregister_RelPositionChanged_Callback (RelPositionChanged_Callback cb) {RelPositionChangedCallbacks -= cb;}

        //************************************************************************************************************

        // Print callback

        private event PrintFunction Print;

        public void Register_PrintFunction   (PrintFunction pf) {Print += pf;}
        public void Unregister_PrintFunction (PrintFunction pf) {Print -= pf;}

        //************************************************************************************************************

        public Point3D CenterOn
        {
            get {return Camera3D.CenterOn;}

            set
            {
                Camera3D.CenterOn = value;
                CenterChangedCallbacks?.Invoke (Camera3D, value);
                AbsPositionChangedCallbacks?.Invoke (Camera3D, Camera3D.AbsPosition);
            }
        }

        public double CenterDistance
        {
            get {return RhoScrollbar.Value;}

            set
            {
                RhoScrollbar.Minimum = value / 4;
                RhoScrollbar.Maximum = value * 2;
                RhoScrollbar.Value = value;
            }
        }

        public Point3D CameraPosition
        {
            get {return Camera3D.AbsPosition;}
            set {AxesTight = false;  Camera3D.AbsPosition = value;}
        }

        public Point3D CameraRelPosition
        {
            get {return Camera3D.RelPosition;}
            set {Camera3D.RelPosition = value;}
        }

        //************************************************************************************************************



        struct ID
        {
            public int index;
            public double distance;
        };

        List<ID> ids;

        private void SortByDistance ()
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
            //ContainsTranslucent = false;
            displayObjects.Clear ();
            ViewportBoundingBox.Clear ();
            Draw ();
        }

    }
}
