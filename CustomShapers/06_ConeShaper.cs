// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The cone custom shaper creates a cone with a base radius and a height. </para>
    ///     <para> Input: float fRadius, float fHeight, float fXCenter, float fYCenter, float fZCenter. Output: Cone lattice structure.</para>
    /// </summary>
    public class ConeShaper : IShaper
    {
        public override string strDescription()
        {
            return "Creates and returns a cone lattice";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fRadius", "Radius of the circular base"));
            AddParameter(new Hy.FloatEditBoxParameter("fHeight", "Height of the cone"));
            AddParameter(new Hy.FloatEditBoxParameter("fXCenter", "X position of the center"));
            AddParameter(new Hy.FloatEditBoxParameter("fYCenter", "Y position of the center"));
            AddParameter(new Hy.FloatEditBoxParameter("fZCenter", "Z position of the center"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        /// <para> The vector coordinate of the Center of the cone at the base and the is first calculated using the vector coordinates given in the input. 
        /// A for loop is then created to iterate through from 0 to 2PI. The vectors generated in this for loop is the circle at the base of the cone. For
        /// each iteration, a beam will be added from the new point to the base Center, another beam will be added from the new point to the tip. Eventually,
        /// a cone is created.</para>
        /// </summary>
        public override bool bProduceResult()
        {
            float fBeamRadius = 0.5f;

            bool bOk = true;
            float fRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk) - fBeamRadius;
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk) - 2 * fBeamRadius;
            float fXCenter = Hy.ShaperInterface.fGetFloatArgument("fXCenter", ref bOk);
            float fYCenter = Hy.ShaperInterface.fGetFloatArgument("fYCenter", ref bOk);
            float fZCenter = Hy.ShaperInterface.fGetFloatArgument("fZCenter", ref bOk) + fBeamRadius;

            Hy.Lattice oLattice = new Hy.Lattice();

            Vector3 vecCenterBase = new Vector3(fXCenter, fYCenter, fZCenter);
            Vector3 vecTip = vecCenterBase + new Vector3(0, 0, fHeight);

            for (float theta = 0; theta < Math.PI * 2; theta += 0.05f)
            {
                float fX = fXCenter + (float)(fRadius * Math.Cos(theta));
                float fY = fYCenter + (float)(fRadius * Math.Sin(theta));
                Vector3 newVecLow = new Vector3(fX, fY, fZCenter);
                oLattice.AddBeam(vecCenterBase, fBeamRadius, newVecLow, fBeamRadius);
                oLattice.AddBeam(vecTip, fBeamRadius, newVecLow, fBeamRadius);
            }

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

    }
}


