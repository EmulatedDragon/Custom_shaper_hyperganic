// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The SuperFormula2D shaper returns a lattice with all vertices derived by the spherical product of 2 super formulas. https://en.wikipedia.org/wiki/Superformula </para>
    ///     <para> Input: float m1, float n11, float n21, float n31, float m2, float n12, float n22, float n32, float fShapeRadius, float fMetaballSize. Output: super shape lattice.</para>
    /// </summary>
    public class SuperFormula3D : IShaper
    {
        public override string strDescription()
        {
            return "Creates and returns a 3D super shape lattice";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("m1", "m1"));
            AddParameter(new Hy.FloatEditBoxParameter("n11", "n11"));
            AddParameter(new Hy.FloatEditBoxParameter("n21", "n21"));
            AddParameter(new Hy.FloatEditBoxParameter("n31", "n31"));
            AddParameter(new Hy.FloatEditBoxParameter("m2", "m2"));
            AddParameter(new Hy.FloatEditBoxParameter("n12", "n12"));
            AddParameter(new Hy.FloatEditBoxParameter("n22", "n22"));
            AddParameter(new Hy.FloatEditBoxParameter("n32", "n32"));
            AddParameter(new Hy.FloatEditBoxParameter("fShapeRadius", "Shape Radius"));
            AddParameter(new Hy.FloatEditBoxParameter("fAngularResolution", "Angular Resolution"));
            AddParameter(new Hy.FloatEditBoxParameter("fMetaballSize", "Metaball Size"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
		}

        /// <summary>
        /// <para> This custom shaper uses the spherical product of 2 super formula to form a 3D object.
        /// The Add-Beam method is then used to add spheres at each deformed location to give the final shape. </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float m1 = Hy.ShaperInterface.fGetFloatArgument("m1", ref bOk);
            float n11 = Hy.ShaperInterface.fGetFloatArgument("n11", ref bOk);
            float n21 = Hy.ShaperInterface.fGetFloatArgument("n21", ref bOk);
            float n31 = Hy.ShaperInterface.fGetFloatArgument("n31", ref bOk);
            float m2 = Hy.ShaperInterface.fGetFloatArgument("m2", ref bOk);
            float n12 = Hy.ShaperInterface.fGetFloatArgument("n12", ref bOk);
            float n22 = Hy.ShaperInterface.fGetFloatArgument("n22", ref bOk);
            float n32 = Hy.ShaperInterface.fGetFloatArgument("n32", ref bOk);
            float fShapeRadius = Hy.ShaperInterface.fGetFloatArgument("fShapeRadius", ref bOk);
            float fAngularResolution = Hy.ShaperInterface.fGetFloatArgument("fAngularResolution", ref bOk);
            float fMetaballSize = Hy.ShaperInterface.fGetFloatArgument("fMetaballSize", ref bOk);

            Hy.Lattice oLattice = new Hy.Lattice();

            for (float fPhi = -0.5f * MathF.PI; fPhi < 0.5f * MathF.PI; fPhi += fAngularResolution)
            {
                for (float fTheta = -0f * MathF.PI; fTheta < 2f * MathF.PI; fTheta += fAngularResolution)
                {
                    Vector3 vecPoint = vecGetSphericalProduct(fPhi, fTheta, fShapeRadius, m1, n11, n21, n31, m2, n12, n22, n32);
                    oLattice.AddBeam(vecPoint, fMetaballSize, vecPoint, fMetaballSize);
                }
            }

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

        public Vector3 vecGetSphericalProduct(float fPhi, float fTheta, float fRadius, float m1, float n11, float n21, float n31, float m2, float n12, float n22, float n32)
        {
            float a = 1;
            float b = 1;

            float fRadius2 = (float)(Math.Pow(Math.Pow((Math.Abs(Math.Cos(m2 * fPhi / 4) / a)), n22) + Math.Pow((Math.Abs(Math.Sin(m2 * fPhi / 4) / b)), n32), -1 / n12));
            float fRadius1 = (float)(Math.Pow(Math.Pow((Math.Abs(Math.Cos(m1 * fTheta / 4) / a)), n21) + Math.Pow((Math.Abs(Math.Sin(m1 * fTheta / 4) / b)), n31), -1 / n11));

            float fX = (float)(fRadius * fRadius1 * Math.Cos(fTheta) * fRadius2 * Math.Cos(fPhi));
            float fY = (float)(fRadius * fRadius1 * Math.Sin(fTheta) * fRadius2 * Math.Cos(fPhi));
            float fZ = (float)(fRadius * fRadius2 * Math.Sin(fPhi));

            Vector3 vecAddPoint = new Vector3(fX, fY, fZ);
            return vecAddPoint;
        }

    }
}


