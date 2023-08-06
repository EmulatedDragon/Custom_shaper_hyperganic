namespace Hy
{
    using System;
    using System.Numerics;


    public static partial class Operations
    {
        public static Vector3 vecRotate(Vector3 vecPoint, Vector3 vecEulerAngle)
            => vecRotate(vecPoint, vecEulerAngle, Vector3.Zero);

        public static Vector3 vecRotate(Vector3 vecPoint, float[,] aafRotationMatrix)
            => vecRotate(vecPoint, aafRotationMatrix, Vector3.Zero);

        public static Vector3 vecRotate(Vector3 vecPoint, float[,] aafRotationMatrix, Vector3 vecRotationBase)
        {
            float[,] aafPoint = new float[3, 1];
            aafPoint[0, 0] = vecPoint.X - vecRotationBase.X;
            aafPoint[1, 0] = vecPoint.Y - vecRotationBase.Y;
            aafPoint[2, 0] = vecPoint.Z - vecRotationBase.Z;

            float[,] aafResult = Operations.aMatrixMultiplication(aafRotationMatrix, aafPoint);

            return new Vector3(aafResult[0, 0], aafResult[1, 0], aafResult[2, 0]) + vecRotationBase;
        }

        public static Vector3 vecRotate(Vector3 vecPoint, Vector3 vecEulerAngle, Vector3 vecRotationBase)
        {
            float[,] aafRotationMatrix = new float[3, 3];

            // First row
            aafRotationMatrix[0, 0] = (float)(Math.Cos(vecEulerAngle.Z) * Math.Cos(vecEulerAngle.Y));
            aafRotationMatrix[0, 1] = (float)(Math.Cos(vecEulerAngle.Z) * Math.Sin(vecEulerAngle.Y)
                * Math.Sin(vecEulerAngle.X) - Math.Sin(vecEulerAngle.Z) * Math.Cos(vecEulerAngle.X));
            aafRotationMatrix[0, 2] = (float)(Math.Cos(vecEulerAngle.Z) * Math.Sin(vecEulerAngle.Y)
                * Math.Cos(vecEulerAngle.X) + Math.Sin(vecEulerAngle.Z) * Math.Sin(vecEulerAngle.X));

            // Second row
            aafRotationMatrix[1, 0] = (float)(Math.Sin(vecEulerAngle.Z) * Math.Cos(vecEulerAngle.Y));
            aafRotationMatrix[1, 1] = (float)(Math.Sin(vecEulerAngle.Z) * Math.Sin(vecEulerAngle.Y)
                * Math.Sin(vecEulerAngle.X) + Math.Cos(vecEulerAngle.Z) * Math.Cos(vecEulerAngle.X));
            aafRotationMatrix[1, 2] = (float)(Math.Sin(vecEulerAngle.Z) * Math.Sin(vecEulerAngle.Y)
                * Math.Cos(vecEulerAngle.X) - Math.Cos(vecEulerAngle.Z) * Math.Sin(vecEulerAngle.X));

            // Third row
            aafRotationMatrix[2, 0] = -(float)Math.Sin(vecEulerAngle.Y);
            aafRotationMatrix[2, 1] = (float)(Math.Cos(vecEulerAngle.Y) * Math.Sin(vecEulerAngle.X));
            aafRotationMatrix[2, 2] = (float)(Math.Cos(vecEulerAngle.Y) * Math.Cos(vecEulerAngle.X));

            vecPoint -= vecRotationBase;
            float[,] aafPoint = new float[3, 1]
            {
                { vecPoint.X },
                { vecPoint.Y },
                { vecPoint.Z }
            };

            float[,] aafProduct = aMatrixMultiplication(aafRotationMatrix, aafPoint);
            Vector3 vecRotatedPoint = new Vector3(aafProduct[0, 0],
                                                  aafProduct[1, 0],
                                                  aafProduct[2, 0]);
            return vecRotatedPoint + vecRotationBase;
        }

        public static float[,] aRotationMatrixFromAxisAngle(Vector3 vecRotationAxis, float fAngle)
        {
            Vector3 vecAxis = Vector3.Normalize(vecRotationAxis);
            float fC = (float)Math.Cos(fAngle);
            float fS = (float)Math.Sin(fAngle);
            float fT = 1 - fC;

            float[,] aafR = new float[3, 3];

            // First row
            aafR[0, 0] = (float)(fT * Math.Pow(vecAxis.X, 2) + fC);
            aafR[0, 1] = fT * vecAxis.X * vecAxis.Y - vecAxis.Z * fS;
            aafR[0, 2] = fT * vecAxis.X * vecAxis.Z + vecAxis.Y * fS;

            // Second row
            aafR[1, 0] = fT * vecAxis.X * vecAxis.Y + vecAxis.Z * fS;
            aafR[1, 1] = (float)(fT * Math.Pow(vecAxis.Y, 2) + fC);
            aafR[1, 2] = fT * vecAxis.Y * vecAxis.Z - vecAxis.X * fS;

            // Third row
            aafR[2, 0] = fT * vecAxis.X * vecAxis.Z - vecAxis.Y * fS;
            aafR[2, 1] = fT * vecAxis.Y * vecAxis.Z + vecAxis.X * fS;
            aafR[2, 2] = (float)(fT * Math.Pow(vecAxis.Z, 2) + fC);

            return aafR;
        }

        // Because I am honestly very confused with C# templates and generic types in general
        public static float[,] aRotationMatrixFromAxisAngle(Vector3 vecRotationAxis, double dAngle)
        {
            Vector3 vecAxis = Vector3.Normalize(vecRotationAxis);
            double dC = Math.Cos(dAngle);
            double dS = Math.Sin(dAngle);
            double dT = 1 - dC;

            float[,] aafR = new float[3, 3];

            // First row
            aafR[0, 0] = (float)(dT * Math.Pow(vecAxis.X, 2) + dC);
            aafR[0, 1] = (float)(dT * vecAxis.X * vecAxis.Y - vecAxis.Z * dS);
            aafR[0, 2] = (float)(dT * vecAxis.X * vecAxis.Z + vecAxis.Y * dS);

            // Second row
            aafR[1, 0] = (float)(dT * vecAxis.X * vecAxis.Y + vecAxis.Z * dS);
            aafR[1, 1] = (float)(dT * Math.Pow(vecAxis.Y, 2) + dC);
            aafR[1, 2] = (float)(dT * vecAxis.Y * vecAxis.Z - vecAxis.X * dS);

            // Third row
            aafR[2, 0] = (float)(dT * vecAxis.X * vecAxis.Z - vecAxis.Y * dS);
            aafR[2, 1] = (float)(dT * vecAxis.Y * vecAxis.Z + vecAxis.X * dS);
            aafR[2, 2] = (float)(dT * Math.Pow(vecAxis.Z, 2) + dC);

            return aafR;
        }
    }
}
