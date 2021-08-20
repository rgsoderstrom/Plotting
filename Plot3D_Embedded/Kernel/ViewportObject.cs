using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Plot3D_Embedded
{
    abstract public class ViewportObject
    {
        //
        // Add this to Viewport to view the object
        //
        abstract public ModelVisual3D View {get;}

        //
        // Add this to Viewport to view the object's bounding box
        //
        abstract protected /*public*/ ModelVisual3D BoundingBoxView {get;}

        //
        // Get an objects bounding box, typically to combine it with other
        // objects boxes to get the size and location of an entire scene
        //
        abstract public BoundingBox3D BoundingBox {get;}
    }

    abstract public class ViewportObjectGeometry
    {
        public BoundingBox3D BoundingBox {get; protected set;} = new BoundingBox3D ();
    }
}
