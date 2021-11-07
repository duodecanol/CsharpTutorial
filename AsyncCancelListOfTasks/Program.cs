using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq; // Language Integrated Query


// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/cancel-an-async-task-or-a-list-of-tasks
namespace AsyncCancelListOfTasks
{
    class Program
    {
        static readonly CancellationTokenSource s_cts = new CancellationTokenSource(); // async Request 취소용 토큰

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
        /// <summary>
        /// The entry point of this program.
        /// </summary>
        /// <param name="args"></param>        
        static async Task Main(string[] args)
        {
            //await ManualCancelAsync(); // Elapsed Time:                  00:00:04.0973786
            //await ScheduledTaskCancellationAsync();
            await SumPageSizesParallelAsync(); // Elapsed Time:            00:00:02.4514291
        }

        static async Task ScheduledTaskCancellationAsync()
        {
            Console.WriteLine("Application started.");
            try
            {
                s_cts.CancelAfter(3500); // 3.5초 뒤에 작업이 취소되도록 토큰을 설정
                
                await SumPageSizesAsync();
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("\nTasks cancelled: timed out.\n");
            }
            finally
            {
                s_cts.Dispose();
            }
            Console.WriteLine("Application ending.");
        }

        static async Task ManualCancelAsync()
        {
            Console.WriteLine("Application started.");
            Console.WriteLine("Press the ENTER key to cancel ....." + Environment.NewLine);

            Task cancelTask = Task.Run(() => { // Task instance named cancelTask, which will read console key strokes.
                while (Console.ReadKey().Key != ConsoleKey.Enter)
                {   // If the Enter key is pressed, a call to CancellationTokenSource.Cancel() is made. This will signal cancellation. 
                    Console.WriteLine("Press the ENTER key to cancel...");
                }
                Console.WriteLine(Environment.NewLine + "ENTER key pressed: cancelling downloads" + Environment.NewLine); // ENTER 키가 눌리면 작업을 취소한다.
                s_cts.Cancel();
            });

            Task sumPageSizesTask = SumPageSizesAsync(); // 위 작업을 띄워놓고 한편으로는 페이지 크기 출력 작업을 시작한다.

            await Task.WhenAny(new[] { cancelTask, sumPageSizesTask }); // 둘 중 한 작업이라도 종료되면 애플리케이션이 종료되는 것.

            Console.WriteLine("Application ending.");
        }

        static async Task SumPageSizesAsync()
        {
            var stopwatch = Stopwatch.StartNew(); // 작업시간 측정을 위한 시계

            int total = 0;
            foreach (string url in s_urlList)
            {
                int contentLength = await ProcessUrlAsync(url, s_client, s_cts.Token); // 클래스의 URL 리스트에서 하나씩 페이지 사이즈 계산 프로세스를 돌린다.
                total += contentLength; // 계산된 일 페이지 값은 총합에 sum한다.
            }

            stopwatch.Stop(); // stopwatch 종료

            Console.WriteLine(Environment.NewLine + $"Total bytes returned: {total:#,#}");  // 최종 메시지를 출력한다.
            Console.WriteLine($"Elapsed Time:                  {stopwatch.Elapsed}" + Environment.NewLine);
        }
        static async Task SumPageSizesParallelAsync() // Process asynchronous tasks as they complete
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/start-multiple-async-tasks-and-process-them-as-they-complete
        {
            var stopwatch = Stopwatch.StartNew();
            
            IEnumerable<Task<int>> downloadTasksQuery = 
                from url in s_urlList
                select ProcessUrlAsync(url, s_client); // C# LINQ : https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/keywords/select-clause
            
            // Due to deferred execution with the LINQ, you call Enumerable.ToList to start each task.
            // See more about "deferred execution" : https://docs.microsoft.com/en-us/dotnet/standard/linq/deferred-execution-example
            List<Task<int>> downloadTasks = downloadTasksQuery.ToList();

            int total = 0;
            while (downloadTasks.Any())
            {
                //Awaits a call to WhenAny to identify the first task in the collection that has finished its download.
                Task<int> finishedTask = await Task.WhenAny(downloadTasks);
                downloadTasks.Remove(finishedTask); // Removes finished task from the collection.
                total += await finishedTask;
            }

            stopwatch.Stop();

            Console.WriteLine(Environment.NewLine + $"Total bytes returned: {total:#,#}");  // 최종 메시지를 출력한다.
            Console.WriteLine($"Elapsed Time:                  {stopwatch.Elapsed}" + Environment.NewLine);
        }
        static async Task<int> ProcessUrlAsync(string url, HttpClient client, CancellationToken token)
        {
            HttpResponseMessage response = await client.GetAsync(url, token); // URI와 취소용 토큰을 집어넣는다.
            byte[] content = await response.Content.ReadAsByteArrayAsync(); // No token required current in current version?
            Console.WriteLine($"{url,-60}  {content.Length,10:#,#}");
            return content.Length;
        }
        static async Task<int> ProcessUrlAsync(string url, HttpClient client)
        {
            byte[] content = await client.GetByteArrayAsync(url);
            Console.WriteLine($"{url,-60}  {content.Length,10:#,#}");
            return content.Length;
        }

    }
}
