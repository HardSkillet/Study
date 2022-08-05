using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChapterXXVIII.AsynchronousIO
{
    internal sealed class Example {
        internal static async Task<String> IssueClientRequestAsync(String serverName, String message) {
            using (var pipe = new NamedPipeClientStream(serverName, "PipeName", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough)) {
                pipe.Connect();
                pipe.ReadMode = PipeTransmissionMode.Message;

                Byte[] request = Encoding.UTF8.GetBytes(message);
                await pipe.WriteAsync(request, 0, request.Length);

                Byte[] response = new byte[1000];
                Int32 bytesRead = await pipe.ReadAsync(response, 0, response.Length);
                return Encoding.UTF8.GetString(response, 0, bytesRead);
            }
        }

        /*
         * 1. Метода Мain() приложения не может быть преобразован в асинхронную функцию
         * 2. Асинхронная функция не может иметь параметры ref и out
         * 3. Оператор await не может использоваться в блоке catch, finally или unsafe
         * 4. Не допускается установление блокировки, поддерживающей владение потоком или рекурсию, до операции await, и ее снаятие после оператора await
         * 5. В выражениях запросов оператор await может использоваться только в первом выражении коллекции условия from или в выражении коллекции условия join
         */
        
        public static async Task<String> MyMethodAsync(Int32 argument) {
            Int32 local = argument;
            try
            {
                Type1 result1 = await Method1Async();
                for (Int32 x = 0; x < 3; x++)
                {
                    Type2 result2 = await Method2Async();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Catch");
            }
            finally {
                Console.WriteLine("Finally");
            }
            return "Done";
        }
        private static async Task<Type2> Method2Async()
        {
            await new Task(() => Console.WriteLine("Method2Async"));
            return new Type2();
        }
        private static async Task<Type1> Method1Async()
        {
            await new Task(() => Console.WriteLine("Method1Async"));
            return new Type1();
        }
    }
    internal sealed class Type1 { }
    internal sealed class Type2 { }
    public static class TaskLogger
    {
        public enum TaskLogLevel { None, Pending }
        public static TaskLogLevel LogLevel { get; set; }

        public sealed class TaskLogEntry {
            public Task Task { get; internal set; }
            public DateTime LogTime { get; internal set; }
            public String Tag { get; internal set; }
            public String CallerMemberName { get; internal set; }
            public String CallerFilePath { get; internal set; }
            public Int32 CallerLineNumber { get; internal set; }
            public override String ToString()
            {
                return String.Format("LogTime={0}, Tag={1}, Member={2}, File={3}({4})",
                  LogTime, Tag ?? "(none)", CallerMemberName, CallerFilePath,
                  CallerLineNumber);
            }
        }

        private static readonly ConcurrentDictionary<Task, TaskLogEntry> s_log =
            new ConcurrentDictionary<Task, TaskLogEntry>();
        public static IEnumerable<TaskLogEntry> GetLogEntries() { return s_log.Values; }
        public static Task<TResult> Log<TResult>(this Task<TResult> task,
            String tag = null,
            [CallerMemberName] String callerMemberName = null,
            [CallerFilePath] String callerFilePath = null,
            [CallerLineNumber] Int32 callerLineNumber = 1)
        {
            return (Task<TResult>)
                Log((Task)task, tag, callerMemberName, callerFilePath, callerLineNumber);
        }
        public static Task Log(this Task task,
            String tag = null,
            [CallerMemberName] String callerMemberName = null,
            [CallerFilePath] String callerFilePath = null,
            [CallerLineNumber] Int32 callerLineNumber = 1)
        {
            if (LogLevel == TaskLogLevel.None) return task;
            var logEntry = new TaskLogEntry
            {
                Task = task,
                LogTime = DateTime.Now,
                Tag = tag,
                CallerMemberName = callerMemberName,
                CallerFilePath = callerFilePath,
                CallerLineNumber = callerLineNumber
            };
            s_log[task] = logEntry;
            task.ContinueWith(t =>
            {
                TaskLogEntry entry; s_log.TryRemove(t, out entry);
            }, TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }
        public static async Task Go()
        {
#if DEBUG
            TaskLogger.LogLevel = TaskLogger.TaskLogLevel.Pending;
#endif
            var tasks = new List<Task> {
                Task.Delay(2000).Log("2s op"),
                Task.Delay(5000).Log("5s op"),
                Task.Delay(6000).Log("6s op")
            };

            try {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException) { }
            foreach (var op in TaskLogger.GetLogEntries().OrderBy(tle => tle.LogTime))
                Console.WriteLine(op);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
