namespace Hy
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    public static class Segment
    {
        /// <summary>
        /// Function to interpolate points within a list of control points
        /// </summary>
		public static List<Vector3> aSegmentPoints(List<Vector3> listControlPoints, int iSegments)
        {
            List<Vector3> interpolatedPts = new List<Vector3>();
            double step = 1 / (double)iSegments;
            for (int i = 0; i <= iSegments; i++)
            {
                float t = i * (float)step;
                Vector3 refPt = new Vector3(Single.MinValue, Single.MinValue, Single.MinValue);
                EvalPoint2(listControlPoints, t, ref refPt);

                if (refPt != new Vector3(Single.MinValue, Single.MinValue, Single.MinValue))
                    interpolatedPts.Add(refPt);
            }
            return interpolatedPts;
        }

        public static void EvalPoint2(List<Vector3> points, float t, ref Vector3 evalPt)
        {
            if (points.Count < 2)
                return;
            List<Vector3> tPoints = new List<Vector3>();
            for (int i = 1; i < points.Count; i++)
            {

                Vector3 deltaVec = points[i] - points[i - 1];
                Vector3 pt = points[i - 1] + (deltaVec) * t;
                tPoints.Add(pt);
            }
            if (tPoints.Count == 1)
            {
                evalPt = tPoints[0];
            }

            EvalPoint2(tPoints, t, ref evalPt);
        }

        /// <summary>
        /// Function to subsample a list of control points
        /// </summary>
        public static List<Vector3> aSubsample(List<Vector3> listControlPoints, int iAmountSamples)
        {
            List<Vector3> subsampledContourPoints = new List<Vector3>();
            for (int j = 0; j < listControlPoints.Count; j += iAmountSamples)
            {
                subsampledContourPoints.Add(listControlPoints[j]);
            }
            return subsampledContourPoints;
        }

        /// <summary>
        /// Derivation of a nurbed path, after inputting a raw path consisting of points (3D coordinates)
        /// </summary>
        public static List<Vector3> aNurb(List<Vector3> listControlPoints, int iterationCount)
        {
            List<Vector3> newlistControlPoints = new List<Vector3>();
            for (int j = 0; j < iterationCount; j++)
            {
                newlistControlPoints = new List<Vector3>();
                for (int i = 0; i < listControlPoints.Count - 1; i++)
                {
                    Vector3 oneQuarterPoint = 0.75f * listControlPoints[i] + 0.25f * listControlPoints[i + 1];
                    Vector3 threeQuarterPoint = 0.25f * listControlPoints[i] + 0.75f * listControlPoints[i + 1];
                    newlistControlPoints.Add(oneQuarterPoint);
                    newlistControlPoints.Add(threeQuarterPoint);
                }
                listControlPoints = newlistControlPoints;
            }

            return newlistControlPoints;
        }
    }
}