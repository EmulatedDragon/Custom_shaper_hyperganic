// < This is Source Code to be used on Hyperganic Core. >
// < The Hyperganic Platform - Source Code License applies to the usage of this code. > 
// < https://gitlab.hyperganic.com/hyperganic-platformcommunity/license > 

using System.Numerics;

namespace Hyperganic
{
    public class MyCustomShaper : IShaper
    {
        /// <summary>
        ///     <para> After you have gone through the concepts covered here, you may take the Level 2 Algorithmic Engineer test </para>
        ///     <para> For the test, you are required to use 2 or more concepts that were covered here to create an object using custom shapers </para>
        ///     <para> There are 2 parts to this test:
        ///     <para> 1. Pseudo-Code section </para>
        ///     <para> 2. Writing your custom shaper section</para> </para>
        ///     <para> Input: ... </para>
        ///     <para> Output: ... </para>
        /// </summary>
        public override string strDescription()
        {
            return "Creates and returns a sphere lattice"; // TODO_1: what object do you want to produce
        }

        public override string strType()
        {
            return "filter";
        }

        public override void SetParameters()
        {
            AddParameter(new Hy.FloatEditBoxParameter("fRadius", "radius of the sphere"));
            // TODO_3: What parameters do you need as inputs?
        }
        public override void SetReturns()
        {
            AddReturn(new Hy.LatticeParameter());
        }

        public override bool bProduceResult()
        {
            bool bOk = true;
            // TODO_2: Writing the pseudo code. Below are some of the considerations.
            // Can the object be made up of smaller components ? (e.g.Surfaces can be made of many line components).
            // Keep it simple, concise, and readable.
            // Use indentations to show hierarchy, improve readability, and show nested constructs. 

            // TODO_4: Writing the custom shaper.
            float fBeamRadius = Hy.ShaperInterface.fGetFloatArgument("fRadius", ref bOk);

            Hy.Lattice oLattice = new Hy.Lattice();

            Vector3 vecPoint = new Vector3(0);
            oLattice.AddBeam(vecPoint, fBeamRadius, vecPoint, fBeamRadius);

            Hy.ShaperInterface.Return(0, oLattice);
            return true;
        }

    }
}


