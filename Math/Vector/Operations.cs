namespace Hy
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    public static partial class Operations
    {
        /// <summary>
        /// Function to offset a point (3D coordinate) in the XY plane by an offset value fOffset
        /// </summary>
		public static Vector3 vecOffsetPointPolarXY(Vector3 vecPoint, float fAngle, float fOffset)
        {
            float currRadius = (float)(Math.Sqrt(vecPoint.X * vecPoint.X + vecPoint.Y * vecPoint.Y));
            float xMod = (float)Math.Cos(fAngle) * (currRadius + fOffset);
            float yMod = (float)Math.Sin(fAngle) * (currRadius + fOffset);
            float z = vecPoint.Z;
            Vector3 vecMod = new Vector3(xMod, yMod, z);
            return vecMod;
        }
        /// <summary>
        /// Function to sample points (3D coordinates) on a density object given angle and z height
        /// </summary>
        public static Vector3 vecSpineOverZ(Hy.DensityField oDensity, float fAngle, float fStepZ, float fDeltaAngle, float fOffset)
        {
            fAngle = fAngle - (fDeltaAngle * fStepZ);
            float fCylinderX = 100 * (float)Math.Cos(fAngle);
            float fCylinderY = 100 * (float)Math.Sin(fAngle);
            float fCylinderZ = fStepZ;
            Vector3 CylinderPoint = new Vector3(fCylinderX, fCylinderY, fCylinderZ);
            SurfacePoint sp = oDensity.oNearestPointOnSurface(CylinderPoint);
            Vector3 vecMod = sp.vecPreciseLocation();

            vecMod = vecOffsetPointPolarXY(vecMod, fAngle, fOffset);

            return vecMod;
        }
        /// <summary>
        /// <para>Function to sample points (3D coordinates) on a density object given angle and z height</para>
        /// <para>Listof points gets interpolated in iSeg Segments</para>
        /// </summary>
        public static List<Vector3> vecSamplePointsAtangleAndHeight(Hy.DensityField oDensity, float fAngle, float fDeltaZ, float fDeltaAngle, float fOffset, int iSeg)
        {
            List<Vector3> Points = new List<Vector3>();
            int iCount = 0;
            for (float z = oDensity.oUnitBoundingBox().vecMin.Z; z < oDensity.oUnitBoundingBox().vecMax.Z; z += fDeltaZ)
            {
                Vector3 vecCurr = vecSpineOverZ(oDensity, fAngle, z, fDeltaAngle, fOffset);
                Points.Add(vecCurr);
                iCount++;
            }
            Points = Hy.Segment.aSegmentPoints(Points, iSeg);
            return Points;
        }
        /// <summary>
        /// <para>Function to snap a list of points (3D coordinates) onto a density object</para>
        /// </summary>
        public static List<Vector3> vecSnappToScan(Hy.DensityField oDensity, List<Vector3> aReferencePoints)
        {
            List<Vector3> projPoints = new List<Vector3>();
            foreach (Vector3 point in aReferencePoints)
            {
                Hy.SurfacePoint projectedPoint = oDensity.oNearestPointOnSurface(point);
                Vector3 projPoint = projectedPoint.vecPreciseLocation();
                projPoints.Add(projPoint);
            }
            return projPoints;
        }

        /// <summary>
        /// <para>Moves the first point towards from the other along the slope</para>
        /// </summary>
        public static Vector3 vecMovePointTowards(Vector3 vecPoint1, Vector3 vecPoint2, float fDistance)
        {
            Vector3 vector = new Vector3(vecPoint2.X - vecPoint1.X, vecPoint2.Y - vecPoint1.Y, vecPoint2.Z - vecPoint1.Z);
            float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            Vector3 unitVector = new Vector3(vector.X / length, vector.Y / length, vector.Z / length);
            return new Vector3(vecPoint1.X + unitVector.X * fDistance, vecPoint1.Y + unitVector.Y * fDistance, vecPoint1.Z + unitVector.Z * fDistance);
        }
        /// <summary>
        /// <para>Moves the first point away from the other along the slope</para>
        /// </summary>
        public static Vector3 vecMovePointAway(Vector3 vecPoint1, Vector3 vecPoint2, float fDistance)
        {
            Vector3 vector = new Vector3(vecPoint2.X - vecPoint1.X, vecPoint2.Y - vecPoint1.Y, vecPoint2.Z - vecPoint1.Z);
            float length = MathF.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            Vector3 unitVector = new Vector3(vector.X / length, vector.Y / length, vector.Z / length);
            return new Vector3(vecPoint1.X - unitVector.X * fDistance, vecPoint1.Y - unitVector.Y * fDistance, vecPoint1.Z - unitVector.Z * fDistance);
        }

        /// <summary>
        /// Rotate a list of points around a defined axis and angle
        /// </summary>
        public static List<Vector3> vecGetPointsRotatedAroundZ(List<Vector3> aPoints, Vector3 vecPoint1Axis, float fDeltaPhi)
        {
            List<Vector3> rotatedPoints = new List<Vector3>();

            for (int j = 0; j < aPoints.Count; j++)
            {
                rotatedPoints.Add(vecGetVectorRotatedAroundZ(aPoints[j], vecPoint1Axis, fDeltaPhi));
            }
            return rotatedPoints;
        }
        /// <summary>
        /// Rotate a list of points around the Z-axis about a defined angle
        /// </summary>
        public static Vector3 vecGetVectorRotatedAroundZ(Vector3 vecPoint, Vector3 vecPoint1Axis, float fDeltaPhi)
        {
            float dx = vecPoint.X - vecPoint1Axis.X;
            float dy = vecPoint.Y - vecPoint1Axis.Y;
            float radius = (float)(Math.Sqrt(dx * dx + dy * dy));
            float phi = (float)(Math.Atan2(dy, dx)) + fDeltaPhi;
            float x = (float)(vecPoint1Axis.X + radius * Math.Cos(phi));
            float y = (float)(vecPoint1Axis.Y + radius * Math.Sin(phi));
            float z = vecPoint.Z;
            Vector3 vecRotatedPoint = new Vector3(x, y, z);
            return vecRotatedPoint;
        }
    }
}