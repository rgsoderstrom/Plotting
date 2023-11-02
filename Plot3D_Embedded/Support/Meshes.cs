﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Common;
using CommonMath;

namespace Plot3D_Embedded
{
    public static partial class Utils
    {
        //
        // Vertical mesh, used to build "cylinders":
        //
        // Cylinder - surface generated by a straight line that moves parallel to a given
        //            line and passes through a given curve
        //
        // - here the "given line" is the z axis and the "given curve" is in the x-y plane,
        //   specified by XDomain and YRange
        //

        static internal MeshGeometry3D BuildVerticalMesh (double xMin, double xMax,
                                                          ddFunction xyMapping, // YFromXFunction xyMapping,
                                                          double zMin, double zMax)
        {
            MeshGeometry3D mesh = new MeshGeometry3D ();
            mesh.Positions = new Point3DCollection ();
            mesh.TriangleIndices = new Int32Collection ();


            int Layers = 2; // along z axis --- MAKE THESE PROPERTIES, RE-CALC MESH ON CHANGE

            // Generate the points. Start from minimum z and minimum x. Work
            // to maximum x, then next z

            List<Point> xyPoints = LinearAlgebra.PartitionCurve (xyMapping, xMin, xMax);

            double zStep = (zMax - zMin) / (Layers - 1);

            for (int p = 0; p<Layers; p++)
            {
                double z = zMin + p * zStep;

                foreach (Point pt in xyPoints)
                    mesh.Positions.Add (new Point3D (pt.X, pt.Y, z));
            }

            // triangle indices            
            int bottomRow = 0;

            int pointsPerLayer = xyPoints.Count;

            while (bottomRow<Layers-1)
            {
                int leftCol = 0;

                while (leftCol<pointsPerLayer-1)
                {
                    // 4 corners of a rectangle that will be drawn as 2 triangles
                    int blc = bottomRow * pointsPerLayer + leftCol; // bottom left corner
                    int brc = blc + 1;
                    int tlc = blc + pointsPerLayer;
                    int trc = tlc + 1;

                    mesh.TriangleIndices.Add (blc);
                    mesh.TriangleIndices.Add (tlc);
                    mesh.TriangleIndices.Add (brc);

                    mesh.TriangleIndices.Add (brc);
                    mesh.TriangleIndices.Add (tlc);
                    mesh.TriangleIndices.Add (trc);

                    leftCol++;
                }

                bottomRow++;
            }

            return mesh;
        }

        //***********************************************************************************************************
        //***********************************************************************************************************
        //***********************************************************************************************************
        //
        // Horizontal mesh - z is a function of (x, y)
        //

        static internal MeshGeometry3D BuildHorizontalMesh (List<double> xCoords,
                                                            List<double> yCoords,
                                                            double [, ] zValues)
        {
            int xCount = xCoords.Count;
            int yCount = yCoords.Count;

            MeshGeometry3D mesh = new MeshGeometry3D ();
            mesh.Positions = new Point3DCollection ();
            mesh.TriangleIndices = new Int32Collection ();
            mesh.TextureCoordinates = new PointCollection ();

            //
            // Store points in 1D lists as WPF requires. All stored by row, i.e. x index varies more rapidly.
            //

            for (int yi = 0; yi<yCount; yi++)
            {
                int yNext = (yi < yCount - 1) ? (yi + 1) : (yi - 1);
                int yMult = (yNext > yi) ? 1 : -1;

                for (int xi = 0; xi<xCount; xi++)
                {
                    // normal vector
                    int xNext = (xi < xCount - 1) ? (xi + 1) : (xi - 1);
                    int xMult = (xNext > xi) ? 1 : -1;

                    Point3D here  = new Point3D (xCoords [xi],    yCoords [yi],    zValues [yi, xi]);
                    Point3D nextX = new Point3D (xCoords [xNext], yCoords [yNext], zValues [yi, xNext]);
                    Point3D nextY = new Point3D (xCoords [xi],    yCoords [yNext], zValues [yNext, xi]);

                    Vector3D vx = (nextX - here) * xMult;
                    Vector3D vy = (nextY - here) * yMult;                    
                    mesh.Normals.Add (Vector3D.CrossProduct (vx, vy));

                    // position
                    mesh.Positions.Add (here);                    

                    // texture - color the mesh based on z value
                    mesh.TextureCoordinates.Add (new Point (zValues [yi, xi], zValues [yi, xi]));
                }
            }

            //
            // triangles in format required by WPF
            //

            for (int xi=0;  xi<xCount-1; xi++)
            {
                for (int yi=0;  yi<yCount-1; yi++)
                {
                    // 4 corners of a rectangle that will be drawn as 2 triangles
                    int blc = yi * xCount + xi; // bottom left corner
                    int brc = blc + 1;
                    int tlc = blc + xCount;
                    int trc = tlc + 1;

                    mesh.TriangleIndices.Add (blc);
                    mesh.TriangleIndices.Add (brc);
                    mesh.TriangleIndices.Add (tlc);

                    mesh.TriangleIndices.Add (brc);
                    mesh.TriangleIndices.Add (trc);
                    mesh.TriangleIndices.Add (tlc);
                }
            }

            return mesh;
        }

        //***********************************************************************************************************

        //static internal MeshGeometry3D BuildHorizontalMesh (double x1, double x2, int xSamples,
        //                                                    double y1, double y2, int ySamples,
        //                                                    ZFromXYFunction ZFunction)
        //{
        //    //
        //    // first generate z data in 2-dimensional x,y array
        //    //
        //    Point3D [,] surface = new Point3D [xSamples, ySamples];

        //    double dx = (x2 - x1) / (xSamples - 1);
        //    double dy = (y2 - y1) / (ySamples - 1);

        //    for (int yi = 0; yi<ySamples; yi++)
        //    {
        //        double y = y1 + yi * dy; // common to all points in this yc loop

        //        for (int xi = 0; xi<xSamples; xi++)
        //        {
        //            double x = x1 + xi * dx;
        //            double z = ZFunction (x, y);
        //            surface [xi, yi] = new Point3D (x, y, z);
        //        }
        //    }

        //    //
        //    // look for NaN or infinite z data
        //    //

        //    bool [,] pointValid = new bool [xSamples, ySamples];

        //    for (int yi = 0; yi<ySamples; yi++)
        //    {
        //        for (int xi = 0; xi<xSamples; xi++)
        //        {
        //            pointValid [xi, yi] = Utils.IsValidZ (surface [xi, yi]);
        //        }
        //    }





        //    //
        //    // get count of valid neighbors for all points
        //    //
        //    int [,] numberValidNeighbors = new int [xSamples, ySamples];

        //    for (int xi = 0; xi<xSamples; xi++)
        //    {
        //        for (int yi = 0; yi<ySamples; yi++)
        //        {                    
        //            if (pointValid [xi, yi] == true) // if this point is good, incr neighbor count for all of its neighbors
        //            {
        //                for (int xi2 = xi-1; xi2<=xi+1; xi2++)
        //                {
        //                    for (int yi2 = yi-1; yi2<=yi+1; yi2++)
        //                    {
        //                        if (ValidIndex (xi2, yi2, xSamples, ySamples))
        //                        {
        //                            if (xi2 != xi || yi2 != yi) // don't incr its own neighbor count
        //                                numberValidNeighbors [xi2, yi2]++;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //
        //    // find points that should be moved, i.e. invalid points with valid neighbors
        //    //
        //    List<MoveTargetInfo> pointsToMove = new List<MoveTargetInfo> ();

        //    for (int xi = 0; xi<xSamples; xi++)
        //    {
        //        for (int yi = 0; yi<ySamples; yi++)
        //        {
        //            if (pointValid [xi, yi] == false)
        //            {
        //                if (numberValidNeighbors [xi, yi] > 0)
        //                {
        //                    MoveTargetInfo tgt = new MoveTargetInfo (xi, yi);

        //                    for (int xi2 = xi-1; xi2<=xi+1; xi2++)
        //                    {
        //                        for (int yi2 = yi-1; yi2<=yi+1; yi2++) 
        //                        {                                      
        //                            if (ValidIndex (xi2, yi2, xSamples, ySamples)) // we know [xi, yi] is bad, so don't need to test
        //                            {                                              // xi2 == xi && yi2 == yi
        //                                if (pointValid [xi2, yi2] == true)
        //                                    tgt.neighbors.Add (new IndexPair (xi2, yi2));
        //                            }
        //                        }
        //                    }

        //                    pointsToMove.Add (tgt);
        //                }
        //            }
        //        }
        //    }

        //    //
        //    // move them
        //    //

        //    foreach (MoveTargetInfo tgt in pointsToMove)
        //    {
        //        if (tgt.neighbors.Count == 0)
        //            throw new Exception ("MoveTargetInfo entry has no neighbors");

        //        // find centroid of all neighbors
        //        double xc = 0, yc = 0;

        //        foreach (IndexPair ip in tgt.neighbors)
        //        {
        //            xc += surface [ip.xi, ip.yi].X;
        //            yc += surface [ip.xi, ip.yi].Y;
        //        }

        //        xc /= tgt.neighbors.Count;
        //        yc /= tgt.neighbors.Count;

        //        Point3D centroid = new Point3D (xc, yc, ZFunction (xc, yc));
        //        surface [tgt.target.xi, tgt.target.yi] = FindTransition (surface [tgt.target.xi, tgt.target.yi], centroid, ZFunction);
        //    }


        //    //**********************************************************************************************************
        //    //**********************************************************************************************************
        //    //**********************************************************************************************************
           

        //    MeshGeometry3D mesh = new MeshGeometry3D ();
        //    mesh.Positions = new Point3DCollection ();
        //    mesh.TriangleIndices = new Int32Collection ();


        //    //
        //    // Store points in 1D list as WPF requires. points stored by row, i.e. x index varies more rapidly.
        //    // Invalid points are also stored. they will not be used to draw triangles
        //    //
        //    List<Point3D> Positions = new List<Point3D> ();

        //    for (int yi = 0; yi<ySamples; yi++)
        //    {
        //        for (int xi = 0; xi<xSamples; xi++)
        //        {
        //            mesh.Positions.Add (surface [xi, yi]);
        //        }
        //    }

        //    //
        //    // triangles in format required by WPF
        //    //

        //    List<int> TriangleIndices = new List<int> ();

        //    for (int xi=0;  xi<xSamples-1; xi++)
        //    {
        //        for (int yi=0;  yi<ySamples-1; yi++)
        //        {
        //            // 4 corners of a rectangle that will be drawn as 0, 1 or 2 triangles
        //            int blc = yi * xSamples + xi; // bottom left corner
        //            int brc = blc + 1;
        //            int tlc = blc + xSamples;
        //            int trc = tlc + 1;

        //            int validCount = 0;
        //            validCount += pointValid [xi, yi]         ? 1 : 0;
        //            validCount += pointValid [xi, yi + 1]     ? 1 : 0;
        //            validCount += pointValid [xi + 1, yi]     ? 1 : 0;
        //            validCount += pointValid [xi + 1, yi + 1] ? 1 : 0;

        //            if (validCount > 0)
        //            {
        //                mesh.TriangleIndices.Add (blc);
        //                mesh.TriangleIndices.Add (tlc);
        //                mesh.TriangleIndices.Add (brc);

        //                mesh.TriangleIndices.Add (brc);
        //                mesh.TriangleIndices.Add (tlc);
        //                mesh.TriangleIndices.Add (trc);
        //            }
        //        }
        //    }

        //    return mesh;
        //}




    public struct IndexPair
    {
        public int xi;
        public int yi;

        public IndexPair (int x, int y)
        {
            xi = x;
            yi = y;
        }
    }

    struct MoveTargetInfo
    {
        public IndexPair target; // the invalid point

        public List<IndexPair> neighbors;
        
        public MoveTargetInfo (int xi, int yi)
        {
            target = new IndexPair ();
            target.xi = xi;
            target.yi = yi;

            neighbors = new List<IndexPair> ();
        }

        public override string ToString ()
        {
            string str = string.Format ("({0}, {1})", target.xi, target.yi)
                       + string.Format ("   {0} neighbors", neighbors.Count);

            foreach (IndexPair p in neighbors)
                str += string.Format ("   ({0}, {1})\n", p.xi, p.yi);

            return str;
        }
    }





        static bool ValidIndex (int xi, int yi, int xSamples, int ySamples)
        {
            return (xi >= 0) && (xi < xSamples) && (yi >= 0) && (yi < ySamples);
        }

        static Point3D FindCenter (Point3D p1, Point3D p2, ZFromXYFunction zfunc)
        {
            double xc = (p1.X + p2.X) / 2;
            double yc = (p1.Y + p2.Y) / 2;
            return new Point3D (xc, yc, zfunc (xc, yc));
        }

        static Point3D FindTransition (Point3D left, Point3D right, ZFromXYFunction func)
        {
            try
            {
                Point3D center = FindCenter (left, right, func);

                double maxLRDifference = 1e-6; // 1e-6;
                int maxLoopCount = 32;
                int loopCount = 0;

                while (true)
                {
                    bool leftValid = Utils.IsValidZ (left);
                    bool rightValid = Utils.IsValidZ (right);
                    bool centerValid = Utils.IsValidZ (center);

                    if (!leftValid && rightValid)
                    {
                        if (centerValid)
                            right = center;
                        else
                            left = center;
                    }
                    else if (leftValid && !rightValid)
                    {
                        if (centerValid)
                            left = center;
                        else
                            right = center;
                    }
                    else
                    {
                        throw new Exception ("FindTransition logic error");
                    }

                    center = FindCenter (left, right, func);

                    //Console.WriteLine ("({0:0.000}),   ({1:0.000}),   ({2:0.000})", left, center, right);


                    Point leftXY = new Point (left.X, left.Y);
                    Point rightXY = new Point (right.X, right.Y);

                    if ((rightXY - leftXY).Length < maxLRDifference)
                    {
                        //EventLog.WriteLine (string.Format ("lc {0}", loopCount));
                        break;
                    }

                    if (++loopCount == maxLoopCount)
                        throw new Exception (string.Format ("FindTransition loop count, dist = {0:0.00}", (right - left).Length));
                }
            }

            catch (Exception ex)
            {
                EventLog.WriteLine (ex.Message);
            }

            bool lv = Utils.IsValidZ (left);
            bool rv = Utils.IsValidZ (right);

            if (lv == false && rv == false)
                throw new Exception ("Both false in FindTransition");

            if (lv == true && rv == true)
                throw new Exception ("Both true in FindTransition");

            return lv ? left : right;
        }
        /*************/


    }
}
