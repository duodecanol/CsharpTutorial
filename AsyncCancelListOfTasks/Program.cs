using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/cancel-an-async-task-or-a-list-of-tasks
namespace AsyncCancelListOfTasks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Application started.");
            Console.WriteLine("Press the ENTER key to cancel ....." + Environment.NewLine);

            Task cancelTask = Task.Run( ()=> { // Task instance named cancelTask, which will read console key strokes.
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {   // If the Enter key is pressed, a call to CancellationTokenSource.Cancel() is made. This will signal cancellation. 
                    Console.WriteLine("Press the ENTER key to cancel...");
                }
                Console.WriteLine(Environment.NewLine + "ENTER key pressed: cancelling downloads" + Environment.NewLine);
                s_cts.Cancel();
            });

            Task sumPageSizesTask = SumPageSizesTask();

            await Task.WhenAny(new[] { cancelTask, sumPageSizesTask });

            Console.WriteLine("Application ending.");
        }

        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();

        static readonly HttpClient s_client = new HttpClient
        {
            MaxResponseContentBufferSize = 1_000_000
        };
        static readonly IEnumerable<string> s_urlList = new string[]
        {
                "https://docs.microsoft.com",
                "https://docs.microsoft.com/aspnet/core",
                "https://docs.microsoft.com/azure",
                "https://docs.microsoft.com/azure/devops",
                "https://docs.microsoft.com/dotnet",
                "https://docs.microsoft.com/dynamics365",
                "https://docs.microsoft.com/education",
                "https://docs.microsoft.com/enterprise-mobility-security",
                "https://docs.microsoft.com/gaming",
                "https://docs.microsoft.com/graph",
                "https://docs.microsoft.com/microsoft-365",
                "https://docs.microsoft.com/office",
                "https://docs.microsoft.com/powershell",
                "https://docs.microsoft.com/sql",
                "https://docs.microsoft.com/surface",
                "https://docs.microsoft.com/system-center",
                "https://docs.microsoft.com/visualstudio",
                "https://docs.microsoft.com/windows",
                "https://docs.microsoft.com/xamarin"
        };
        static async Task SumPageSizesTask()
        {
            var stopwatch = Stopwatch.StartNew();

            int total = 0;
            foreach (string url in s_urlList)
            {
                int contentLength = await ProcessUrlAsync(url, s_client, s_cts.Token);
                total += contentLength;
            }

            stopwatch.Stop();

            Console.WriteLine(Environment.NewLine + $"Total bytes returned: {total:#,#}");
            Console.WriteLine($"Elapsed Time:                  {stopwatch.Elapsed}" + Environment.NewLine);
        }
        static async Task<int> ProcessUrlAsync(string url, HttpClient client, CancellationToken token)
        {
            HttpResponseMessage response = await client.GetAsync(url, token);
            byte[] content = await response.Content.ReadAsByteArrayAsync(token);
            Console.WriteLine($"{url,-60}  {content.Length,10:#,#}");
            return content.Length;
        }
    }
}
