using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        static void Main(string[] args)
        {
            //ExampleAsync();
            ExampleContext();
        }
    }
}
