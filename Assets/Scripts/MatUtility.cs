using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    public class MatUtility
    {
      
        public static int vecVecMul(int[] a, int[] b)
        {
            int result = 0;

            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                result += a[i] * b[i];
            }

            return result;
        }

        //m[zeile][spalte]


        public static int[] matVecMul(int[][] m, int[] v)
        {
            int[] result = new int[m.Length];

            for (int i = 0; i < m.Length; i++)
            {
                result[i] = vecVecMul(m[i], v);
            }

            return result;
        }

        public static int[][] transpose(int[][] a)
        {
            int[][] result = new int[a[0].Length][];

            for(int i = 0; i < a.Length; i++)
            {
                result[i] = new int[a.Length];
            }

            for (int i = 0; i < a.Length; i++)
            {               
                for (int j = 0; j < a[i].Length; j++)
                {
                    result[j][i] = a[i][j];
                }
            }

            return result;
        }

        public static int[][] matMatMult(int[][] a, int[][] b)
        {
            int[][] result = new int[a.Length][];
            int[][] transposed = transpose(b);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = matVecMul(a, transposed[i]);
            }
            return transpose(result);
        }

    }

