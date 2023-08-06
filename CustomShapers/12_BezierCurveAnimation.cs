// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Hy;

namespace Hyperganic
{
    public class BezierCurveAnimation : IShaper
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
            AddParameter(new Hy.FloatEditBoxParameter("fIterator", "ratio between 0 and 1 that is used to form bezier curves"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        ///     <para> 4 points are generated from the input. The rest of the points are added using BezierCurve algorithms and LinearInterPolation to form a smooth spline</para>
        ///     <para> The GetBezierCurveVector function generates the interpolated points by calling on the LinearInterPolation function which interpolates between 2 points based on a given ratio</para>
        /// </summary>

        Lattice oBezierCurve = new Lattice();
        Lattice oMainLines = new Lattice();
        Lattice oLine1= new Lattice();
        Lattice oLine2= new Lattice();
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fIterator = Hy.ShaperInterface.fGetFloatArgument("fIterator", ref bOk);

			if (fIterator == 0)
			{
                oBezierCurve = new Lattice();
            }
            oMainLines = new Lattice();
            oLine1 = new Lattice();
            oLine2 = new Lattice();
            List<Vector3> vecList = new List<Vector3>
            {
                new Vector3(-20, 20, 0),
                new Vector3(-10, -20, 20),
                new Vector3(10, -20, 20),
                new Vector3(20, 20, 0)
            };

            Vector3 vecCurve = GetBezierCurveVector(vecList, fIterator);
            oBezierCurve.AddBeam(vecCurve, 0.5f, vecCurve, 0.5f);

            ShaperInterface.Return(0, oMainLines);
            ShaperInterface.Return(1, oLine1);
            ShaperInterface.Return(2, oLine2);
            ShaperInterface.Return(3, oBezierCurve);

            return true;
        }

        public Vector3 GetBezierCurveVector(List<Vector3> vecPoints, float fTRatio)
        {
            if (vecPoints.Count > 1)
            {

                List<Vector3> aInterpolatedListPoints = new List<Vector3>();
                for (int i = 0; i < vecPoints.Count() - 1; i++)
                {
                    if (vecPoints.Count == 4)
                    {
                        oMainLines.AddBeam(vecPoints[i], 0.1f, vecPoints[i + 1], 0.1f);
                    }
					else if (vecPoints.Count == 3)
                    {
                        oLine1.AddBeam(vecPoints[i], 0.1f, vecPoints[i + 1], 0.1f);
                    }
                    else if (vecPoints.Count == 2)
                    {
                        oLine2.AddBeam(vecPoints[i], 0.1f, vecPoints[i + 1], 0.1f);
                    }
                    aInterpolatedListPoints.Add(LinearInterPolation(vecPoints[i], vecPoints[i + 1], fTRatio));
                }
                if (aInterpolatedListPoints.Count == 1)
                {
                    oBezierCurve.AddBeam(aInterpolatedListPoints[0], 0.3f, aInterpolatedListPoints[0], 0.3f);
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

       
