// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Collections.Generic;
using System.Numerics;
using Hy;

namespace Hyperganic
{
    public class SmoothInterpolationShaper : IShaper
    {
        /// <summary>
        ///     <para> The Nurb custom shaper creates 3 initial fixed points and interpolated points are created based on the number of iterations. </para>
        ///     <para> Input: int Count, float Ratio </para>
        ///     <para> Output: Curved lattice structure </para>
        /// </summary>
        /// <returns></returns>

        public override string strDescription()
        {
            return "Creates and returns a curved lattice that has more points the heigher the iteration count";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.IntEditBoxParameter("iCount", "number of times interpolation is conducted"));
            AddParameter(new Hy.FloatEditBoxParameter("fRatio", "ratio used in the interpolation method"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        ///     <para> Creates 3 fixed vectors and interpolates them iteratively. </para>
        ///     <para> The curvature can be controlled using the Ratio and the resolution can be controlled using the number of points. </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            int iCount = (int)Hy.ShaperInterface.fGetFloatArgument("iCount", ref bOk);
            float fRatio = Hy.ShaperInterface.fGetFloatArgument("fRatio", ref bOk);

            Hy.Lattice oOriginal = new Lattice();
            Hy.Lattice oSmoothen = new Lattice();
            List<Vector3> vecList = new List<Vector3>()
            {
                // adding control points
                new Vector3(0, 40, 40),
                new Vector3(40, 0, 40),
                new Vector3(-40, -40, 0),
            };
            
            // original control points
            for (int i = 0; i < vecList.Count; i++)
            {
                oOriginal.AddBeam(vecList[i], 1f, vecList[i], 1f);
            }

            //Smoothen
            vecList = SmoothenSamples(vecList, iCount, fRatio);
            for (int i = 0; i < vecList.Count; i++)
            {
                oSmoothen.AddBeam(vecList[i], 1f, vecList[i], 1f);
            }

            ShaperInterface.Return(0, oSmoothen);
            ShaperInterface.Return(1, oOriginal);

            return true;
        }
        private List<Vector3> SmoothenSamples(List<Vector3> vecList, int iIterationCount, float fRatio)
        {
            List<Vector3> aNewControlPoints = new List<Vector3>();

            for (int j = 0; j < iIterationCount; j++)
            {
                aNewControlPoints = new List<Vector3>();
                for (int i = 0; i < vecList.Count - 1; i++)
                {
                    Vector3 vecPoint1 = fRatio * vecList[i] + (1 - fRatio) * vecList[i + 1];
                    Vector3 vecPoint2 = (1 - fRatio) * vecList[i] + (fRatio) * vecList[i + 1];
                    aNewControlPoints.Add(vecPoint1);
                    aNewControlPoints.Add(vecPoint2);
                }
                vecList = aNewControlPoints;
            }

            return aNewControlPoints;
        }
    }
}

       
