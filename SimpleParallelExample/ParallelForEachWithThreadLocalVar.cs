using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SimpleParallelExample
{
    // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-parallel-for-loop-with-thread-local-variables
    public class ParallelForEachWithThreadLocalVar
    {
        public static void MainEntry()
        {
            int[] nums = Enumerable.Range(0, 1_000_000_000).ToArray(); // million일 때는 그냥 for가 12배 빠르지만 숫자가 커질수록 Parallel 성능이 뛰어남
            long totalFromSumForLoop = 0; // Local variable to store sum
            long totalFromSumParallelForEachLoop = 0;

            var watch = Stopwatch.StartNew();
            totalFromSumForLoop = SumForLoop(nums);
            watch.Stop();

            var watchForParallel = Stopwatch.StartNew();
            totalFromSumParallelForEachLoop = SumParallelForLoop(nums);
            watchForParallel.Stop();

            Console.WriteLine($"Classical for loop | Total : {totalFromSumForLoop:N0} | Time Taken : {watch.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel Thread Local loop  | Total : {totalFromSumParallelForEachLoop:N0} | Time Taken : {watchForParallel.ElapsedMilliseconds} ms.");
            
            Console.WriteLine("Press Any key to exit");
            Console.ReadKey();
        }

        private static long SumForLoop(int[] nums)
        {
            long total = 0;

            for (int i = 0; i < nums.Length; i++)
            {
                total += nums[i];
            }

            return total;
        }

        private static long SumParallelForLoop(int[] nums)
        {
            long total = 0; // Local variable to store sum

            // Use type parameter to make subtotal a long, not an int
            Parallel.For<long>(fromInclusive: 0, toExclusive: nums.Length,
                localInit: () => 0,
                body: (j, loop, subtotal) =>
                {
                    subtotal += nums[j];
                    return subtotal;
                },
              localFinally: (x) => Interlocked.Add(ref total, x) // Adds up value x to total
            );

            return total;
        }
    }
}
