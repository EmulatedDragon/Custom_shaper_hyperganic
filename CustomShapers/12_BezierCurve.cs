// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Hy;

namespace Hyperganic
{
    public class BezierCurve : IShaper
    {
        /// <summary>
        ///     <para> The Custom Curve Custom shaper creates a curved surface which intersects the 3 input vector points </para>
        ///     <para> Inputs: vector vecOne, vector vecTwo, vector vecThree </para>
        ///     <para> Ouput: Curved lattice structure </para>
        /// </summary>

        public override string strDescription()
        {
            return "Creates and returns a curvature";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatVector3Parameter("vecOne", "first control point"));
            AddParameter(new Hy.FloatVector3Parameter("vecTwo", "second control point"));
            AddParameter(new Hy.FloatVector3Parameter("vecThree", "third control point"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        ///     <para> 3 points are generated from the input. The rest of the points are added using BezierCurve algorithms and LinearInterPolation to form a smooth spline</para>
        ///     <para> The GetBezierCurveVector function generates the interpolated points by calling on the LinearInterPolation function which interpolates between 2 points based on a given ratio</para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;

            List<Vector3> vecList = new List<Vector3>();
            
            Vector3 vecOne = Hy.ShaperInterface.vecGetVectorArgument("vecOne", ref bOk);
            Vector3 vecTwo = Hy.ShaperInterface.vecGetVectorArgument("vecTwo", ref bOk);
            Vector3 vecThree = Hy.ShaperInterface.vecGetVectorArgument("vecThree", ref bOk);

            vecList.Add(vecOne);
            vecList.Add(vecTwo);
            vecList.Add(vecThree);

            Lattice oBezierCurve = new Lattice();
            List<Vector3> vecListCurve = new List<Vector3>();
            for (float fTRatio = 0; fTRatio < 1; fTRatio += 0.01f)
            {
                Vector3 vecCurve = GetBezierCurveVector(vecList, fTRatio);
                vecListCurve.Add(vecCurve);
            }

            for (int i = 0; i < vecListCurve.Count - 1; i++)
            {
                oBezierCurve.AddBeam(vecListCurve[i], 0.5f, vecListCurve[i + 1], 0.5f);
            }


            Lattice oLattice = new Lattice();
            for (int i = 0; i < vecList.Count; i++)
            {
                oLattice.AddBeam(vecList[i], 1f, vecList[i], 1f);
            }
            ShaperInterface.Return(0, oBezierCurve);
            ShaperInterface.Return(1, oLattice);
            return true;
        }

        public static Vector3 GetBezierCurveVector(List<Vector3> vecPoints, float fTRatio)
        {
            if (vecPoints.Count > 1)
            {
                List<Vector3> aInterpolatedListPoints = new List<Vector3>();
                for (int i = 0; i < vecPoints.Count() - 1; i++)
                {
                    aInterpolatedListPoints.Add(LinearInterPolation(vecPoints[i], vecPoints[i + 1], fTRatio));
                }
                if (aInterpolatedListPoints.Count == 1)
                {
                    return aInterpolatedListPoints[0];
                }
                else
                {
                    return GetBezierCurveVector(aInterpolatedListPoints, fTRatio);
                }
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }
        public static Vector3 LinearInterPolation(Vector3 vec1, Vector3 vec2, float fTRatio)
        {
            float fX = (1 - fTRatio) * vec1.X + fTRatio * vec2.X;
            float fY = (1 - fTRatio) * vec1.Y + fTRatio * vec2.Y;
            float fZ = (1 - fTRatio) * vec1.Z + fTRatio * vec2.Z;
            return new Vector3(fX, fY, fZ);
        }
    }
}

       
