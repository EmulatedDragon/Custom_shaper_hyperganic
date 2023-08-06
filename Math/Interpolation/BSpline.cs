namespace Hy
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    public class BSpline
    {
        public List<Vector3> aInputPoints;
        PathLengthCounter oLengthCounter;
        public int iDegree;
        public double[] dKnotV;
        int iNumberOfPointsAfterInterpolation;

        public BSpline(List<Vector3> aPoints, double[] kV, int n, int nPointsAfterInterpolation)
        {
            dKnotV = kV;
            aInputPoints = aPoints;
            iDegree = n;
            iNumberOfPointsAfterInterpolation = nPointsAfterInterpolation;
            oLengthCounter = new PathLengthCounter();
        }
        public BSpline(List<Vector3> aPoints, int n, int nPointsAfterInterpolation)
        {
            dKnotV = dConstructPinneddKnotVector(n, aPoints.Count);
            aInputPoints = aPoints;
            iDegree = n;
            iNumberOfPointsAfterInterpolation = nPointsAfterInterpolation;
            oLengthCounter = new PathLengthCounter();
        }


        public List<Vector3> aInterpolate()
        {
            List<Vector3> interpolated = new List<Vector3>();
            if (dKnotV.Length != aInputPoints.Count + iDegree + 1) throw new InvalidCastException("Wrong input: knot vector should have following length: iDegree + NumberOfPoints + 1 = " + (aInputPoints.Count + iDegree + 1));

            float minStep = (float)dKnotV[0];
            float maxStep = (float)dKnotV[dKnotV.Length - 1];
            float stepSize = (maxStep - minStep) / (iNumberOfPointsAfterInterpolation);

            for (float t = minStep; t <= maxStep; t += stepSize)
            {
                Vector3 newP = new Vector3(0, 0, 0);
                for (int i = 0; i < aInputPoints.Count; i++)
                {
                    double d = dCalcBSplineBaseFunction(i, iDegree, t, dKnotV);
                    Vector3 sp = (aInputPoints[i] * (float)d);
                    newP = newP + sp;
                }
                interpolated.Add(newP);
                oLengthCounter.AddNode(newP);
            }
            return interpolated;
        }

        public double dCalcBSplineBaseFunction(int i, int j, double t, double[] dKnotV)
        {
            if (j == 0)
            {
                if (t >= dKnotV[i] && t <= dKnotV[i + 1]) return 1;
                return 0;
            }
            double left = 0;
            if (dKnotV[i + j] - dKnotV[i] != 0) left = ((t - dKnotV[i]) / (dKnotV[i + j] - dKnotV[i])) * dCalcBSplineBaseFunction(i, j - 1, t, dKnotV);
            double right = 0;
            if (dKnotV[i + j + 1] - dKnotV[i + 1] != 0) right = ((dKnotV[i + j + 1] - t) / (dKnotV[i + j + 1] - dKnotV[i + 1])) * dCalcBSplineBaseFunction(i + 1, j - 1, t, dKnotV);

            return left + right;
        }

        public static double[] dConstructPinneddKnotVector(int d, int pNumber) // constructs a knot vector for a spline that includes beginning and end points 
        {
            double[] knot = new double[pNumber + d + 1];
            int l = pNumber + d + 1;
            int k = d + 1;
            int it = 1;
            for (int i = 0; i < l; i++)
            {
                if (i < k)
                {
                    knot[i] = 0;
                    continue;
                }
                if (i >= l - k)
                {
                    knot[i] = pNumber - 1;
                    continue;
                }
                knot[i] = it;
                it++;
            }
            return knot;
        }
    }
}