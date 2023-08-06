// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The SuperFormula2D shaper returns a lattice with all vertices derived by the super formula. https://en.wikipedia.org/wiki/Superformula </para>
    ///     <para> Input: float m, float n1, float n2, float n3, float fShapeRadius, float fMetaballSize. Output: super shape lattice.</para>
    /// </summary>
    public class SuperFormula2D : IShaper
    {

        public override string strDescription()
        {
            return "Creates and returns a super shape lattice";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("m", "m"));
            AddParameter(new Hy.FloatEditBoxParameter("n1", "n1"));
            AddParameter(new Hy.FloatEditBoxParameter("n2", "n2"));
            AddParameter(new Hy.FloatEditBoxParameter("n3", "n3"));
            AddParameter(new Hy.FloatEditBoxParameter("fShapeRadius", "Shape Radius"));
            AddParameter(new Hy.FloatEditBoxParameter("fMetaballSize", "Metaball Size"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
		}

        /// <summary>
        /// <para> This custom shaper uses the super formula to deform vector points along a ring. The Superforumla method encapsulated the transformation
        /// made by the super formula onto the X and Y values of the points. The Add-Beam method is then used to add spheres at each deformed location to give the final shape. </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float m = Hy.ShaperInterface.fGetFloatArgument("m", ref bOk);
            float n1 = Hy.ShaperInterface.fGetFloatArgument("n1", ref bOk);
            float n2 = Hy.ShaperInterface.fGetFloatArgument("n2", ref bOk);
            float n3 = Hy.ShaperInterface.fGetFloatArgument("n3", ref bOk);
            float fShapeRadius = Hy.ShaperInterface.fGetFloatArgument("fShapeRadius", ref bOk);
            float fMetaballSize = Hy.ShaperInterface.fGetFloatArgument("fMetaballSize", ref bOk);

            Hy.Lattice oLattice = new Hy.Lattice();

            float fFactorA = 1;
            float fFactorB = 1;

            for (float fPhi = 0; fPhi < 2 * MathF.PI; fPhi += 0.001f)
            {
                Vector3 vecPoint = Superformula(fPhi, fShapeRadius, m, n1, n2, n3, fFactorA, fFactorB);
                oLattice.AddBeam(vecPoint, fMetaballSize, vecPoint, fMetaballSize);
            }

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

        public Vector3 Superformula(float fPhi, float fRadius, float m, float n1, float n2, float n3, float a, float b)
        {
            float fDeformedR = fRadius * (float)(Math.Pow(Math.Pow((Math.Abs(Math.Cos(m * fPhi / 4) / a)), n2) + Math.Pow((Math.Abs(Math.Sin(m * fPhi / 4) / b)), n3), -1 / n1));
            float fX = (float)(fDeformedR * Math.Cos(fPhi)); ;
            float fY = (float)(fDeformedR * Math.Sin(fPhi));

            Vector3 vecAddPoint = new Vector3(fX, fY, 0);
            return vecAddPoint;
        }

    }
}


