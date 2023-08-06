// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Collections.Generic;
using System.Numerics;
using Hy;

namespace Hyperganic
{
    public class RotationExplorer : IShaper
    {
        /// <summary>
        ///     <para> Creates an oval and rotates based on the direction vector of the inputs. </para>
        ///     <para> Input: float radius, float fAngle, vector vecAxis, string RotationAxis. </para>
        ///     <para> Output: Oval shaped lattice structure </para>
        /// </summary>

        public override string strDescription()
        {
            return "Rotates an Oval of input radius by the input angle given an input vector axis";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fRadius", "radius of circle being drawn"));
            AddParameter(new Hy.FloatEditBoxParameter("fAngle", "angle to be rotated"));
            AddParameter(new Hy.FloatVector3Parameter("vecAxis", "axis when custom rotation is selected"));
            AddParameter(new Hy.StringEditBoxParameter("strRotationAxis", "axis at which circle will be rotated"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
            AddReturn(new Hy.LatticeParameter());
        }

        /// <summary>
        ///     <para> The function first calls a function "CreateCircle" which creates a circle of float Radius. </para>
        ///     <para> Depending on the type of rotation (X,Y,Z,Custom) selected, the points on the circle will rotate accordingly using matrix multiplication by calling the respective rotation functions. </para>
        ///     <para> RotateAboutXOrigin rotates a point about the x axis using matrix multiplication by fTheta. </para>
        ///     <para> RotateAboutYOrigin rotates a point about the y axis using matrix multiplication by fTheta. </para>
        ///     <para> RotateAboutZOrigin rotates a point about the z axis using matrix multiplication by fTheta. </para>
        ///     <para> GetRotatedVector rotates a given point by fTheta about a given vector axis. </para>
        ///     <para> The formulae for the GetRotatedVector can be found from the following paper http://scipp.ucsc.edu/~haber/ph216/rotation_12.pdf </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            float fRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk);
            float fAngle = Hy.ShaperInterface.fGetFloatArgument("fAngle", ref bOk) / 180 * MathF.PI;
            Vector3 vecAxis = Hy.ShaperInterface.vecGetVectorArgument("vecAxis", ref bOk);
            string strRotationAxis = Hy.ShaperInterface.strGetStringArgument("strRotationAxis", ref bOk);

            float fLatticeBeamRadius = 0.3f;
            float fAxisBeamRadius = 0.3f; 
            float fLatticeSpheresRadius = 0.4f;
            float fTrackerSpheresRadius = 0.5f;

            Lattice latLattice = new Lattice();
            Lattice latAxis = new Lattice();
            Lattice latTracker = new Lattice();

            List<Vector3> vecList = GetVectorPoints(fRadius);
            // | cos(theta)  -sin(theta) |  | x |  =  | x' |
            // | sin(theta)   cos(theta) |  | y |     | y' |
            Vector3 vecNew;
            for (int i = 0; i < vecList.Count; i++)
            {

                if (strRotationAxis == "X")
                {
                    vecNew = RotateAboutXOrigin(vecList[i], fAngle);
                }
                else if (strRotationAxis == "Y")
                {
                    vecNew = RotateAboutYOrigin(vecList[i], fAngle);
                }
                else if (strRotationAxis == "Z")
                {
                    vecNew = RotateAboutZOrigin(vecList[i], fAngle);
                }
                else
                {
                    vecNew = GetRotatedVector(vecList[i], vecAxis.vecNormalized(), fAngle);
                }
                latLattice.AddBeam(vecNew, fLatticeBeamRadius, vecNew, fLatticeBeamRadius);
                latAxis.AddBeam(vecList[i], fAxisBeamRadius, vecList[i], fAxisBeamRadius);
                if (i == vecList.Count / 4 || i == 3 * vecList.Count / 4)
                {
                    latAxis.AddBeam(vecList[i], fLatticeSpheresRadius, vecList[i], fLatticeSpheresRadius);
                    latTracker.AddBeam(vecNew, fTrackerSpheresRadius, vecNew, fTrackerSpheresRadius);
                }
            }

            // Set lattice for axis of rotation
            Vector3 vecAxisStart = fRadius * vecAxis.vecNormalized();
            if (strRotationAxis == "X")
            {
                vecAxisStart = new Vector3(fRadius, 0, 0);
            }
            else if (strRotationAxis == "Y")
            {
                vecAxisStart = new Vector3(0, fRadius, 0);
            }
            else if (strRotationAxis == "Z")
            {
                vecAxisStart = new Vector3(0, 0, fRadius);
            }
            latAxis.AddBeam(vecAxisStart, fAxisBeamRadius, -vecAxisStart, fAxisBeamRadius);
            Hy.ShaperInterface.Return(0, latLattice);
            Hy.ShaperInterface.Return(1, latAxis);
            Hy.ShaperInterface.Return(2, latTracker);
            return true;
        }

        public List<Vector3> GetVectorPoints(float fRadius)
        {
            List<Vector3> vecList = new List<Vector3>();

            for (float theta = 0; theta <= (float)Math.PI * 2; theta += MathF.PI / 50)
            {
                float fX = fRadius * MathF.Cos(theta);
                float fY = (fRadius) * MathF.Sin(theta);
                Vector3 vecNow = new Vector3(fX, fY, 0);
                vecList.Add(vecNow);
            }

            return vecList;
        }

        public Vector3 RotateAboutZOrigin(Vector3 vecPoint, float fTheta)
        {
            float fvx = vecPoint.X;
            float fvy = vecPoint.Y;
            float fvz = vecPoint.Z;

            float fCos = MathF.Cos(fTheta);
            float fSin = MathF.Sin(fTheta);

            float fX = (fvx * fCos) - (fvy * fSin);
            float fY = (fvx * fSin) + (fvy * fCos);

            Vector3 vecPointNew = new Vector3(fX, fY, fvz);
            return vecPointNew;

        }

        public Vector3 RotateAboutYOrigin(Vector3 vecPoint, float fTheta)
        {
            float fvx = vecPoint.X;
            float fvy = vecPoint.Y;
            float fvz = vecPoint.Z;

            float fCos = MathF.Cos(fTheta);
            float fSin = MathF.Sin(fTheta);

            float fX = (fvx * fCos) + (fvz * fSin);
            float fZ = (fvx * -fSin) + (fvz * fCos);

            Vector3 vecPointNew = new Vector3(fX, fvy, fZ);
            return vecPointNew;

        }

        public Vector3 RotateAboutXOrigin(Vector3 vecPoint, float fTheta)
        {
            float fvx = vecPoint.X;
            float fvy = vecPoint.Y;
            float fvz = vecPoint.Z;

            float fCos = MathF.Cos(fTheta);
            float fSin = MathF.Sin(fTheta);

            float fY = (fvy * fCos) - (fvz * fSin);
            float fZ = (fvy * fSin) + (fvz * fCos);

            Vector3 vecPointNew = new Vector3(fvx, fY, fZ);
            return vecPointNew;

        }
        public Vector3 GetRotatedVector(Vector3 vecPoint, Vector3 vecAxis, float fAngle)
        {
            float[,] aRotationMatrix = Operations.aRotationMatrixFromAxisAngle(vecAxis, fAngle);
            //http://scipp.ucsc.edu/~haber/ph216/rotation_12.pdf -- equation(20)
            Vector3 vecBase = new Vector3(0, 0, 0);
            Vector3 vecRotated = Operations.vecRotate(vecPoint, aRotationMatrix, vecBase);
            return vecRotated;
        }
    }
}
