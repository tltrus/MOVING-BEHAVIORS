using System;
using System.Threading.Tasks;

namespace DrawingVisualApp
{
    class Matrix2d
    {
        public double M11;

        public double M12;

        public double M13;

        public double M21;

        public double M22;

        public double M23;

        public double M31;

        public double M32;

        public double M33;

        public Matrix2d()
        {
        }

        public void Rotate(double angle)
        {
            double num = angle * Math.PI / 180.0;
            M11 = Math.Cos(num);
            M12 = Math.Sin(num);
            M13 = 0.0;
            M21 = 0.0 - Math.Sin(num);
            M22 = Math.Cos(num);
            M23 = 0.0;
            M31 = 0.0;
            M32 = 0.0;
            M33 = 1.0;
        }

        public void Translate(double Tx, double Ty)
        {
            M11 = 1.0;
            M12 = 0.0;
            M13 = 0.0;
            M21 = 0.0;
            M22 = 1.0;
            M23 = 0.0;
            M31 = Tx;
            M32 = Ty;
            M33 = 1.0;
        }

        public double[,] ToArray()
        {
            return new double[3, 3]
            {
            { M11, M12, M13 },
            { M21, M22, M23 },
            { M31, M32, M33 }
            };
        }

        public static double[,] Create(int rows, int cols)
        {
            double[,] array = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = 0.0;
                }
            }

            return array;
        }

        public static double[,] Mult(double[,] matrixA, double[,] matrixB)
        {
            int length = matrixA.GetLength(0);
            int aCols = matrixA.GetLength(1);
            int length2 = matrixB.GetLength(0);
            int bCols = matrixB.GetLength(1);
            if (aCols != length2)
            {
                throw new Exception("Non-conformable matrices in MatrixProduct");
            }

            double[,] result = Create(length, bCols);
            Parallel.For(0, length, delegate (int i)
            {
                for (int j = 0; j < bCols; j++)
                {
                    for (int k = 0; k < aCols; k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            });
            return result;
        }

        public static Matrix2d operator *(Matrix2d a, Matrix2d b)
        {
            double m = a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31;
            double m2 = a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32;
            double m3 = a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33;
            double m4 = a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31;
            double m5 = a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32;
            double m6 = a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33;
            double m7 = a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31;
            double m8 = a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32;
            double m9 = a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33;
            return new Matrix2d
            {
                M11 = m,
                M12 = m2,
                M13 = m3,
                M21 = m4,
                M22 = m5,
                M23 = m6,
                M31 = m7,
                M32 = m8,
                M33 = m9
            };
        }
    }
}
