using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleParallelExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string[] myargs = { @"c:\windows\", @"C:\Users\" };
            DirectorySizeExample.MainFunc( myargs );
        }
    }

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
                        index => { FileInfo fi = new FileInfo(files[index]);
                            long size = fi.Length;
                            Interlocked.Add(ref totalSize, size); // the new value stored at location1:totalSize
                        } );
            Console.WriteLine("Directory '{0}'", args[0]);
            Console.WriteLine($"{files.Length:N0} files, {totalSize:N0} bytes");
        }
    }
}
