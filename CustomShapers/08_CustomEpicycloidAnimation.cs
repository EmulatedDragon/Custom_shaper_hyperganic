// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Collections.Generic;
using System.Numerics;
using Hy;

namespace Hyperganic
{
    public class CustomEpicycloidAnimation : IShaper
    {
        /// <summary>
        ///     <para> The epicycloid custom shaper creates an epicycloid around the center of the axis. </para>
        ///     <para> The small radius is the radius of the small circle created from points on the larger center circle which has a radius of large radius. </para>
        ///     <para> Input: float large radius, float small radius </para>
        ///     <para> Output: Epicycloid lattice structure </para>
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
            AddParameter(new Hy.FloatEditBoxParameter("fTheta", "theta value at each animation timestep"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        /// <para> A list of points that satisfies the Epicycloid equation will be generated when calling the Epicycloid method. </para>
        /// <para> The list of points are then iterated through and beams are created joining the initial vector and the next vector. </para>
        /// </summary>
        public Lattice latCircle = new Lattice();
        public Lattice latOuterCircle = new Lattice();
        public Lattice latLines = new Lattice();
        public Lattice latPoints = new Lattice();
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fTheta = Hy.ShaperInterface.fGetFloatArgument("fTheta", ref bOk);
            float fRadius = 10;
            float fOuterRadius = 5;
            if (fTheta == 0)
            {
                latCircle = new Lattice();
                latPoints = new Lattice();
                for (float fAngle = 0; fAngle < MathF.PI * 2; fAngle += 0.02f)
                {
                    Vector3 vecCircle = new Vector3(fRadius * MathF.Cos(fAngle), fRadius * MathF.Sin(fAngle), 0);
                    latCircle.AddBeam(vecCircle, 0.15f, vecCircle, 0.15f);
                }
            }
            latLines = new Lattice();
            latOuterCircle = new Lattice();

            Vector3 vecCircleTheta = new Vector3(fRadius * MathF.Cos(fTheta), fRadius * MathF.Sin(fTheta), 0);
            latLines.AddBeam(new Vector3(0), 0.2f, vecCircleTheta, 0.2f);

            Vector3 vecOuter = new Vector3((fRadius + fOuterRadius) * MathF.Cos(fTheta), (fRadius + fOuterRadius) * MathF.Sin(fTheta), 0);
            for (float fAngle = 0; fAngle < MathF.PI * 2; fAngle += 0.03f)
            {
                Vector3 vecOuterCircle = vecOuter + new Vector3(fOuterRadius * MathF.Cos(fAngle), fOuterRadius * MathF.Sin(fAngle), 0);
                latOuterCircle.AddBeam(vecOuterCircle, 0.15f, vecOuterCircle, 0.15f);
            }
            Vector3 vecPoint = GetEpicycloidVector(fRadius, fOuterRadius, fTheta);
			latLines.AddBeam(vecOuter, 0.2f, vecPoint, 0.2f);
			latPoints.AddBeam(vecPoint, 0.3f, vecPoint, 0.3f);

            Hy.ShaperInterface.Return(0, latCircle);
            Hy.ShaperInterface.Return(1, latOuterCircle);
            Hy.ShaperInterface.Return(2, latLines);
            Hy.ShaperInterface.Return(3, latPoints);
            return true;
        }
        public Vector3 GetEpicycloidVector(float fMainCircle, float fOuterRadius, float fTheta)
        {
            float fX = (fMainCircle + fOuterRadius) * MathF.Cos(fTheta) - (fOuterRadius) * MathF.Cos(fTheta * (fMainCircle + fOuterRadius) / fOuterRadius);
            float fY = (fMainCircle + fOuterRadius) * MathF.Sin(fTheta) - (fOuterRadius) * MathF.Sin(fTheta * (fMainCircle + fOuterRadius) / fOuterRadius);
            Vector3 vecNewVector = new Vector3(fX, fY, 0);
            return vecNewVector;
        }
    }
}

       
