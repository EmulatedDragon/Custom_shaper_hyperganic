namespace Hy
{
    using System;
    public static partial class Operations
    {
        public static float[,] aMatrixMultiplication(float[,] aafMatrix1, float[,] aafMatrix2)
        {
            int iNumRows1 = aafMatrix1.GetLength(0);
            int iNumCols1 = aafMatrix1.GetLength(1);
            int iNumRows2 = aafMatrix2.GetLength(0);
            int iNumCols2 = aafMatrix2.GetLength(1);

            if (iNumCols1 != iNumRows2)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

            float[,] aafProduct = new float[iNumRows1, iNumCols2];

            unsafe
            {
                fixed (float* paafProduct = aafProduct,
                       paafMatrix1 = aafMatrix1,
                       paafMatrix2 = aafMatrix2)
                {
                    int i = 0;

                    // looping through matrix 1 rows  
                    for (int iRow1 = 0; iRow1 < iNumRows1; ++iRow1)
                    {
                        // for each matrix 1 row, loop through matrix 2 columns  
                        for (int iCol2 = 0; iCol2 < iNumCols2; ++iCol2)
                        {
                            // loop through matrix 1 columns to calculate the dot product  
                            for (int iCol1 = 0; iCol1 < iNumCols1; iCol1++)
                            {
                                float fVal1 = *(paafMatrix1 + (iNumRows1 * iRow1) + iCol1);
                                float fVal2 = *(paafMatrix2 + (iNumCols2 * iCol1) + iCol2);

                                *(paafProduct + i) += fVal1 * fVal2;
                            }

                            ++i;
                        }
                    }
                }
            }

            return aafProduct;
        }
    }
}
