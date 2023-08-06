// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Numerics;

namespace Hyperganic
{
    public class CustomSpring : IShaper
    {
        /// <summary>
        ///     <para> The CustomSpring custom Shaper creates a spring about the origin in the z direction. </para>
        ///     <para> Input: float radius, float height, float thickness, float resolution, integer coils. </para>
        ///     <para> Output: Spring lattice structure </para>
        /// </summary>
        
        public override string strDescription()
        {
            return "Creates and returns a spring";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fRadius", "radius of the spring"));
            AddParameter(new Hy.FloatEditBoxParameter("fHeight", "height of the spring"));
            AddParameter(new Hy.FloatEditBoxParameter("fBeamThickness", "radius of each beam added"));
            AddParameter(new Hy.FloatEditBoxParameter("fResolution", "resolution affecting the iteration increment"));
            AddParameter(new Hy.IntEditBoxParameter("iCoils", "number of coils in the spring"));
        }

        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        ///     <para> The method iterates through 2PI * no. coils times with an increment of fResolution.</para>
        ///     <para> The X coordinate is calculated by using trigonometry of radius * Sin(theta) </para>
        ///     <para> While the Y coordinate is calcuated using radius * Cos(theta) </para>
        ///     <para> The Z coordinate is calcuated using the total height * theta / fMaxTheta </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fThickness = Hy.ShaperInterface.fGetFloatArgument("fBeamThickness", ref bOk);
            float fRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk) - fThickness;
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk) - 2 * fThickness;
            float fResolution = Hy.ShaperInterface.fGetFloatArgument("fResolution", ref bOk);
            int iCoils = (int)Hy.ShaperInterface.fGetFloatArgument("iCoils", ref bOk);

            Hy.Lattice oStructure = new Hy.Lattice();
            float fMaxTheta = MathF.PI * 2 * iCoils;

            for (float theta = 0; theta <= fMaxTheta; theta += fResolution)
            {
                float fZ = fHeight * (theta / fMaxTheta);
                float fY = fRadius * (float)Math.Sin(theta);
                float fX = fRadius * (float)Math.Cos(theta);
                Vector3 vecNow = new Vector3(fX, fY, fZ);
                oStructure.AddBeam(vecNow, fThickness, vecNow, fThickness);
            }

            Hy.ShaperInterface.Return(0, oStructure);
            return true;
        }
    }
}

       
