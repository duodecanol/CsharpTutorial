using System;
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
    }

    #endregion
        class CustomData
        {
            public long CreationTime;
            public int Name;
            public int ThreadNum;
        }
}
