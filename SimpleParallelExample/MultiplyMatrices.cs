using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


/* This example uses the Parallel.For method to compute the product of two matrices. 
 * It also shows how to use the System.Diagnostics.Stopwatch class to compare the performance of a parallel loop with a non-parallel loop. 
 * Note that, because it can generate a large volume of output, the example allows output to be redirected to a file.
 * 
 * https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-for-loop#matrix-and-stopwatch-example
 */
namespace SimpleParallelExample
{
    public class MultiplyMatrices
    {
        #region Sequential_Loop
        static void MultiplyMatricesSequential(double[,] matA, double[,] matB, double[,] result)
        {
            int matACols = matA.GetLength(1);
            int matBCols = matB.GetLength(1);
            int matARows = matA.GetLength(0);

            for (int i=0; i < matARows; i++)
            {
                for (int j=0; j < matBCols; j++)
                {
                    double temp = 0;
                    for (int k=0; k < matACols; k++)
                    {
                        temp += matA[i, k] * matB[k, j];
                    }
                    result[i, j] += temp;
                }
            }
        }
        #endregion

        #region Parallel_Loop
        static void MultiplyMatricesParallel(double[,] matA, double[,] matB, double[,] result)
        {
            int matACols = matA.GetLength(1);
            int matBCols = matB.GetLength(1);
            int matARows = matA.GetLength(0);

            // A basic matrix multiplication.
            // Parallelize the outer loop to partition the source array by rows.
            Parallel.For(fromInclusive: 0, toExclusive: matARows, i => 
            {
                for (int j = 0; j < matBCols; j++)
                {
                    double temp = 0;
                    for (int k = 0; k < matACols; k++)
                    {
                        temp += matA[i, k] * matB[k, j];
                    }
                    result[i, j] += temp;
                }
            }); // Parallel.For
        }
        #endregion

        #region Main
        public static void MainSub()
        {
            // Set up matrices. Use small values to better view result matrix.
            // Increase the counts to see greater speedup in the parallel loop vs the sequential loop.
            /* 
             * === Result of 2000/2000/2000 ===
             * Executing sequential Loop ....
             * Sequential loop time in milliseconds: 103075 ... 103 seconds
             * Executing parallel loop... 
             * Parallel loop time in milliseconds: 11195    ... 11 seconds
            */
            int colCount = 2000;
            int rowCount = 2000;
            int colCount2 = 2000;
            double[,] m1 = InitializeMatrix(rowCount, colCount);
            double[,] m2 = InitializeMatrix(colCount, colCount2);
            double[,] result = new double[rowCount, colCount2];

            // First do the Sequential version.
            Console.Error.WriteLine("Executing sequential Loop ....");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            MultiplyMatricesSequential(m1, m2, result);
            stopwatch.Stop();
            Console.Error.WriteLine($"Sequential loop time in milliseconds: {stopwatch.ElapsedMilliseconds}");

            // For the skeptics.
            //OfferToPrint(rowCount, colCount, result);

            // Reset timer and result matrix.
            stopwatch.Reset();
            result = new double[rowCount, colCount2];

            // Do the parallel loop.
            Console.Error.WriteLine("Executing parallel loop...");
            stopwatch.Start();
            MultiplyMatricesParallel(m1, m2, result);
            stopwatch.Stop();
            Console.Error.WriteLine($"Parallel loop time in milliseconds: {stopwatch.ElapsedMilliseconds}");
            //OfferToPrint(rowCount, colCount2, result);

            // Keep the console window open in debug mode.
            Console.Error.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        #endregion
        #region Helper_Methods
        static double[,] InitializeMatrix(int rows, int cols)
        {
            double[,] matrix = new double[rows, cols];

            Random r = new Random();
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = r.Next(100);
            
            return matrix;
        }

        private static void OfferToPrint(int rowCount, int colCount, double[,] matrix)
        {
            Console.Error.Write("Computation complete. Print results (y/n)? ");
            char c = Console.ReadKey(true).KeyChar;
            Console.Error.WriteLine(c);
            if (Char.ToUpperInvariant(c) == 'Y')
            {
                if (!Console.IsOutputRedirected) Console.WindowWidth = 180;
                Console.WriteLine();
                for (int x=0; x < rowCount; x++)
                {
                    Console.WriteLine($"ROW {x}");
                    for (int y=0; y < colCount; y++)
                    {
                        Console.WriteLine($"{matrix[x, y]}");
                    }
                    Console.WriteLine();
                }
            }
        }
        #endregion
    }


}
