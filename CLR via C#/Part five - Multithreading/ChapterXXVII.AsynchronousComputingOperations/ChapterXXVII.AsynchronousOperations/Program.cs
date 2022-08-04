using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapterXXVII.AsynchronousOperations
{
    public sealed class Example
    {
        private static void ExampleAsync() {
            Console.WriteLine("Main thread: queuing an asynchronous operation");
            ThreadPool.QueueUserWorkItem(ComputeBoundOp, 12.5d);
            Console.WriteLine("Main thread: Doing other work here...");
            Thread.Sleep(10000);
            Console.WriteLine("Hit <Enter> to end this program");
            Console.ReadLine();
        }
        private static void ComputeBoundOp(Object state) {
            Console.WriteLine("In ComputeBoundOp: state={0}", state);
            Thread.Sleep(1000);
        }
        private static void ExampleContext() {
            CallContext.LogicalSetData("Name", "Nikita");

            ThreadPool.QueueUserWorkItem(
                state => Console.WriteLine("Name={0}",
                CallContext.LogicalGetData("Name")));
            ExecutionContext.SuppressFlow();            //Запрещаем использование контекста
            ThreadPool.QueueUserWorkItem(
                state => Console.WriteLine("Name={0}",
                CallContext.LogicalGetData("Name")));
            ExecutionContext.RestoreFlow();             //Разрещаем использование контекста
            Console.ReadLine();
        }
        internal static class CancellationDemo
        {
            public static void Demo()
            {
                CancellationTokenSource cts = new CancellationTokenSource();

                ThreadPool.QueueUserWorkItem(o => Count(cts.Token, 1000));
                Console.WriteLine("Press <Enter> to cancel the operation");
                Console.ReadLine();
                cts.Cancel();

                Console.ReadLine();
            }
            private static void Count(CancellationToken token, Int32 countTo)
            {
                for (Int32 count = 0; count < countTo; count++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("Count is cancelled");
                        break;
                    }
                    Console.WriteLine(count);
                    Thread.Sleep(20);
                }
                Console.WriteLine("Count is done");
            }
            public static void CallbackExample()
            {
                var cts = new CancellationTokenSource();
                cts.Token.Register(() => Console.WriteLine("First"));
                cts.Token.Register(() => Console.WriteLine("Second"));

                cts.Cancel();
                Console.ReadLine();
            }
            public static void LinkedExample()
            {
                var cts1 = new CancellationTokenSource();
                var cts2 = new CancellationTokenSource();

                cts1.Token.Register(() => Console.WriteLine("cts1 canceled"));
                cts2.Token.Register(() => Console.WriteLine("cts2 canceled"));

                var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
                linkedCts.Token.Register(() => Console.WriteLine("linkedCts canceled"));

                cts2.Cancel();

                Console.WriteLine("cts1 canceled={0}, cts2 canceled={1}, linkedCts canceled={2}",
                    cts1.IsCancellationRequested, cts2.IsCancellationRequested, linkedCts.IsCancellationRequested);

                Console.ReadLine();
            }
        }
        internal static class TaskDemo
        {
            public static void Test(Int32 num)
            {
                var cts = new CancellationTokenSource();
                Task<Int32> t = Task.Run(() => Sum(CancellationToken.None, 1000));

                Task cwt = t.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result));
                cwt.ContinueWith(task => Console.WriteLine("#"));

                Console.ReadLine();
            }
            public static void ParentTask() {
                var cts = new CancellationTokenSource();
                Task<Int32[]> parent = new Task<int[]>(() =>
                {
                    var results = new Int32[3];

                    new Task(() => results[0] = Sum(cts.Token, 10000), TaskCreationOptions.AttachedToParent).Start();
                    new Task(() => results[1] = Sum(cts.Token, 20000), TaskCreationOptions.AttachedToParent).Start();
                    new Task(() => results[2] = Sum(cts.Token, 30000), TaskCreationOptions.AttachedToParent).Start();

                    return results;
                });
                var cwt = parent.ContinueWith(
                    parentTask => Array.ForEach(parentTask.Result, Console.WriteLine));
                parent.Start();
                Console.ReadLine();
            }
            public static Int32 Sum(CancellationToken ct, Int32 n) {
                Int32 sum = 0;
                for (; n > 0; n--) {
                    ct.ThrowIfCancellationRequested();    
                    checked { sum += n; } 
                }
                return sum;
            }
            public static void TestFactory() {
                Task parent = new Task(() =>
                {
                    var cts = new CancellationTokenSource();
                    var tf = new TaskFactory<Int32>(cts.Token,
                        TaskCreationOptions.AttachedToParent,
                        TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);

                    var childTasks = new[] {
                        tf.StartNew(() => Sum(cts.Token, 100000000)),
                        tf.StartNew(() => Sum(cts.Token, 20000)),
                        tf.StartNew(() => Sum(cts.Token, Int32.MaxValue))
                    };

                    for (Int32 task = 0; task < childTasks.Length; task++)
                    {
                        childTasks[task].ContinueWith(
                            t => cts.Cancel(), TaskContinuationOptions.OnlyOnFaulted);
                    }

                    tf.ContinueWhenAll(
                        childTasks,
                        completedTasks => completedTasks.Where(
                            t => !t.IsFaulted && !t.IsCanceled).Max(t => t.Result),
                        CancellationToken.None)
                    .ContinueWith(t => Console.WriteLine("The maximum is: " + t.Result),
                    TaskContinuationOptions.ExecuteSynchronously);
                });
                parent.ContinueWith(p => {
                    StringBuilder sb = new StringBuilder("The folloewing exception(s) occurred:" + Environment.NewLine);
                    foreach (var e in p.Exception.Flatten().InnerExceptions) {
                        sb.AppendLine(" " + e.GetType().ToString());
                    }
                    Console.WriteLine(sb.ToString());
                }, TaskContinuationOptions.OnlyOnFaulted);

                parent.Start();
                Console.ReadLine();
            }
        }
        internal sealed class MyForm : Form {
            private readonly TaskScheduler m_syncContextTaskScheduler;
            public MyForm() {
                m_syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

                Text = "Synchronization Context Task Scheduler Demo";
                Visible = true; Width = 600; Height = 100;
            }

            private CancellationTokenSource m_cts;
            protected override void OnMouseClick(MouseEventArgs e)
            {
                if (m_cts != null)
                {
                    m_cts.Cancel();
                    m_cts = null;
                }
                else {
                    Text = "Operation running";
                    m_cts = new CancellationTokenSource();

                    Task<Int32> t = Task.Run(() => TaskDemo.Sum(m_cts.Token, 20000), m_cts.Token);
                    t.ContinueWith(task => Text = "Result: " + task.Result,
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        m_syncContextTaskScheduler);
                    t.ContinueWith(task => Text = "Operation canceled",
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnCanceled,
                        m_syncContextTaskScheduler);
                    t.ContinueWith(task => Text = "Operation faulted",
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnFaulted,
                        m_syncContextTaskScheduler);
                }
                base.OnMouseClick(e);
            }
        }
        internal sealed class ParallelDemo {
            public static Int64 DirectoryBytes(String path, String searchPattern,
                SearchOption searchOption)
            {
                var files = Directory.EnumerateFiles(path, searchPattern, searchOption);
                Int64 masterTotal = 0;

                ParallelLoopResult result = Parallel.ForEach<String, Int64>(
                    files,
                    () =>
                    {
                        return 0;
                    },
                    (file, ParallelLoopState, index, taskLocalTotal) =>
                    {
                        Int64 fileLength = 0;
                        FileStream fs = null;
                        try
                        {
                            fs = File.OpenRead(file);
                            fileLength = fs.Length;
                        }
                        catch (IOException) { }
                        finally { if (fs != null) fs.Dispose(); }
                        return taskLocalTotal + fileLength;
                    },
                    taskLocalTotal =>
                    {
                        Interlocked.Add(ref masterTotal, taskLocalTotal);
                    });

                return masterTotal;
            }
        }
        internal static class DelayDemo {
            public static void Demo() {
                Console.WriteLine("Checking status every 2 seconds");
                Status();
                Console.ReadLine();
            }
            private static async void Status() {
                while (true) {
                    Console.WriteLine("Checking status at {0}", DateTime.Now);

                    await Task.Delay(2000);
                }   
            }
        }
        static void Main(string[] args)
        {
            //ExampleAsync();
            //ExampleContext();
            //CancellationDemo.Demo();
            //CancellationDemo.CallbackExample();
            //CancellationDemo.LinkedExample();
            //TaskDemo.Test(10123);
            //TaskDemo.ParentTask();
            //TaskDemo.TestFactory();
            DelayDemo.Demo();
        }
    }
}
