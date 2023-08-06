// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Numerics;

namespace Hyperganic
{
    public class CylinderPolar : IShaper
    {
        /// <summary>
        ///     <para> The CylinderHollowPolar custom shaper creates a cylinder using polar coordinates. </para>
        ///     <para> Input: float radius, float height, float thickness, float resolution. </para>
        ///     <para> Output: Cylinder (polar) lattice structure </para>
        /// </summary>
        
        public override string strDescription()
        {
            return "Creates and returns a Cylinder using polar coordinates";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fRadius", "radius of of cylinder"));
            AddParameter(new Hy.FloatEditBoxParameter("fHeight", "height of cylinder"));
            AddParameter(new Hy.FloatEditBoxParameter("fThickness", "thickness of cylinder"));
            AddParameter(new Hy.FloatEditBoxParameter("fResolution", "resolution affecting the iteration increment"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        ///     <para> The cylinder is created using an angular iterative loop with increments of resolution and the x coordinate is calculated using radius * cos(theta) </para>
        ///     <para> Likewise, the y coordinate is calulated using radius * cos(theta). The beams are then added with respect to themselves</para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fBeamThickness = Hy.ShaperInterface.fGetFloatArgument("fThickness", ref bOk) / 2;
            float fRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk) - fBeamThickness;
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk) - 2 * fBeamThickness;
            float fResolution = Hy.ShaperInterface.fGetFloatArgument("fResolution", ref bOk);

            Hy.Lattice oStructure = new Hy.Lattice();

            for (float theta = 0; theta < (float)Math.PI * 2; theta += 1/(10 * fResolution))
            {
                float fY = fRadius * (float)Math.Sin(theta);
                float fX = fRadius * (float)Math.Cos(theta);

                Vector3 vecNowTop = new Vector3(fX, fY, fHeight);
                Vector3 vecNowBot = new Vector3(fX, fY, 0);
                oStructure.AddBeam(vecNowTop, fBeamThickness, vecNowBot, fBeamThickness);
            }

            Hy.ShaperInterface.Return(0, oStructure);
            return true;
        }
    }
}

       
