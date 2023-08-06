using System;

namespace Hy
{
    public static class DiscreteDistributions
    {
        /// <summary>Derives a discrete point on a Triangle-shaped distribution.</summary>
        public static float fTriangleShape(float fStartX, float fEndX, float fMaxY, float fMinY, float fCurrentX)
        {
            float fMidX = (fEndX - fStartX) / 2;
            float fY = 0;

            if (fCurrentX < fMidX)
            {
                fY = fLinearShape(fMaxY, fMinY, fStartX, fMidX, fCurrentX);
            }
            else if (fCurrentX > fMidX)
            {
                fY = fLinearShape(fMinY, fMaxY, fMidX, fEndX, fCurrentX);
            }
            else
            {
                fY = fMaxY;
            }

            return fY;
        }


        /// <summary>
        /// Function to derive a discrete point on a Linear function based on discrete starting and end points
        /// </summary>
        public static float fLinearShape(float fMinY, float fMaxY, float fStartX, float fEndX, float fCurrentX)
        {
            float m = (fMaxY - fMinY) / (fEndX - fStartX);
            float Y = m * (fCurrentX - fStartX) + fMinY;
            return Y;
        }

        /// <summary>
        /// Funtion to derive a discrete point in a Gaussian-Normal-distribution
        /// </summary>
        public static float fGaussShape(float fStandardDeviationX, float fStartX, float fEndX, float fMaxY, float fMinY, float fCurrentX)
        {
            float xLength = fEndX - fStartX;
            float xMean = fStartX + xLength / 2;
            float yDiff = fMaxY - fMinY;
            float factor = (float)(1 / (Math.Sqrt(Math.PI * 2)));
            float exponent = -0.5f * ((fCurrentX - xMean) / fStandardDeviationX) * ((fCurrentX - xMean) / fStandardDeviationX);
            float Y;
            if (fCurrentX - xMean == 0)
            {
                Y = yDiff + fMinY;
            }
            else
            {
                Y = yDiff * (float)Math.Exp(exponent) + fMinY;
            }

            return Y;
        }


        /// <summary>
        /// Function to derive a discrete point in a circle-shaped distribution
        /// </summary>
        public static float fCircleShape(float fCurrentX, float fStartX, float fMinY, float f_RadiusInXAndY)
        {
            float Y = (float)(Math.Sqrt(f_RadiusInXAndY * f_RadiusInXAndY - (fCurrentX - fStartX) * (fCurrentX - fStartX)) + fMinY);
            return Y;
        }

        /// <summary>
        /// Funtion to derice a discrete point on an elliptically shaped distribution
        /// </summary>
        public static float fEllipseShape(float fCurrentX, float fStartX, float f_RadiusInX, float f_RadiusInY)
        {
            float Y = f_RadiusInY * (float)(Math.Sqrt(f_RadiusInX * f_RadiusInX - (fCurrentX - fStartX) * (fCurrentX - fStartX)));
            return Y;
        }

        /// <summary>
        /// <para>Function to derive a discrete point on a Tangens-Hyperbolicus-function</para>
        /// <para>Parameters:</para>
        /// <para>fTransitionX dictates the location of the steepest slope (e.g. transition) on the Tanh function</para>
        /// <para>fSmoothingRatio dicates the range of the steepest slope, hence the steepness</para>
        /// <para>Reference: https://de.wikipedia.org/wiki/Tangens_hyperbolicus_und_Kotangens_hyperbolicus </para>
        /// </summary>
        public static float fTanSmooth(float fCurrentX, float fTransitionX, float fSmoothingRatio)
        {
            float Y = (float)(0.5 + 0.5 * Math.Tanh((fCurrentX - fTransitionX) / fSmoothingRatio));
            return Y;
        }

        /// <summary>
        /// <para>The SuperShaper describes a generalisation of the Lametian Curve </para>
        /// <para>Parameters:</para>
        /// <para>f_m dictates the symmetry </para>
        /// <para>f_n1, f_n2, f_n3 dicate the shape </para>
        /// <para>f_a and f_b describe the percentagewise enlagement in x and y (see ellipse)</para>
        /// <para>Reference: https://de.wikipedia.org/wiki/Superformel </para>
        /// </summary>
        public static float fSuperShaper(float fAngle, float fShapeRadius, float f_m, float f_n1, float f_n2, float f_n3, float f_a = 1, float f_b = 1)
        {

            float fRADIUS = fShapeRadius;
            float fLimitRadius = fRADIUS * (float)(Math.Pow(Math.Pow((Math.Abs(Math.Cos(f_m * fAngle / 4) / f_a)), f_n2) + Math.Pow((Math.Abs(Math.Sin(f_m * fAngle / 4) / f_b)), f_n3), -1 / f_n1));
            return fLimitRadius;
        }

    }
}



