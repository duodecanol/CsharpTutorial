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
            //Console.WriteLine("Hello World!");

            //string[] myargs = { @"c:\windows\", @"C:\Users\" };
            //DirectorySizeExample.MainFunc( myargs );

            //////////////////////////////////////////////////////////////

            //MultiplyMatrices.MainSub();

            //SimpleParallelForEachLoop.MainEntry();

            //ParallelForEachWithThreadLocalVar.MainEntry();

            //CancelParallelLoops.MainEntry();

            ParallelExceptionHandleDemo.MainEntry();
        }
    }

    
}
