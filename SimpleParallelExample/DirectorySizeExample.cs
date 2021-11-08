using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

/**
 * This example is a simple command-line utility that calculates the total size of files in a directory. 
 * It expects a single directory path as an argument, and reports the number and total size of the files in that directory. 
 * After verifying that the directory exists, it uses the Parallel.For method to enumerate the files in the directory and determine their file sizes. 
 * Each file size is then added to the totalSize variable. Note that the addition is performed by calling the Interlocked.Add so that 
 * the addition is performed as an atomic operation. Otherwise, multiple tasks could try to update the totalSize variable simultaneously.
 *  https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-write-a-simple-parallel-for-loop#directory-size-example
 */
namespace SimpleParallelExample
{
    public class DirectorySizeExample
    {
        public static void MainFunc(string[] args)
        {
            long totalSize = 0;

            if (args.Length == 0)
            {
                Console.WriteLine("There are no command line arguments.");
                return;
            }
            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("The directory does not exist.");
                return;
            }

            String[] files = Directory.GetFiles(args[0]);
            Parallel.For(fromInclusive: 0,
                        toExclusive: files.Length,
                        index => {
                            FileInfo fi = new FileInfo(files[index]);
                            long size = fi.Length;
                            Interlocked.Add(ref totalSize, size); // the new value stored at location1:totalSize
                        });
            Console.WriteLine("Directory '{0}'", args[0]);
            Console.WriteLine($"{files.Length:N0} files, {totalSize:N0} bytes");
        }
    }
}
