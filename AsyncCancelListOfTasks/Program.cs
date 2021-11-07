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
        static void Main(string[] args)
        {
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
    }
}
