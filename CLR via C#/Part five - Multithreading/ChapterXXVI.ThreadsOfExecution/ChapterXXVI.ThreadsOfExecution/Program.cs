using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * Процессом называется набор ресурсов, используемый отдельным экземпляром приложения
 * Экземпляр каждого приложения запускается в отдельном процессе
 * Каждому процессу выдается виртуальное адресное пространство - гарантия того, 
 что код и данные одного процесса будут не доступны для другого процесса
 * Каждому Windows-процессу выделяется собственный поток исполнения, который работает как виртуальный процессор
 */

/*
 * Каждый поток состоит из нескольких частей:
 * 1. Объект ядра потока
       * Для каждого созданного потока ОС выделяеи рдну из структур данных. Набор свойств этой структуры описывает поток.
 * 2. Блок окружения потока
       * Это место в памяти, выделенное и инициализированное в пользовательском режиме. Содержит локальное хранилище данных для потока и некоторые структуры данных,
    используемые интерфейсом графических устройств
 * 3. Стек пользовательского режима
       * Применяется для хранения передаваемых в методы локальных переменных и аргументов. Содержит адрес, показывающий, откуда продолжится выполнение, после того,
    как текущий метод возвратит управление. По умолчанию на каждый стек резервируется 1МБ памяти и добавляет физическую память по мере роста стека
 * 4. Стек режима ядра
       * Используется, когда приложение передает аргументы в функцию ОС, находящуюся в режиме ядра
 * 5. Уведомления о создании и завершении потока
 */

/*
 * Переключение контекста:
 * 1. Значения регистров процессора исполняющегося в данный момент потока, сохраняются в структуре контекста, которая располагается в ядре потока
 * 2. Из набора имеющихся потоков выбирается тот, которому будет передано управление. Если выбранный поток принадлежит другому процессу, 
 * происходит переключение для потока виртуального адресного пространства
 * 3. Значения из выбранной структуры контекста потока загружаются в регистры процессора
 * Это происходит примерно каждые 30мс. Выигрыша в производительности это не дает, а только обеспечивает надежность и быстрое реагирование ОС
 */

namespace ChapterXXVI.ThreadsOfExecution
{

    class Program
    {
        public static void ExampleThreading()
        {
            Console.WriteLine("Main thread: starting a dedicated thread " + "to do an asynchonous operation");
            Thread dedicatedThread = new Thread(ComputeBoundOp);
            dedicatedThread.Start(5);

            Console.WriteLine("Maint thread: Doing other work here...");
            dedicatedThread.Join();
            Console.WriteLine("Hit <Enter> to end this program...");
            Console.ReadLine();
            
        }
        private static void ComputeBoundOp(Object state) {
            Console.WriteLine("In ComputeBoundOp: state={0}", state);
            Thread.Sleep(1000);
            Console.WriteLine("In ComputeBoundOp: state={0}", state);
        }
        private static void BackgroundExample() {
            Thread t = new Thread(Worker);

            t.IsBackground = true;
            t.Start();

            Console.WriteLine("Return to Main");
        }
        private static void Worker() {
            Thread.Sleep(10000);

            Console.WriteLine("Returning from Worker");
        }
        static void Main(string[] args)
        {
            //ExampleThreading();

            BackgroundExample();
        }
    }
}
