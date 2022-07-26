using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/*
 * Преимущества использования обобщений:
    * Защита исходного кода
    * Безопасность типов
    * Более простой и понятный код
    * Повышение производительности
 */


namespace Generic {
    internal sealed class DictionaryStringKey<TValue> : Dictionary<String, TValue> { }
    public static class Program {
        private static Object CreateInstance(Type t) {
            Object o = null;
            try
            {
                o = Activator.CreateInstance(t);
                Console.WriteLine("Created instance of {0}", t.ToString());
            }
            catch (ArgumentException e) {
                Console.WriteLine(e.Message);
            }
            return o;
        }
        internal sealed class Test<T>
        {
            static Test()
            {
                Console.WriteLine(typeof(T));
            }
        }

        public static void M() {
            Object o = null;
            
            Type t = typeof(Dictionary<,>);
            o = CreateInstance(t);
            Console.WriteLine();

            t = typeof(DictionaryStringKey<>);
            o = CreateInstance(t);
            Console.WriteLine();

            t = typeof(DictionaryStringKey<Guid>);
            o = CreateInstance(t);
            Console.WriteLine("Object type= + {0}", o.GetType());

            Console.WriteLine();

            var a = new Test<Int32>();
            var b = new Test<String>();
        }
        //Имена типов, выводимых программой, заканчиваются символом ', который азначает арность типа
        /*
         * Статические поля обобщенного типа отдельны для каждого закрытого типа (обобщенный тип является открытым и его экземпляр нельзя создать в CLR)
         * Статический конструктор обобщенного типа выполнится для каждого закрытого типа только один раз
         */
    }
}

namespace GenericTypesAndInheritance {
    internal sealed class Node<T> {
        public T m_data;
        public Node<T> m_next;

        public Node(T data) : this(data, null) { }
        public Node(T data, Node<T> next) {
            m_data = data; m_next = next;
        }

        public override string ToString()
        {
            return m_data.ToString() + ((m_next != null) ? m_next.ToString() : null);
        }
    }
    //С помощью класса Node<T> не получится создать список, хранящий значения разных типов (не используя System.Object). Поэтому лучше создать следующие 2 класса
    internal class Node
    {
        protected Node m_next;
        public Node(Node next) {
            m_next = next;
        }
    }

    internal sealed class TypedNode<T> : Node {
        public T m_data;

        public TypedNode(T data) : this(data, null) { }

        public TypedNode(T data, Node next) : base(next) {
            m_data = data;
        }
        public override string ToString()
        {
            return m_data.ToString() + ((m_next != null) ? m_next.ToString() : String.Empty);
        }
    }
    public static class Program {
        public static void Main() {
            Node<Char> head = new Node<Char>('C');
            head = new Node<Char>('B', head);
            head = new Node<Char>('A', head);
            Console.WriteLine(head.ToString());

            Node headTwo = new TypedNode<Char>('C');
            headTwo = new TypedNode<Int32>(21, headTwo);
            headTwo = new TypedNode<String>("Hi", headTwo);
            Console.WriteLine(headTwo.ToString());
        }
    }
}

//Можно использовать using для сокращения имен Generic-типов
//CLR создает машинный код для каждого сочетания "метод + тип" (в обобщенных типах). Для ссылочных типов создает только один раз и по одному разу для каждого ссылочного типа

namespace GenericDelegate  {
    
}

namespace ChapterXII.GenericTypes
{
    internal sealed class OperationTimer : IDisposable
    {
        private Stopwatch m_startTime;
        private String m_text;
        private Int32 m_collectionCount;

        public OperationTimer(String text) {
            PrepareForOperation();

            m_text = text;

            m_collectionCount = GC.CollectionCount(0);

            m_startTime = Stopwatch.StartNew();
        }

        private static void PrepareForOperation()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
        public void Dispose()
        {
            Console.WriteLine("{0} (GCs={1, 3}) {2}", (m_startTime.Elapsed), GC.CollectionCount(0), m_collectionCount, m_text);
        }
    }
    class Program
    {
        private static void ValueTypePerfTest() {

            const Int32 count = 1000000;

            using (new OperationTimer("List<Int32>"))
            {
                List<Int32> l = new List<Int32>();
                for (Int32 i = 0; i < count; i++)
                {
                    l.Add(i);
                    Int32 x = l[i];
                }
                l = null;
            }

            using (new OperationTimer("ArrayList of Int32"))
            {
                ArrayList a = new ArrayList();
                for (Int32 i = 0; i < count; i++)
                {
                    a.Add(i);
                    Int32 x = (Int32)a[i];
                }
                a = null;
            }
        }
        private static void ReferenceTypePerfTest() {

            const Int32 count = 1000000;

            using (new OperationTimer("List<String>"))
            {
                List<String> l = new List<String>();
                for (Int32 i = 0; i < count; i++)
                {
                    l.Add("X");
                    String x = l[i];
                }
                l = null;
            }

            using (new OperationTimer("ArrayList of String"))
            {
                ArrayList a = new ArrayList();
                for (Int32 i = 0; i < count; i++)
                {
                    a.Add("X");
                    String x = (String)a[i];
                }
                a = null;
            }
        }
        static void M(string[] args)
        {
            /* Тест производительности обобщенных типов
            ValueTypePerfTest();
            ReferenceTypePerfTest();
            */
        }
    }
}
