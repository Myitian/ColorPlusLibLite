using System;

namespace Myitian.ColorPlusLibLite.XMath
{
    public static class Math_
    {
        public static double Degrees(double rad)
        {
            return 180 / Math.PI * rad;
        }
        public static double Radians(double deg)
        {
            return Math.PI / 180 * deg;
        }
        public static double SafeSqrt(double d)
        {
            d = Math.Sqrt(d);
            return d == double.NaN ? 0 : d;
            //return Math.Sqrt(Math.Abs(d));
        }

        /// <summary>
        /// 欧几里得距离
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static double EuclideanDistance(double x0, double y0, double x1, double y1)
        {
            return SafeSqrt(EuclideanDistance_Square(x0, y0, x1, y1));
        }
        /// <summary>
        /// 欧几里得距离
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <returns></returns>
        public static double EuclideanDistance(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            return SafeSqrt(EuclideanDistance_Square(x0, y0, z0, x1, y1, z1));
        }
        public static double EuclideanDistance(in ColorBase col0, in ColorBase col1)
        {
            return SafeSqrt(EuclideanDistance_Square(in col0, in col1));
        }

        public static double EuclideanDistance_Square(double x0, double y0, double x1, double y1)
        {
            return Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2);
        }
        public static double EuclideanDistance_Square(in ColorBase col0, in ColorBase col1)
        {
            return EuclideanDistance_Square(col0.V0, col0.V1, col0.V2, col1.V0, col1.V1, col1.V2);
        }
        public static double EuclideanDistance_Square(double x0, double y0, double z0, double x1, double y1, double z1)
        {
            return Math.Pow(x0 - x1, 2) + Math.Pow(y0 - y1, 2) + Math.Pow(z0 - z1, 2);
        }
    }

    //原内容：
    //孙继磊，2010-10-18
    //sun.j.l.studio@gmail.com

    //一些修改

    /// <summary>
    /// 矩阵类
    /// </summary>
    public sealed class Matrix
    {
        public int Row { get; private set; }              //矩阵的行数
        public int Column { get; private set; }           //矩阵的列数
        public double[,] Data { get; private set; }       //矩阵的数据

        #region 构造函数
        public Matrix(int rowNum, int columnNum)
        {
            Row = rowNum;
            Column = columnNum;
            Data = new double[Row, Column];
        }
        public Matrix(double[,] members)
        {
            Row = members.GetUpperBound(0) + 1;
            Column = members.GetUpperBound(1) + 1;
            Data = new double[Row, Column];
            Array.Copy(members, Data, Row * Column);
        }
        public Matrix(double[] vector)
        {
            Row = 1;
            Column = vector.GetUpperBound(0) + 1;
            Data = new double[1, Column];
            for (int i = 0; i < vector.Length; i++)
            {
                Data[0, i] = vector[i];
            }
        }
        public Matrix(double[] vector, bool isSingleColumn)
        {
            if (isSingleColumn)
            {
                Column = 1;
                Row = vector.GetUpperBound(0) + 1;
                Data = new double[Row, 1];
                for (int i = 0; i < vector.Length; i++)
                {
                    Data[i, 0] = vector[i];
                }
            }
            else
            {
                Row = 1;
                Column = vector.GetUpperBound(0) + 1;
                Data = new double[1, Column];
                for (int i = 0; i < vector.Length; i++)
                {
                    Data[0, i] = vector[i];
                }
            }
        }
        #endregion

        #region 属性和索引器
        public int RowNum { get { return Row; } }
        public int ColumnNum { get { return Column; } }

        public double this[int r, int c]
        {
            get { return Data[r, c]; }
            set { Data[r, c] = value; }
        }
        #endregion

        #region 操作符重载 *

        public static Matrix operator *(in Matrix a, in Matrix b)
        {
            if (a.Column != b.Row) throw new Exception("行数或列数不匹配");
            Matrix result = new Matrix(a.Row, b.Column);
            for (int i = 0; i < a.Row; i++)
                for (int j = 0; j < b.Column; j++)
                    for (int k = 0; k < a.Column; k++)
                        result[i, j] += a[i, k] * b[k, j];

            return result;
        }
        #endregion

        public override string ToString()
        {
            string strRet = "";
            for (int i = 0; i < Row; i++)
                for (int j = 0; j < Column; j++)
                {
                    strRet += Data[i, j] + " , ";
                }
            return strRet;
        }

        public static implicit operator Matrix(in double[,] members)
        {
            return new Matrix(members);
        }
        public static implicit operator double[,](in Matrix matrix)
        {
            return matrix.Data;
        }

        public Matrix Inverse()
        {
            return GetInverseMatrix(this);
        }
        public static Matrix GetInverseMatrix(in Matrix martix)
        {
            int
                m = martix.Data.GetLength(0),
                n = martix.Data.GetLength(1);
            double[,] array = new double[2 * m + 1, 2 * n + 1];
            for (int k = 0; k < 2 * m + 1; k++)  //初始化数组
            {
                for (int t = 0; t < 2 * n + 1; t++)
                {
                    array[k, t] = 0;
                }
            }
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    array[i, j] = martix[i, j];
                }
            }

            for (int k = 0; k < m; k++)
            {
                for (int t = n; t <= 2 * n; t++)
                {
                    if ((t - k) == m)
                    {
                        array[k, t] = 1;
                    }
                    else
                    {
                        array[k, t] = 0;
                    }
                }
            }
            //得到逆矩阵
            for (int k = 0; k < m; k++)
            {
                if (array[k, k] != 1)
                {
                    double bs = array[k, k];
                    array[k, k] = 1;
                    for (int p = k + 1; p < 2 * n; p++)
                    {
                        array[k, p] /= bs;
                    }
                }
                for (int q = 0; q < m; q++)
                {
                    if (q != k)
                    {
                        double bs = array[q, k];
                        for (int p = 0; p < 2 * n; p++)
                        {
                            array[q, p] -= bs * array[k, p];
                        }
                    }
                }
            }
            double[,] InverseMatrix = new double[m, n];
            for (int x = 0; x < m; x++)
            {
                for (int y = n; y < 2 * n; y++)
                {
                    InverseMatrix[x, y - n] = array[x, y];
                }
            }
            return InverseMatrix;
        }
    }
}
