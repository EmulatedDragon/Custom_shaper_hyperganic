// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The pyramid custom shaper creates a pyramid with a base length and a given height. </para>
    ///     <para> Input: float fLength, float fHeight. Output: Pyramid lattice structure.</para>
    /// </summary>
    public class PyramidShaper : IShaper
    {

        public override string strDescription()
        {
            return "Creates and returns a pyramid lattice";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fLength", "length of the square base"));
            AddParameter(new Hy.FloatEditBoxParameter("fHeight", "height of the pyramid"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        /// <para> The vector coordinate for the tip of the pyramid is first calculated. A nested for loop is then created to iterate through every single point 
        /// bounded by x-y plane with the given base length. For each point, a beam at that point is added and a point from the tip to it is also added. 
        /// Eventually, a pyramid is created.</para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fLength = Hy.ShaperInterface.fGetFloatArgument("fLength", ref bOk);
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk);

            Hy.Lattice oLattice = new Hy.Lattice();

            float fXCenter = 0;
            float fYCenter = 0;
            float fZCenter = fHeight;
            float fBeamRadius = 0.5f;
            Vector3 vecTip = new Vector3(fXCenter, fYCenter, fZCenter - fBeamRadius);

            for (float fX = -fLength / 2 + fBeamRadius; fX <= fLength / 2 - fBeamRadius; fX += 0.1f)
            {
				for (float fY = -fLength / 2 + fBeamRadius; fY <= fLength / 2 - fBeamRadius; fY += 0.1f)
                {
                    Vector3 newVec = new Vector3(fX, fY, 0 + fBeamRadius);
                    oLattice.AddBeam(vecTip, fBeamRadius, newVec, fBeamRadius);
                }
            }

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

    }
}


