namespace Hy
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    public static partial class Operations
    {
        /// <summary>
        /// <para>Derivation of an orthogonal vector to a given vector (3D coordinate)</para>
        /// <para>Vectors are implemented as 3D coordinates</para>
        /// <para>Inputting just a 3D coordinate (A) returns an orthogonal vector (B) that is orthogonal to the connection (0-A)</para>
        /// </summary>
		public static Vector3 vecFindOrthogonalVector(Vector3 vecPoint)
        {
            float fX = (float)Math.CopySign(vecPoint.Z, vecPoint.X);
            float fY = (float)Math.CopySign(vecPoint.Z, vecPoint.Y);
            float fZ = (float)(-Math.CopySign(vecPoint.X, vecPoint.Z) - Math.CopySign(vecPoint.Y, vecPoint.Z));
            Vector3 orthogonalVector = new Vector3(fX, fY, fZ);
            return orthogonalVector;
        }

        /// <summary>
        /// Derivation of the mean distance between points (in a list)
        /// </summary>
        public static float fGetMidDistancePoints(List<Vector3> vecPoints)
        {
            List<Vector3> points = vecPoints;
            float midDistance = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 vec1 = points[i];
                Vector3 vec2 = points[i + 1];
                midDistance += Vector3.Distance(vec1, vec2);
            }
            midDistance = midDistance / points.Count;
            return midDistance;
        }
        /// <summary>
        /// <para>Outputting the length of a List of Vectors</para>
        /// </summary>
        public static float fGetPathLength(List<Vector3> aLeveledContourPoints)
        {
            List<float> lengths = new List<float>();
            float length = 0;
            lengths.Add(length);
            for (int j = 1; j < aLeveledContourPoints.Count; j++)
            {
                Vector3 pt1 = aLeveledContourPoints[j - 1];
                Vector3 pt2 = aLeveledContourPoints[j];
                float dist = (pt2 - pt1).Length();
                length += dist;
                lengths.Add(length);
            }
            return length;
        }
    }
}