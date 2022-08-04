using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
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
    class Program
    {
        static void Main(string[] args)
        {
            var a = KekAsync();
            Console.WriteLine("adasdasd");
            for (int i = 0; i < 100000; i++) { Console.WriteLine("kek"); Thread.Sleep(100); }
            Console.ReadLine();
        }
        public static async Task KekAsync() {
            for (Int32 i = 0; i < 10; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(100);
            }
            await Task.Run(() => { 
                Console.WriteLine("Method1Async");
                Thread.Sleep(1000);
                Console.WriteLine("Method1Async");
            });
            for(Int32 i = 0; i < 10; i++)
            {
                Console.WriteLine(i+10);
                Thread.Sleep(100);
            }
        }
    }
}
