// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hyperganic
{
    public class CustomEpicycloid : IShaper
    {
        /// <summary>
        ///     <para> The epicycloid custom shaper creates an epicycloid around the center of the axis. </para>
        ///     <para> The small radius is the radius of the small circle created from points on the larger center circle which has a radius of large radius. </para>
        ///     <para> Input: float large radius, float small radius </para>
        ///     <para> Output: Epicycloid lattice structure </para>
        /// 
        /// </summary>

        public override string strDescription()
        {
            return "Creates and returns a custom epiycloid";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fRadius", "radius of inner circle"));
            AddParameter(new Hy.FloatEditBoxParameter("fOuterRadius", "radius of outer circle"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        /// <para> A list of points that satisfies the Epicycloid equation will be generated when calling the Epicycloid method. </para>
        /// <para> The list of points are then iterated through and beams are created joining the initial vector and the next vector. </para>
        /// </summary>
        public override bool bProduceResult()
        {
            float fBeamRadius = 0.5f;
            bool bOk = true;
            float fRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk);
            float fOuterRadius = Hy.ShaperInterface.fGetFloatArgument("fOuterRadius", ref bOk);

            Hy.Lattice oStructure = new Hy.Lattice();

            List<Vector3> aVectors = GetEpicycloidVectorList(fRadius, fOuterRadius);
            for (int i = 0; i < aVectors.Count - 1; i++)
            {
                oStructure.AddBeam(aVectors[i], fBeamRadius, aVectors[i + 1], fBeamRadius);
            }

            Hy.ShaperInterface.Return(0, oStructure);
            return true;
        }
        public List<Vector3> GetEpicycloidVectorList(float fMainCircle, float fOuterRadius)
        {
            List<Vector3> aVectorList = new List<Vector3>();
            for (float fTheta = 0; fTheta < Math.PI * 2; fTheta += 0.1f)
            {
                float x = (fMainCircle + fOuterRadius) * MathF.Cos(fTheta) - (fOuterRadius) * MathF.Cos(fTheta * (fMainCircle + fOuterRadius) / fOuterRadius);
                float y = (fMainCircle + fOuterRadius) * MathF.Sin(fTheta) - (fOuterRadius) * MathF.Sin(fTheta * (fMainCircle + fOuterRadius) / fOuterRadius);
                Vector3 vecNewVector = new Vector3(x, y, 0);
                aVectorList.Add(vecNewVector);
            }

            return aVectorList;
        }
    }
}

       
