// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The Sphere custom shaper creates a sphere with Center being the input vector coordinate and radius being the input radius.</para>
    ///     <para> Input: float fRadius, float fXCoord, float fYCoord, float fZCoord. Output: Sphere lattice structure.</para>
    /// </summary>
    public class SphereShaper : IShaper
    {

        public override string strDescription()
        {
            return "Creates and returns a sphere lattice";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fSphereRadius", "radius of the sphere"));
            AddParameter(new Hy.FloatEditBoxParameter("fXPos", "X position of the sphere"));
            AddParameter(new Hy.FloatEditBoxParameter("fYPos", "Y position of the sphere"));
            AddParameter(new Hy.FloatEditBoxParameter("fZPos", "Z position of the sphere"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        /// <para> A vertor will be created using the x, y and z coordinates from the input. Subsequently a beam will be created from this point to the same
        /// point, with the radius from the input. This generates a sphere as desired. </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fSphereRadius = Hy.ShaperInterface.fGetFloatArgument("fSphereRadius", ref bOk);
            float fXPos = Hy.ShaperInterface.fGetFloatArgument("fXPos", ref bOk);
            float fYPos = Hy.ShaperInterface.fGetFloatArgument("fYPos", ref bOk);
            float fZPos = Hy.ShaperInterface.fGetFloatArgument("fZPos", ref bOk);

            Hy.Lattice oLattice = new Hy.Lattice();

            Vector3 vecPoint = new Vector3(fXPos, fYPos, fZPos);
            oLattice.AddBeam(vecPoint, fSphereRadius, vecPoint, fSphereRadius);

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

    }
}


