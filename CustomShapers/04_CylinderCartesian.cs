// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Numerics;

namespace Hyperganic
{
    public class CylinderCartesian : IShaper
    {
        ///<summary>
        ///     <para> The cylinderHollowCartesian custom shaper takes in a Radius, Height, Thickness and resolution of the lattice and generates a cylinder about the origin using cartesian coordinates. </para>
        ///     <para> Input: float Radius, float Height, float Thickness, float Resolution. </para>
        ///     <para> Output: Cartesian lattice structure</para>
        ///</summary>

        public override string strDescription()
        {
            return "Creates and returns a Cylinder";
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
        /// <para> The points are iterate by a factor of resolution, through -ve x to +ve x and y is calculated using x^2 + y^2 = r^2. </para>
        /// <para> During the iteration the positive top/bottom and negative top/bottom vectors are created and beams are created joining their respective top and bottom counterhalf</para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fBeamRadius = Hy.ShaperInterface.fGetFloatArgument("fThickness", ref bOk) / 2;
            float fRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk) - fBeamRadius;
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk) - 2 * fBeamRadius;
            float fResolution = Hy.ShaperInterface.fGetFloatArgument("fResolution", ref bOk);

            Hy.Lattice oStructure = new Hy.Lattice();
            double fIncrement = 1 / (50 * fResolution);
            for (double fX = -fRadius; fX <= fRadius + 0.00001; fX += fIncrement)
            {
                double fY = Math.Pow((Math.Pow(fRadius, 2) - Math.Pow(fX, 2)), 0.5f);
                if (double.IsNaN(fY))
                {
                    fY = 0;
                }

                Vector3 vecPositiveTop = new Vector3((float)fX, (float)fY, fHeight);
                Vector3 vecNegativeTop = new Vector3((float)fX, -(float)fY, fHeight);
                Vector3 vecPositiveBot = new Vector3((float)fX, (float)fY, 0);
                Vector3 vecNegativeBot = new Vector3((float)fX, -(float)fY, 0);

                oStructure.AddBeam(vecPositiveTop, fBeamRadius, vecPositiveBot, fBeamRadius);
                oStructure.AddBeam(vecNegativeTop, fBeamRadius, vecNegativeBot, fBeamRadius);
			}

			Hy.ShaperInterface.Return(0, oStructure);
            return true;
        }

    }
}

       
