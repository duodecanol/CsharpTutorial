using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleParallelExample
{
    //https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-foreach-loop
    public class SimpleParallelForEachLoop
    {
        public static void MainEntry()
        {
            // 2 million
            var limit = 2_000_000;
            var numbers = Enumerable.Range(0, limit).ToList();

            var watch = Stopwatch.StartNew();
            var primeNumbersFromForEach = GetPrimeList(numbers);
            watch.Stop();

            var watchForParallel = Stopwatch.StartNew();
            var primeNumbersFromParallelForEach = GetPrimeListWithParallel(numbers);
            watchForParallel.Stop();

            Console.WriteLine($"Classical foreach loop | Total prime numbers : {primeNumbersFromForEach.Count} | Time Taken : {watch.ElapsedMilliseconds} ms.");
            Console.WriteLine($"Parallel.ForEach loop  | Total prime numbers : {primeNumbersFromParallelForEach.Count} | Time Taken : {watchForParallel.ElapsedMilliseconds} ms.");

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// GetPrimeList returns Prime numbers by using sequential ForEach
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeList(IList<int> numbers) => numbers.Where(IsPrime).ToList(); // Linq
        
        /// <summary>
        /// GetPrimeListWithParallel returns Prime numbers by using Parallel.ForEach
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private static IList<int> GetPrimeListWithParallel(IList<int> numbers)
        {
            var primeNumbers = new ConcurrentBag<int>();

            Parallel.ForEach(numbers, number =>
            {
                if (IsPrime(number)) primeNumbers.Add(number);
            });

            return primeNumbers.ToList();
        }

        /// <summary>
        /// IsPrime returns true if number is Prime, else false.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>bool indicates whether the number is prime </returns>
        private static bool IsPrime(int number)
        {
            if (number < 2) return false;

            // try division by 2 to upper bound of square root of the number to determine Prime number
            for (var divisor = 2; divisor <= Math.Sqrt(number); divisor++)
            {
                if (number % divisor == 0) return false;
            }
            return true;
            // e.g. number = 11 =>  3.31  11/2, 11/3 
        }
    }
}
