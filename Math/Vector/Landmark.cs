namespace Hy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    public static partial class Operations
    {
        /// <summary>
        /// Function to derive the nearest point (as a vector3) on the surface of a density object
        /// </summary>
        public static Vector3 vecNearestPointOnSurface(Hy.DensityField oDensity, Vector3 vecStartPoint)
        {
            Vector3 finalPoint = new Vector3();
            SurfacePoint sp = oDensity.oNearestPointOnSurface(vecStartPoint);
            if (sp.bHasPreciseLocation() == true)
            {
                finalPoint = sp.vecPreciseLocation();
            }
            return finalPoint;
        }
        /// <summary>
        /// Function to derive the nearest point (as a vector3) on the surface of a density object
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecNearestPointOnSurfaceAndNormal(Hy.DensityField oDensity, Vector3 vecStartPoint)
        {
            Vector3 finalPoint = new Vector3();
            Vector3 vecNormal = new Vector3();
            SurfacePoint sp = oDensity.oNearestPointOnSurface(vecStartPoint);
            if (sp.bHasPreciseLocation() == true)
            {
                finalPoint = sp.vecPreciseLocation();
                vecNormal = sp.vecGradientNormal();
            }
            return (finalPoint, vecNormal);
        }

        /// <summary>
        /// <para>Function to derive a point on the surface of a density object using raycasting</para>
        /// <para>Raycasting means following a beam (ray, vector) until it hits the surfce of the density</para>
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecRaycastPointAndNormal(DensityField oDensity, Vector3 vecStartPoint, Vector3 vecDirection)
        {
            Vector3 finalPoint = new Vector3();
            Vector3 vecNormal = new Vector3();
            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecStartPoint, vecDirection);
            if (sp.bHasPreciseLocation() == true)
            {
                finalPoint = sp.vecPreciseLocation();
                vecNormal = sp.vecGradientNormal();
            }

            return (finalPoint, vecNormal);
        }
        /// <summary>
        /// <para>Function to derive a point on the surface of a density object using raycasting</para>
        /// <para>Raycasting means following a beam (ray, vector) until it hits the surfce of the density</para>
        /// </summary>
        public static Vector3 vecRaycastPoint(DensityField oDensity, Vector3 vecStartPoint, Vector3 vecDirection)
        {
            Vector3 finalPoint = new Vector3();
            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecStartPoint, vecDirection);
            if (sp.bHasPreciseLocation() == true)
            {
                finalPoint = sp.vecPreciseLocation();
            }

            return finalPoint;
        }


        public static (Vector3, Vector3) vecRaycastCentroidAndNormal(DensityField oDensity, List<Vector3> vecPoints, string strDirection)
        {
            Vector3 finalPoint = new Vector3();
            Vector3 vecNormal = new Vector3();
            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecCentroid = vecFindCentroid(vecPoints);
            (Vector3 vecDir, Vector3 vecStart) = vecGetProjectionVec(strDirection, vecCentroid, oBBox);

            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecStart, vecDir);
            if (sp.bHasPreciseLocation() == true)
            {
                finalPoint = sp.vecPreciseLocation();
                vecNormal = sp.vecGradientNormal();
            }

            return (finalPoint, vecNormal);
        }

        public static (Vector3, Vector3) vecGetProjectionVec(string strDir, Vector3 vecPoint, FloatBox bBox)
        {
            Vector3 vecDir = new Vector3();
            if (strDir == "X+" || strDir == "X")
            {
                vecDir = new Vector3(1, 0, 0);
                vecPoint = new Vector3(bBox.vecMin.X, vecPoint.Y, vecPoint.Z);
            }
            else if (strDir == "X-")
            {
                vecDir = new Vector3(-1, 0, 0);
                vecPoint = new Vector3(bBox.vecMax.X, vecPoint.Y, vecPoint.Z);
            }

            else if (strDir == "Y+" || strDir == "Y")
            {
                vecDir = new Vector3(0, 1, 0);
                vecPoint = new Vector3(vecPoint.X, bBox.vecMin.Y, vecPoint.Z);
            }
            else if (strDir == "Y-")
            {
                vecDir = new Vector3(0, -1, 0);
                vecPoint = new Vector3(vecPoint.X, bBox.vecMax.Y, vecPoint.Z);
            }

            else if (strDir == "Z+" || strDir == "Z")
            {
                vecDir = new Vector3(0, 0, 1);
                vecPoint = new Vector3(vecPoint.X, vecPoint.Y, bBox.vecMin.Z);
            }
            else if (strDir == "Z-")
            {
                vecDir = new Vector3(0, 0, -1);
                vecPoint = new Vector3(vecPoint.X, vecPoint.Y, bBox.vecMax.Z);
            }

            return (vecDir, vecPoint);


        }

        /// <summary>
        /// <para>Raycasts point based on bounding box percentages</para>
        /// </summary>
        public static Vector3 vecProjectPoint(DensityField oDensity, string strBBoxDir1, float fPercentageDir1, string strBBoxDir2, float fPercentageDir2, string strBBoxDir3, float fPercentageDir3, string strProjDir3)
        {
            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecBBoxDir1 = vecDirectionOf(strBBoxDir1);
            Vector3 vecBBoxDir2 = vecDirectionOf(strBBoxDir2);
            Vector3 vecBBoxDir3 = vecDirectionOf(strBBoxDir3);
            Vector3 vecProjDir3 = vecDirectionOf(strProjDir3);

            Vector3 vecBBoxvecPoint1 = fPercentageDir1 * new Vector3(vecBBoxDir1.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir1.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir1.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecBBoxvecPoint2 = fPercentageDir2 * new Vector3(vecBBoxDir2.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir2.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir2.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecBBoxPoint3 = fPercentageDir3 * new Vector3(vecBBoxDir3.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir3.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir3.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));

            Vector3 vecBBoxPoint4 = oBBox.vecMin + vecBBoxvecPoint1 + vecBBoxvecPoint2 + vecBBoxPoint3;

            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecBBoxPoint4, vecProjDir3);
            Vector3 vecFinalPoint = sp.vecPreciseLocation();

            return vecFinalPoint;
        }
        /// <summary>
        /// <para>Raycasts point based on bounding box percentages</para>
        /// <para>Raycasts point based on bounding box percentages</para>
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecProjectPointAndNormal(DensityField oDensity, string strBBoxDir1, float fPercentageDir1, string strBBoxDir2, float fPercentageDir2, string strBBoxDir3, float fPercentageDir3, string strProjDir3)
        {
            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecNormal = new Vector3();
            Vector3 vecBBoxDir1 = vecDirectionOf(strBBoxDir1);
            Vector3 vecBBoxDir2 = vecDirectionOf(strBBoxDir2);
            Vector3 vecBBoxDir3 = vecDirectionOf(strBBoxDir3);
            Vector3 vecProjDir3 = vecDirectionOf(strProjDir3);

            Vector3 vecBBoxvecPoint1 = fPercentageDir1 * new Vector3(vecBBoxDir1.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir1.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir1.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecBBoxvecPoint2 = fPercentageDir2 * new Vector3(vecBBoxDir2.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir2.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir2.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecBBoxPoint3 = fPercentageDir3 * new Vector3(vecBBoxDir3.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir3.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir3.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));

            Vector3 vecBBoxPoint4 = oBBox.vecMin + vecBBoxvecPoint1 + vecBBoxvecPoint2 + vecBBoxPoint3;

            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecBBoxPoint4, vecProjDir3);
            Vector3 vecFinalPoint = sp.vecPreciseLocation();
            vecNormal = sp.vecGradientNormal();

            return (vecFinalPoint, vecNormal);
        }

        /// <summary>
        /// <para>Finds nearest point on surface of a specified point on the connection line between two input points</para>
        /// </summary>
        public static Vector3 vecClosestConnectionLine(DensityField oDensity, Vector3 vecPoint1, Vector3 vecPoint2, float fPercentageDir1)
        {
            Vector3 vecLinkPoint = vecPoint1 + fPercentageDir1 * (vecPoint2 - vecPoint1);

            SurfacePoint sp = oDensity.oNearestPointOnSurface(vecLinkPoint);
            Vector3 vecFinalPoint = sp.vecPreciseLocation();

            return vecFinalPoint;
        }

        /// <summary>
        /// <para>Finds nearest point on surface of a specified point on the connection line between two input points</para>
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecClosestConnectionLineAndNormal(DensityField oDensity, Vector3 vecPoint1, Vector3 vecPoint2, float fPercentageDir1)
        {
            Vector3 vecLinkPoint = vecPoint1 + fPercentageDir1 * (vecPoint2 - vecPoint1);
            Vector3 vecNormal = new Vector3();
            SurfacePoint sp = oDensity.oNearestPointOnSurface(vecLinkPoint);
            Vector3 vecFinalPoint = sp.vecPreciseLocation();
            vecNormal = sp.vecGradientNormal();
            return (vecFinalPoint, vecNormal);
        }

        /// <summary>
        /// <para>Returns raycasted point on surface of a specified point along the connection lne between two input points</para>
        /// <para>Orthogonal Projection side needs to be specified</para>
        /// </summary>
        public static Vector3 vecClosestRayCastConnectionLine(DensityField oDensity, Vector3 vecPoint1, Vector3 vecPoint2, float fPercentageDir1, string vecDirection)
        {
            FloatBox oBox = oDensity.oUnitBoundingBox();
            float xMin = oBox.vecMin.X;
            float xMax = oBox.vecMax.X;
            float zMin = oBox.vecMin.Z;
            float zMax = oBox.vecMax.Z;
            float yMin = oBox.vecMin.Y;
            float yMax = oBox.vecMax.Y;

            Vector3 vecLinkPoint = vecPoint1 + fPercentageDir1 * (vecPoint2 - vecPoint1);
            Vector3 vecRayDirection = new Vector3();
            switch (vecDirection)
            {
                case "Z+":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, vecLinkPoint.Y, zMin - 10);
                    vecRayDirection = new Vector3(0, 0, 1);
                    break;
                case "Z-":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, vecLinkPoint.Y, zMax + 10);
                    vecRayDirection = new Vector3(0, 0, -1);
                    break;
                case "X+":
                    vecLinkPoint = new Vector3(xMin - 10, vecLinkPoint.Y, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(1, 0, 0);
                    break;
                case "X-":
                    vecLinkPoint = new Vector3(xMax + 10, vecLinkPoint.Y, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(-1, 0, 0);
                    break;
                case "Y+":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, yMin - 10, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(0, 1, 0);
                    break;
                case "Y-":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, yMin + 10, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(0, -1, 0);
                    break;

                default:
                    break;
            }
            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecLinkPoint, vecRayDirection);
            Vector3 vecFinalPoint = sp.vecPreciseLocation();

            return vecFinalPoint;
        }
        /// <summary>
        /// <para>Returns raycasted point on surface of a specified point along the connection lne between two input points</para>
        /// <para>Orthogonal Projection side needs to be specified</para>
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecClosestRayCastConnectionLineAndNormal(DensityField oDensity, Vector3 vecPoint1, Vector3 vecPoint2, float fPercentageDir1, string vecDirection)
        {
            FloatBox oBox = oDensity.oUnitBoundingBox();
            Vector3 vecNormal = new Vector3();
            float xMin = oBox.vecMin.X;
            float xMax = oBox.vecMax.X;
            float zMin = oBox.vecMin.Z;
            float zMax = oBox.vecMax.Z;
            float yMin = oBox.vecMin.Y;
            float yMax = oBox.vecMax.Y;

            Vector3 vecLinkPoint = vecPoint1 + fPercentageDir1 * (vecPoint2 - vecPoint1);
            Vector3 vecRayDirection = new Vector3();
            switch (vecDirection)
            {
                case "Z+":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, vecLinkPoint.Y, zMin - 10);
                    vecRayDirection = new Vector3(0, 0, 1);
                    break;
                case "Z-":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, vecLinkPoint.Y, zMax + 10);
                    vecRayDirection = new Vector3(0, 0, -1);
                    break;
                case "X+":
                    vecLinkPoint = new Vector3(xMin - 10, vecLinkPoint.Y, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(1, 0, 0);
                    break;
                case "X-":
                    vecLinkPoint = new Vector3(xMax + 10, vecLinkPoint.Y, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(-1, 0, 0);
                    break;
                case "Y+":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, yMin - 10, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(0, 1, 0);
                    break;
                case "Y-":
                    vecLinkPoint = new Vector3(vecLinkPoint.X, yMin + 10, vecLinkPoint.Z);
                    vecRayDirection = new Vector3(0, -1, 0);
                    break;

                default:
                    break;
            }
            SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecLinkPoint, vecRayDirection);
            Vector3 vecFinalPoint = sp.vecPreciseLocation();
            vecNormal = sp.vecGradientNormal();
            return (vecFinalPoint, vecNormal);
        }

        public static Vector3 vecClosestCentroid(DensityField oDensity, Vector3 vecPoint1, Vector3 vecPoint2, Vector3 point3)
        {

            List<Vector3> triangle = new List<Vector3>();
            triangle.Add(vecPoint1);
            triangle.Add(vecPoint2);
            triangle.Add(point3);

            Vector3 centroid = vecFindCentroid(triangle);

            SurfacePoint sp = oDensity.oNearestPointOnSurface(centroid);
            Vector3 vecCentroid = sp.vecPreciseLocation();

            return vecCentroid;
        }
        public static (Vector3, Vector3) vecClosestCentroidAndNormal(DensityField oDensity, Vector3 vecPoint1, Vector3 vecPoint2, Vector3 vecPoint3)
        {

            List<Vector3> triangle = new List<Vector3>();
            triangle.Add(vecPoint1);
            triangle.Add(vecPoint2);
            triangle.Add(vecPoint3);

            Vector3 centroid = vecFindCentroid(triangle);

            SurfacePoint sp = oDensity.oNearestPointOnSurface(centroid);
            Vector3 vecCentroid = sp.vecPreciseLocation();
            Vector3 vecNormal = sp.vecGradientNormal();

            return (vecCentroid, vecNormal);
        }

        /// <summary>
        /// <para>Finds the extreme point in one of the orthogonal directions by looking at a set of points along a line and raycasting them to the surface</para>
        /// </summary>
        public static Vector3 vecProjectLine(DensityField oDensity, string strBBoxDir1, float fPercentageDir1, string strBBoxDir2, float fPercentageDir2, string strBBoxDir3, List<float> LineList, string strProjDir3)
        {
            List<Vector3> validLinePoints = new List<Vector3>();
            float fMinDistance = 999999999;
            Vector3 vecExtremePoint = new Vector3();
            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecBBoxDir1 = vecDirectionOf(strBBoxDir1);
            Vector3 vecBBoxDir2 = vecDirectionOf(strBBoxDir2);
            Vector3 vecBBoxDir3 = vecDirectionOf(strBBoxDir3);
            Vector3 vecProjDir3 = vecDirectionOf(strProjDir3);

            Vector3 vecBBoxvecPoint1 = fPercentageDir1 * new Vector3(vecBBoxDir1.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir1.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir1.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecBBoxvecPoint2 = fPercentageDir2 * new Vector3(vecBBoxDir2.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir2.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir2.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));

            float fMinPercentageDir3 = LineList[0];
            float fMaxPercentageDir3 = LineList[1];
            float fResolutionDir3 = LineList[2];

            for (float fRatio = fMinPercentageDir3; fRatio <= fMaxPercentageDir3; fRatio += fResolutionDir3)
            {
                Vector3 vecBBoxPoint3 = fRatio * new Vector3(vecBBoxDir3.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir3.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir3.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
                Vector3 vecBBoxPoint4 = oBBox.vecMin + vecBBoxvecPoint1 + vecBBoxvecPoint2 + vecBBoxPoint3;

                SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecBBoxPoint4, vecProjDir3);

                if (sp.bHasPreciseLocation() == true)
                {
                    Vector3 vecFinalPoint = sp.vecPreciseLocation();
                    validLinePoints.Add(vecFinalPoint);

                    //calculate distance
                    float fCurrentDist = (vecFinalPoint - vecBBoxPoint4).Length();

                    if (fCurrentDist < fMinDistance)
                    {
                        vecExtremePoint = vecFinalPoint;
                        fMinDistance = fCurrentDist;
                    }
                }
            }

            return vecExtremePoint;
        }
        /// <summary>
        /// <para>Finds the extreme point in one of the orthogonal directions by looking at a set of points along a line and raycasting them to the surface</para>
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecProjectLineAndNormal(DensityField oDensity, string strBBoxDir1, float fPercentageDir1, string strBBoxDir2, float fPercentageDir2, string strBBoxDir3, List<float> LineList, string strProjDir3)
        {
            List<Vector3> validLinePoints = new List<Vector3>();
            float fMinDistance = 999999999;
            Vector3 vecExtremePoint = new Vector3();
            Vector3 vecNormal = new Vector3();
            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecBBoxDir1 = vecDirectionOf(strBBoxDir1);
            Vector3 vecBBoxDir2 = vecDirectionOf(strBBoxDir2);
            Vector3 vecBBoxDir3 = vecDirectionOf(strBBoxDir3);
            Vector3 vecProjDir3 = vecDirectionOf(strProjDir3);

            Vector3 vecBBoxvecPoint1 = fPercentageDir1 * new Vector3(vecBBoxDir1.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir1.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir1.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecBBoxvecPoint2 = fPercentageDir2 * new Vector3(vecBBoxDir2.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir2.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir2.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));

            float fMinPercentageDir3 = LineList[0];
            float fMaxPercentageDir3 = LineList[1];
            float fResolutionDir3 = LineList[2];

            for (float fRatio = fMinPercentageDir3; fRatio <= fMaxPercentageDir3; fRatio += fResolutionDir3)
            {
                Vector3 vecBBoxPoint3 = fRatio * new Vector3(vecBBoxDir3.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir3.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir3.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
                Vector3 vecBBoxPoint4 = oBBox.vecMin + vecBBoxvecPoint1 + vecBBoxvecPoint2 + vecBBoxPoint3;

                SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecBBoxPoint4, vecProjDir3);

                if (sp.bHasPreciseLocation() == true)
                {
                    Vector3 vecFinalPoint = sp.vecPreciseLocation();
                    validLinePoints.Add(vecFinalPoint);

                    //calculate distance
                    float fCurrentDist = (vecFinalPoint - vecBBoxPoint4).Length();

                    if (fCurrentDist < fMinDistance)
                    {
                        vecExtremePoint = vecFinalPoint;
                        fMinDistance = fCurrentDist;
                        vecNormal = sp.vecGradientNormal();
                    }
                }
            }

            return (vecExtremePoint, vecNormal);
        }
        /// <summary>
        /// <para>Finds extreme point along an orthogonal projection based on a point distribution within a rectangle using Raycasting</para>
        /// </summary>
        public static Vector3 vecProjectGrid(DensityField oDensity, bool isRight, string strBBoxDir1, float fPercentageDir1, string strBBoxDir2, List<float> aLine2, string strBBoxDir3, List<float> aLine3, string strProjDir3)
        {
            List<Vector3> validLinePoints = new List<Vector3>();
            float fMinDistance = 999999999;
            Vector3 vecExtremePoint = new Vector3();

            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecBBoxDir1 = vecDirectionOf(strBBoxDir1);
            Vector3 vecBBoxDir2 = vecDirectionOf(strBBoxDir2);
            Vector3 vecBBoxDir3 = vecDirectionOf(strBBoxDir3);
            Vector3 vecProjDir3 = vecDirectionOf(strProjDir3);

            Vector3 vecBBoxvecPoint1 = fPercentageDir1 * new Vector3(vecBBoxDir1.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir1.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir1.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            float fMinPercentageDir2 = aLine2[0];
            float fMaxPercentageDir2 = aLine2[1];
            float fResolutionDir2 = aLine2[2];

            float fMinPercentageDir3 = aLine3[0];
            float fMaxPercentageDir3 = aLine3[1];
            float fResolutionDir3 = aLine3[2];

            for (float fRatio2 = fMinPercentageDir2; fRatio2 <= fMaxPercentageDir2; fRatio2 += fResolutionDir2)
            {
                Vector3 vecBBoxvecPoint2 = fRatio2 * new Vector3(vecBBoxDir2.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir2.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir2.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));

                for (float fRatio3 = fMinPercentageDir3; fRatio3 <= fMaxPercentageDir3; fRatio3 += fResolutionDir3)
                {
                    Vector3 vecBBoxPoint3 = fRatio3 * new Vector3(vecBBoxDir3.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir3.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir3.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
                    Vector3 vecBBoxPoint4 = oBBox.vecMin + vecBBoxvecPoint1 + vecBBoxvecPoint2 + vecBBoxPoint3;

                    SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecBBoxPoint4, vecProjDir3);

                    if (sp.bHasPreciseLocation() == true)
                    {
                        Vector3 vecFinalPoint = sp.vecPreciseLocation();
                        validLinePoints.Add(vecFinalPoint);

                        //calculate distance
                        float fCurrentDist = (vecFinalPoint - vecBBoxPoint4).Length();

                        if (fCurrentDist < fMinDistance)
                        {
                            vecExtremePoint = vecFinalPoint;
                            fMinDistance = fCurrentDist;
                        }

                    }
                }
            }



            return vecExtremePoint;

        }
        /// <summary>
        /// <para>Finds extreme point along an orthogonal projection based on a point distribution within a rectangle using Raycasting</para>
        /// <para>Returns Vector3() and the local normal of the point</para>
        /// </summary>
        public static (Vector3, Vector3) vecProjectGridAndNormal(DensityField oDensity, string strBBoxDir1, float fPercentageDir1, string strBBoxDir2, List<float> aLine2, string strBBoxDir3, List<float> aLine3, string strProjDir3)
        {
            List<Vector3> validLinePoints = new List<Vector3>();
            float fMinDistance = 999999999;
            Vector3 vecExtremePoint = new Vector3();

            FloatBox oBBox = oDensity.oUnitBoundingBox();
            Vector3 vecBBoxDir1 = vecDirectionOf(strBBoxDir1);
            Vector3 vecBBoxDir2 = vecDirectionOf(strBBoxDir2);
            Vector3 vecBBoxDir3 = vecDirectionOf(strBBoxDir3);
            Vector3 vecProjDir3 = vecDirectionOf(strProjDir3);

            Vector3 vecBBoxvecPoint1 = fPercentageDir1 * new Vector3(vecBBoxDir1.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir1.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir1.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
            Vector3 vecNormal = new Vector3();
            float fMinPercentageDir2 = aLine2[0];
            float fMaxPercentageDir2 = aLine2[1];
            float fResolutionDir2 = aLine2[2];

            float fMinPercentageDir3 = aLine3[0];
            float fMaxPercentageDir3 = aLine3[1];
            float fResolutionDir3 = aLine3[2];

            for (float fRatio2 = fMinPercentageDir2; fRatio2 <= fMaxPercentageDir2; fRatio2 += fResolutionDir2)
            {
                Vector3 vecBBoxvecPoint2 = fRatio2 * new Vector3(vecBBoxDir2.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir2.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir2.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));

                for (float fRatio3 = fMinPercentageDir3; fRatio3 <= fMaxPercentageDir3; fRatio3 += fResolutionDir3)
                {
                    Vector3 vecBBoxPoint3 = fRatio3 * new Vector3(vecBBoxDir3.X * (oBBox.vecMax.X - oBBox.vecMin.X), vecBBoxDir3.Y * (oBBox.vecMax.Y - oBBox.vecMin.Y), vecBBoxDir3.Z * (oBBox.vecMax.Z - oBBox.vecMin.Z));
                    Vector3 vecBBoxPoint4 = oBBox.vecMin + vecBBoxvecPoint1 + vecBBoxvecPoint2 + vecBBoxPoint3;

                    SurfacePoint sp = oDensity.oRaySurfaceIntersection(vecBBoxPoint4, vecProjDir3);

                    if (sp.bHasPreciseLocation() == true)
                    {
                        Vector3 vecFinalPoint = sp.vecPreciseLocation();
                        validLinePoints.Add(vecFinalPoint);

                        //calculate distance
                        float fCurrentDist = (vecFinalPoint - vecBBoxPoint4).Length();

                        if (fCurrentDist < fMinDistance)
                        {
                            vecExtremePoint = vecFinalPoint;
                            fMinDistance = fCurrentDist;
                            vecNormal = sp.vecGradientNormal();
                        }

                    }
                }
            }



            return (vecExtremePoint, vecNormal);

        }

        private static Vector3 ProgramA(Vector3 vecA, Vector3 vecB, Vector3 vecC)
        {
            Vector3 vecAvg = new Vector3();
            vecAvg = (vecA + vecB + vecC) / 3;
            return vecAvg;
        }

        private static float ProgramB(Vector3 vecA, Vector3 vecB, Vector3 vecC)
        {
            Vector3 vecOne = vecB - vecA;
            Vector3 vecTwo = vecC - vecA;
            return Vector3.Cross(vecOne, vecTwo).Length();
        }
        /// <summary>
        /// Finds the centroid of a list of vertices inputed
        /// </summary>
        public static Vector3 vecFindCentroid(List<Vector3> aInput)
        {
            int i = 0;
            int j = aInput.Count() - 2;
            float d = 0.0f;
            Vector3 E = new Vector3(0, 0, 0);
            while (i < j)
            {
                float f = ProgramB(aInput[0], aInput[i + 1], aInput[i + 2]);
                E += f * ProgramA(aInput[0], aInput[i + 1], aInput[i + 2]);
                d += f;
                i++;
            }

            return E / d;
        }
        public static Vector3 vecDirectionOf(string strDir)
        {
            Vector3 vecDir = new Vector3();
            if (strDir == "X+" || strDir == "X")
            {
                vecDir = new Vector3(1, 0, 0);
            }
            else if (strDir == "X-")
            {
                vecDir = new Vector3(-1, 0, 0);
            }

            else if (strDir == "Y+" || strDir == "Y")
            {
                vecDir = new Vector3(0, 1, 0);
            }
            else if (strDir == "Y-")
            {
                vecDir = new Vector3(0, -1, 0);
            }

            else if (strDir == "Z+" || strDir == "Z")
            {
                vecDir = new Vector3(0, 0, 1);
            }
            else if (strDir == "Z-")
            {
                vecDir = new Vector3(0, 0, -1);
            }

            return vecDir;
        }
    }
}
