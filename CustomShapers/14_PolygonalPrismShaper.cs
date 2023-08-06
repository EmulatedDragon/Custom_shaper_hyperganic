// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Hyperganic
{
    /// <summary>
    ///     <para> The polygonal prism custom shaper creates a polygonal prism with a height and a length from Center to all the edges. </para>
    ///     <para> Input: float fHeight, float fDistanceToCenter, float fXCenter, float fYCenter, float fZCenter. Output: polygonal prism assembly 
    ///     structure.</para>
    /// </summary>
    public class PolygonalPrismShaper : IShaper
    {
        public override string strDescription()
        {
            return "Creates and returns a polygonal prism assembly";
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.IntEditBoxParameter("iNumberOfSides", "number of sides on the polygonal prism"));
            AddParameter(new Hy.FloatEditBoxParameter("fHeight", "height of the polygonal prism"));
            AddParameter(new Hy.FloatEditBoxParameter("fDistanceToCenter", "distance to the edge of polygonal prism from center"));
            AddParameter(new Hy.FloatEditBoxParameter("fXCenter", "X coordinate of the starting bottom center point of polygonal prism"));
            AddParameter(new Hy.FloatEditBoxParameter("fYCenter", "Y coordinate of the starting bottom center point of polygonal prism"));
            AddParameter(new Hy.FloatEditBoxParameter("fZCenter", "Z coordinate of the starting bottom center point of polygonal prism"));
            AddParameter(new Hy.FloatEditBoxParameter("fAssemblyBias", "The bias (thickness) of the assembly"));
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.AssemblyParameter());
		}

        /// <summary>
        /// <para> This custom shaper is slightly different from the rest in the sense that it returns an assembly and not a lattice. As such, a solid object
        /// is created at the start instead of a lattice. Using the number of sides that we want for a polygon, we can determine the theta value for each iteration.
        /// Next, we obtain and all vertices to a list of vectors. Along each side face of a polygonal prism, we draw 2 triangles to form the rectangular area.
        /// Then, we use the top and bottom edges to draw a triangular face (2 vertices of the edge + center point forms a triangle) on top and bottom respectively.
        /// </para>
        /// </summary>
        public override bool bProduceResult()
        {
            bool bOk = true;
            int iNumberOfSides = Hy.ShaperInterface.iGetIntArgument("iNumberOfSides", ref bOk);
            float fHeight = Hy.ShaperInterface.fGetFloatArgument("fHeight", ref bOk);
            float fDistanceToCenter = Hy.ShaperInterface.fGetFloatArgument("fDistanceToCenter", ref bOk);
            float fXCenter = Hy.ShaperInterface.fGetFloatArgument("fXCenter", ref bOk);
            float fYCenter = Hy.ShaperInterface.fGetFloatArgument("fYCenter", ref bOk);
            float fZCenter = Hy.ShaperInterface.fGetFloatArgument("fZCenter", ref bOk);
            float fAssemblyBias = Hy.ShaperInterface.fGetFloatArgument("fAssemblyBias", ref bOk);

            fHeight -= 2 * fAssemblyBias;
            fDistanceToCenter -= fAssemblyBias;

            Hy.Assembly oAssembly = GetPolygonalPrismAssembly(iNumberOfSides, fDistanceToCenter, fHeight, new Vector3(fXCenter, fYCenter, fZCenter));

			Hy.ShaperInterface.Return(0, oAssembly);
            return true;
        }
        public static Hy.Assembly GetPolygonalPrismAssembly(int iSides, float fRadius, float fHeight, Vector3 vecOrigin)
        {
            float fAngle = 2 * MathF.PI / iSides;
            List<Vector3[]> vecPairList = new List<Vector3[]>();

            // adding top and bottom vector points to a list
            for (int i = 0; i < iSides; i++)
            {
                Vector3[] VecPair = new Vector3[2];
                float fTheta = i * fAngle;
                float fX = fRadius * MathF.Cos(fTheta);
                float fY = fRadius * MathF.Sin(fTheta);
                VecPair[0] = vecOrigin + new Vector3(fX, fY, 0);
                VecPair[1] = vecOrigin + new Vector3(fX, fY, fHeight);
                vecPairList.Add(VecPair);
            }

            List<Vector3> vecListVertices = new List<Vector3>();

            for (int iTopBottom = 0; iTopBottom < 2; iTopBottom++) // add bottom points first, then add top points
            {
                for (int i = 0; i < vecPairList.Count; i++)
                {
                    vecListVertices.Add(vecPairList[i][iTopBottom]);
                }
            }

            Hy.MeshData oMeshData = new Hy.MeshData();
            for (int i = 0; i < vecPairList.Count; i++)
            {
                int iCurrent = i;
                int iNext = i + 1;
                if (i == vecPairList.Count - 1) iNext = 0;

                Vector3 vec01 = vecPairList[iCurrent][0];//bottom
                Vector3 vec02 = vecPairList[iCurrent][1];//top
                Vector3 vec11 = vecPairList[iNext][0];//bottom
                Vector3 vec12 = vecPairList[iNext][1];//top
                oMeshData.nAddTriangle(vec01, vec12, vec02);
                oMeshData.nAddTriangle(vec01, vec11, vec12);

                //adding top and bottom triangles
                Vector3 vecMiddleTop = vecOrigin + new Vector3(0, 0, fHeight);
                Vector3 vecMiddleBottom = vecOrigin + new Vector3(0, 0, 0);
                oMeshData.nAddTriangle(vec02, vec12, vecMiddleTop);
                oMeshData.nAddTriangle(vec01, vec11, vecMiddleBottom);
            }

            Hy.Assembly oAssembly = new Hy.Assembly(oMeshData);
            return oAssembly;
        }
    }
}


