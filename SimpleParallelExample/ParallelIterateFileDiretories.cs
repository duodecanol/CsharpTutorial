using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleParallelExample
{
    class ParallelIterateFileDiretories
    {
        public static void MainEntry()
        {
            try
            {
                TraverseTreeParallelForEach(@"C:\Program Files", (f) =>
                {
                    try // Exceptions are no-ops.
                    {
                        byte[] data = File.ReadAllBytes(f); // Do nothing with the data except read it.
                    }
                    catch (FileNotFoundException) { }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                    catch (SecurityException) { }                    
                    Console.WriteLine(f); // Display the filename.
                });
            }
            catch (ArgumentException)
            {
                Console.WriteLine(@"The directory 'C:\Program Files' does not exist.");
            }

            Console.ReadKey(); // Keep the console window open.
        }
        public static void TraverseTreeParallelForEach(string root, Action<string> action)
        {
            // Count of files traversed and timer for diagnostic output
            int fileCount = 0;
            Stopwatch sw = Stopwatch.StartNew();

            // Determine wheter to parallelize file processing on each folder based on processor count
            int proCount = System.Environment.ProcessorCount;

            // Data structure to hold names of subfolders to be examined for files
            Stack<string> dirs = new Stack<string>();

            if (!Directory.Exists(root))
                throw new ArgumentException();
            dirs.Push(root);

            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();
                string[] subDirs = { };
                string[] files = { };

                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                catch (UnauthorizedAccessException e)
                { // Thrown if we do not have discovery permission on the directory
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                { // Thrown if another process has deleted the directory after we retrieved its name
                    Console.WriteLine(e.Message);
                    continue;
                }

                try
                {
                    files = Directory.GetFiles(currentDir);
                } 
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                // Execute in parallel if there are enough files in the directory
                // Otherwise, execute sequentially. Files are opened and processed
                // synchronously but this could be modified to perform async I/O.
                try
                {
                    if (files.Length < proCount)
                    {
                        foreach (var file in files)
                        {
                            action(file);
                            fileCount++;
                        }
                    }
                    else // if file length is greater than process count
                    {
                        Parallel.ForEach(
                            files,
                            () => 0,
                            (file, loopState, localCount) =>
                            {
                                action(file);
                                return (int)++localCount;
                            },
                            (c) => Interlocked.Add(ref fileCount, c)
                            );
                    }
                }
                catch (AggregateException ae)
                {
                    ae.Handle((ex) =>
                    {
                        if (ex is UnauthorizedAccessException)
                        {
                            Console.WriteLine(ex.Message);
                            return true;
                        }
                        // Handle other exceptions here if necessary
                        return false;
                    });
                }

                // Push the subdirectories onto the stack for traversal.
                // This could also be done before handling the files.
                foreach (string str in subDirs)
                    dirs.Push(str);
            }
            // For diagnostic purpose.
            Console.WriteLine($"Processed {fileCount} files in {sw.ElapsedMilliseconds} milliseconds");
        }
    }
}
