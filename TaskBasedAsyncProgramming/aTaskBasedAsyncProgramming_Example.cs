using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace TaskBasedAsyncProgramming
{
    // https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming
    public class aTaskBasedAsyncProgramming_Example
    {
        public static void BasicTaskThreadExample()
        {
            Thread.CurrentThread.Name = "Main";

            // Create a task and supply a user delegate by using a lambda expression
            Task taskA = new Task(() => Console.WriteLine("Hello from taskA"));
            // Start the task
            taskA.Start();

            // Define and run the task
            Task taskB = Task.Run( () => Console.WriteLine("Hello from taskB"));

            // Output a message from the calling thread.
            Console.WriteLine("Hello from thread '{0}'.", Thread.CurrentThread.Name);
            taskA.Wait();
            taskB.Wait();
            // Note that the example includes a call to the Task.Wait method
            // to ensure that the task completes execution before the console mode application ends.
        }

        #region Taks Factory Void Example Problem and solution
        public static void TaskFactoryVoidExample()
        {
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew(
                    (Object obj) =>
                    {
                        CustomData data = obj as CustomData;
                        if (data == null)
                            return;
                        data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    },
                    new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks }
                );
            }
            Task.WaitAll(taskArray);
            foreach (var task in taskArray)
            {
                var data = task.AsyncState as CustomData; // passed argument can be accessed using AsyncState
                if (data != null)
                    Console.WriteLine($"Task #{data.Name} created at {data.CreationTime}, ran on thread #{data.ThreadNum}");
            }
        }

        public static void TaskFactoryVoidExampleProblem()
        {
            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew(
                    (Object obj) =>
                    {
                        var data = new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks };
                        data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                        Console.WriteLine($"Task #{data.Name} created at {data.CreationTime}, ran on thread #{data.ThreadNum}");
                    },  // Task Names are all #10????
                    i
                );
            }
            Task.WaitAll(taskArray);
        }

        public static void TaskFactoryVoidExampleProblemSolved()
        {
            // Create the task object by using an Action(Of Object) to pass in custom data
            // to the Task constructor. This is useful when you need to capture outer variables
            // from within a loop.

            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew(
                    (Object obj) => // <======= This lambda expression
                    {
                        CustomData data = obj as CustomData;
                        if (data == null)
                            return;
                        data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                        Console.WriteLine($"Task #{data.Name} created at {data.CreationTime}, ran on thread #{data.ThreadNum}");
                    },  // See closely how the problem was solved.
                    new CustomData() { Name = i, CreationTime = DateTime.Now.Ticks } // This goes into ==>
                );
            }
            Task.WaitAll(taskArray);
        }
        #endregion

#region Task Factory with returning results
        public static void TaskFactoryTResultExample()
        {
            Task<Double>[] taskArray = {
                Task<Double>.Factory.StartNew(() => DoComputation(1.0)),
                Task<Double>.Factory.StartNew(() => DoComputation(100.0)),
                Task<Double>.Factory.StartNew(() => DoComputation(1000.0))
            };

            var results = new Double[taskArray.Length];
            Double sum = 0;

            for (int i = 0; i < taskArray.Length; i++)
            {
                results[i] = taskArray[i].Result;
                Console.Write("{0:N1} {1}", results[i], i == taskArray.Length - 1 ? "= " : "+ ");
                sum += results[i];
            }
            Console.WriteLine("{0:N1}", sum);
        }
        private static Double DoComputation(Double start)
        {
            Double sum = 0;
            for (var value = start; value <= start + 10; value += .1)
                sum += value;
            return sum;
        }

        #region Task, threads, and culture
        public static void ThreadCultureExample()
        {
            decimal[] values = { 163025412.32m, 18905365.59m };
            string formatString = "C2"; // with currency symbol, 2 digits after the decimal point
            Func<String> formatDelegate = () =>
            {
                string output = String.Format("Formatting using the {0} culture on thread {1}.\n",
                    CultureInfo.CurrentCulture.Name,
                    Thread.CurrentThread.ManagedThreadId);

                foreach (var value in values)
                    output += String.Format("{0}    ", value.ToString(formatString)); // "{value:C2}"

                output += Environment.NewLine;
                return output;
            };

            Console.WriteLine("The example is running on thread {0}", Thread.CurrentThread.ManagedThreadId);
            // Make the current culture different from the system culture.
            Console.WriteLine("The current culture is {0}", CultureInfo.CurrentCulture.Name);
            if (CultureInfo.CurrentCulture.Name == "ko-KR")
                Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
                //Thread.CurrentThread.CurrentCulture = new CultureInfo("ja-JP");
            else
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

            Console.WriteLine("Changed the current culture is {0}\n", CultureInfo.CurrentCulture.Name);

            // Execute the delegate synchronously.
            Console.WriteLine("Executing the delegate synchronously: ");
            Console.WriteLine(formatDelegate());

            // Call an async delegate to format the values using one format string.
            Console.WriteLine("Executing a task asynchronously: ");
            var t1 = Task.Run(formatDelegate);
            Console.WriteLine(t1.Result);

            Console.WriteLine("Executing a task synchronously: ");
            var t2 = new Task<String>(formatDelegate);
            t2.RunSynchronously();
            Console.WriteLine(t2.Result);
        }
        // In versions of .NET Framework prior to .NET Framework 4.6,
        // a task's culture is determined by the culture of the thread on which it runs,
        // not the culture of the calling thread. For asynchronous tasks,
        // this means the culture used by the task could be different to the calling thread's culture.
        #endregion
    }

    #endregion
    class CustomData
        {
            public long CreationTime;
            public int Name;
            public int ThreadNum;
        }
}
