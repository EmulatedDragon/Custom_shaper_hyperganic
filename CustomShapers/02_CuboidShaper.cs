// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The cuboid custom shaper creates a cuboid with the starting corner being the input vector coordinate and the length, width and height being the 
    ///     input length, width and height.</para>
    ///     <para> Input: float fLength, float fWidth, float fHeight, float fXCoord, float fYCoord, float fZCoord. Output: Cuboid lattice structure.</para>
    /// </summary>
    public class CuboidShaper : IShaper
    {

        public override string strDescription()
        {
            return "Creates and returns a cuboid lattice";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fLength", "length of the cuboid"));
            AddParameter(new Hy.FloatEditBoxParameter("fWidth", "width of the cuboid"));
            AddParameter(new Hy.FloatEditBoxParameter("fHeight", "height of the cuboid"));
            AddParameter(new Hy.FloatEditBoxParameter("fXStart", "starting X position of the cuboid"));
            AddParameter(new Hy.FloatEditBoxParameter("fYStart", "starting Y position of the cuboid"));
            AddParameter(new Hy.FloatEditBoxParameter("fZStart", "starting Z position of the cuboid"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        } 

        /// <summary>
        /// <para> A nested for loop is created to iterate through every single point bounded by the given cuboid length, width and height, starting from the vector
        /// coordinates from the input. For each point, a beam is added and eventually a cuboid is created.</para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fLength = Hy.ShaperInterface.fGetFloatArgument("fLength", ref bOk);
            float fWidth = Hy.ShaperInterface.fGetFloatArgument("fWidth", ref bOk);
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk);
            float fXStart = Hy.ShaperInterface.fGetFloatArgument("fXStart", ref bOk);
            float fYStart = Hy.ShaperInterface.fGetFloatArgument("fYStart", ref bOk);
            float fZStart = Hy.ShaperInterface.fGetFloatArgument("fZStart", ref bOk);

            Hy.Lattice oLattice = new Hy.Lattice();

            float fStepWidth = 0.5f;
            float fBeamRadius = 0.5f;
            
            for (float x = fXStart + fBeamRadius; x < fLength + fXStart - fBeamRadius; x += fStepWidth)
            {
                for (float y = fYStart + fBeamRadius; y < fWidth + fYStart - fBeamRadius; y += fStepWidth)
                {
                    for (float z = fZStart + fBeamRadius; z < fHeight + fZStart - fBeamRadius; z += fStepWidth)
                    {
                        Vector3 vecPoint = new Vector3(x, y, z);
                        oLattice.AddBeam(vecPoint, fBeamRadius, vecPoint, fBeamRadius);
                    }
                }
            }

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

    }
}


