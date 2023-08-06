namespace Hy
{
    using System.Collections.Generic;
    using System.Numerics;

    public class PathLengthCounter
    {
        protected List<float> aPrefixSum;
        private int iCounter;
        private Vector3 vecPreviousPoint;
        public PathLengthCounter()
        {
            aPrefixSum = new List<float>();
            aPrefixSum.Add(0);
            iCounter = -1;
        }

        public void AddNode(Vector3 pathNode)
        {
            if (iCounter == -1)
            {
                vecPreviousPoint = pathNode;
                iCounter++;
                return;
            }
            float distance = (pathNode - vecPreviousPoint).Length();
            aPrefixSum.Add(aPrefixSum[iCounter] + distance);
            iCounter++;
            vecPreviousPoint = pathNode;
        }

        public float fGetDistanceBetweenNodes(int nodeID1, int nodeID2)
        {
            return aPrefixSum[nodeID2] - aPrefixSum[nodeID1];
        }
    }
}