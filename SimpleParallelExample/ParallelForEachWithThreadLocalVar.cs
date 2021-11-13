using System;
using System.Collections.Concurrent;
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
            int[] nums = Enumerable.Range(0, 1_00_000_000).ToArray(); // million일 때는 그냥 for가 12배 빠르지만 숫자가 커질수록 Parallel 성능이 뛰어남
            long totalFromSumForLoop = 0; // Local variable to store sum
            long totalFromSumParallelForEachLoopThreadLocal = 0;
            long totalFromSumParallelForEachLoopPartitionLocal = 0;
            long totalFromSumParallelForEachLoopSpeedUpSmallBodyPartition = 0;

            var watch = Stopwatch.StartNew();
            totalFromSumForLoop = SumForLoop(nums);
            watch.Stop();

            var watchForParallelThreadLocal = Stopwatch.StartNew();
            totalFromSumParallelForEachLoopThreadLocal = SumParallelForLoopThreadLocal(nums);
            watchForParallelThreadLocal.Stop();

            var watchForParallelPartitionLocal = Stopwatch.StartNew();
            totalFromSumParallelForEachLoopPartitionLocal = SumParallelForEachLoopPartitionLocal(nums);
            watchForParallelPartitionLocal.Stop();

            var watchForParallelForEachLoopSpeedUpSmallBodyPartition = Stopwatch.StartNew();
            totalFromSumParallelForEachLoopSpeedUpSmallBodyPartition = SumParallelForEachLoopSpeedUpSmallBodyPartition(nums);
            watchForParallelForEachLoopSpeedUpSmallBodyPartition.Stop();

            Console.WriteLine($"Classical for loop | Total : {totalFromSumForLoop:N0} | Time Taken : {watch.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel Thread Local Loop  | Total : {totalFromSumParallelForEachLoopThreadLocal:N0} | Time Taken : {watchForParallelThreadLocal.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel Partition Local Loop  | Total : {totalFromSumParallelForEachLoopPartitionLocal:N0} | Time Taken : {watchForParallelPartitionLocal.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel Small Bodies Partition  | Total : {totalFromSumParallelForEachLoopSpeedUpSmallBodyPartition:N0} | Time Taken : {watchForParallelForEachLoopSpeedUpSmallBodyPartition.ElapsedMilliseconds} ms.");

            Console.WriteLine("Press Any key to exit");
            Console.ReadKey();
        }

        private static long SumForLoop(int[] nums)
        {
            long total = 0;

            for (int i = 0; i < nums.Length; i++)           total += nums[i];            

            return total;
        }

        private static long SumParallelForLoopThreadLocal(int[] nums)
        {
            long total = 0; // Local variable to store sum

            // Use type parameter to make subtotal a long, not an int
            Parallel.For<long>(
                fromInclusive: 0, 
                toExclusive: nums.Length,
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

        private static long SumParallelForEachLoopPartitionLocal(int[] nums)
        { // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-parallel-foreach-loop-with-partition-local-variables
            long total = 0;

            // First type parameter is the type of the source elements
            // Second type parameter is the type of the thread-local variable (partition subtotal)
            Parallel.ForEach<int, long>(
                source: nums, // Source collection
                localInit: () => 0, // method to initialize the local variable
                body: (j, loop, subtotal) => // method invoked by the loop on each iteration
                {
                    subtotal += j; // modify local variable
                    return subtotal; // value to be passed to next iteration
                },
                localFinally: (finalResult) => Interlocked.Add(ref total, finalResult)
                // Method to be executed when each partition has completed.
                // finalResult is the final value of subtotal for a particular partition.
            );

            return total;
        }

        // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-speed-up-small-loop-bodies
        // https://www.codeproject.com/Articles/161434/Task-Parallel-Library-3-of-n
        // Adapted and modified from the codes of docs above.
        // Small bodies chunk partitioning strategy seems to be more effective on "void" operation
        // such as ones described in the msdn doc above.
        // For example, array mapping operation like, in pythonic,  list(map(lambda x: x * PI, List<int> numArray))
        private static long SumParallelForEachLoopSpeedUpSmallBodyPartition(int[] nums)
        {
            long total = 0;

            var source = nums.ToList();

            // Partition the entire source array.
            OrderablePartitioner<Tuple<int,int>> rangePartitioner = Partitioner.Create(0, nums.Length, rangeSize:20000);

            // Loop over the partitions in parallel.
            Parallel.ForEach<Tuple<int,int>, long>(
                source: rangePartitioner,
                localInit: () => 0,
                body: (range, loopState, subtotal) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        subtotal += nums[i]; 
                    return subtotal;
                },
                localFinally: (finalResult) => Interlocked.Add(ref total, finalResult)
            );

            return total;
        }
    }
}
