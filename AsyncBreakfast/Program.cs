using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncBreakfast
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //ExecuteBadPractice();
            await ExecuteBetterPractice();
        }
        private static async Task ExecuteBetterPractice()
        {
            Console.WriteLine("Start preparing delicious breakfast");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready"); Console.WriteLine();

            Egg eggs = await FryEggsAsync(2);
            Console.WriteLine("eggs are ready"); Console.WriteLine();

            Bacon bacon = await FryBaconAsync(3);
            Console.WriteLine("bacon is ready"); Console.WriteLine();

            Toast toast = await ToastBreadAsync(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("toast is ready"); Console.WriteLine();

            Juice oj = PourOrangeJuice();
            Console.WriteLine("oj is ready"); Console.WriteLine();
            Console.WriteLine("Breakfast is ready!"); Console.WriteLine();

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine($"Time elapsed: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds:00}");
        }
        private static void ExecuteBadPractice()
        {
            Console.WriteLine("Start preparing delicious breakfast");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Coffee cup = PourCoffee();
            Console.WriteLine("coffee is ready"); Console.WriteLine();

            Egg eggs = FryEggs(2);
            Console.WriteLine("eggs are ready"); Console.WriteLine();

            Bacon bacon = FryBacon(3);
            Console.WriteLine("bacon is ready"); Console.WriteLine();

            Toast toast = ToastBread(2);
            ApplyButter(toast);
            ApplyJam(toast);
            Console.WriteLine("toast is ready"); Console.WriteLine();

            Juice oj = PourOrangeJuice();
            Console.WriteLine("oj is ready"); Console.WriteLine();
            Console.WriteLine("Breakfast is ready!"); Console.WriteLine();

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine($"Time elapsed: {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds:00}");
        }

        private static Juice PourOrangeJuice()
        {
            Console.WriteLine("Pouring Orange Juice");
            return new Juice();
        }
        private static void ApplyJam(Toast toast) => Console.WriteLine("Putting jam on the toast");
        private static void ApplyButter(Toast tost) => Console.WriteLine("Putting butter on the toast");
        private static Toast ToastBread(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
                Task.Delay(500).Wait();
            }
            Console.WriteLine("Start toasting.......");
            Task.Delay(3000).Wait();
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }
        private static Bacon FryBacon(int slices)
        {
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            Task.Delay(3000).Wait();
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine($"flipping a {slice}th slice of bacon");
                Task.Delay(500).Wait();
            }
            Console.WriteLine("cooking the second side of bacon...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Put bacon on plate" );

            return new Bacon();
        }

        private static Egg FryEggs(int howMany)
        {
            Console.WriteLine("Warming the egg pan...");
            Task.Delay(3000).Wait();
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking eggs...");
            Task.Delay(3000).Wait();
            Console.WriteLine("Put eggs on plate");

            return new Egg();
        }

        private static async Task<Toast> ToastBreadAsync(int slices)
        {
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine("Putting a slice of bread in the toaster");
                await Task.Delay(500);
            }
            Console.WriteLine("Start toasting.......");
            await Task.Delay(3000);
            Console.WriteLine("Remove toast from toaster");

            return new Toast();
        }
        private static async Task<Bacon> FryBaconAsync(int slices)
        {
            Console.WriteLine($"putting {slices} slices of bacon in the pan");
            Console.WriteLine("cooking first side of bacon...");
            await Task.Delay(3000);
            for (int slice = 0; slice < slices; slice++)
            {
                Console.WriteLine($"flipping a {slice}th slice of bacon");
                await Task.Delay(500);
            }
            Console.WriteLine("cooking the second side of bacon...");
            await Task.Delay(3000);
            Console.WriteLine("Put bacon on plate");

            return new Bacon();
        }

        private static async Task<Egg> FryEggsAsync(int howMany)
        {
            Console.WriteLine("Warming the egg pan...");
            await Task.Delay(3000);
            Console.WriteLine($"cracking {howMany} eggs");
            Console.WriteLine("cooking eggs...");
            await Task.Delay(3000);
            Console.WriteLine("Put eggs on plate");

            return new Egg();
        }
        private static Coffee PourCoffee()
        {
            Console.WriteLine("Pouring coffee");
            return new Coffee();
        }
    }
    class Juice { }
    class Toast { }
    class Bacon { }
    class Egg { }
    class Coffee { }
}
