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

namespace GenericDelegate
{
    /*
     * Каждый из параметров-типов обобщщенного делегата должен быть помечен как ковариантный или контрвариантный
     * 1. Инвариантный - параметр-тип не может изменяться
     * 2. Контрвариантный - параметр-тип может быть преобразован к классу, производному от него. Помечается ключевым словом in. Появляется только во входной позиции, например, в качестве аргументов метода
     * 4. Ковариантный - параметр-тип может быть преобразован к базовому классу. Помечается ключевым словом out. Появляется только в выходной позиции, например, в качестве возвращаемого значения метода
     */
    public sealed partial class SomeClass
    {
        public delegate TResult Func<in T, out TResult>(T arg);     //Нельзя delegate TResult Func<out T, in TResult>(T arg);
        public static void M()
        {
            Func<Object, ArgumentException> fn1 = null;
            Func<String, Exception> fn2 = fn1;                      //Привидение допустимо
        }
    }

    //Параметры-типа интерфейса так же могут быть ковариантными или контрвариантными

    public sealed partial class SomeClass
    {
        public interface IEnumerator<out T> : IEnumerator
        {
            Boolean MoveNext();
            T Current { get; }
        }
    }
}

namespace GenericMethods {
    internal sealed class GenericType<T> {
        private T m_value;

        public GenericType(T value) {
            m_value = value;
        }

        public TOutput Converter<TOutput>() {                                           //У метода определен свой параметр тип TOutput
            TOutput result = (TOutput)Convert.ChangeType(m_value, typeof(TOutput));
            return result;
        }
    }
    internal static class SomeClass {
        internal static void Swap<T>(ref T o1, ref T o2) {
            T t = o1;
            o1 = o2;
            o2 = t;
        }
        private static void CallingSwapUsingInference() {
            Int32 n1 = 1, n2 = 2;
            Swap(ref n1, ref n2);   //Swap<Int32> - писать необязательно, компилятор смог установить, что переменный принадлежат одному типу Int32

            String s1 = "Aidan";
            Object s2 = "Grant";
            //Swap(ref s1, ref s2); - Ошибка, невозможно вывести тип, т.к. компилятор использует тип данных переменной
        }
    }
}

/*
 * В C# собственных параметров-типов не может быть у свойств, индексаторов, событий, операторных методов, конструкторов и деструкторов 
 */

namespace VerificationAndRestrictions{
    internal sealed class SomeClass {
        private static Boolean MethodTakingAnyType<T>(T o) {
            T temp = o;
            Console.WriteLine(o.ToString());
            Boolean b = temp.Equals(o);
            return b;
        }
        private static T Min<T>(T o1, T o2) where T : IComparable<T> {       //Если не вводить ограничение - ошибка, т.к. не для всех типов опеределен метод CompareTo()
            if (o1.CompareTo(o2) < 0) return o1;
            return o2;
        }
    }
    /*
     * CLR не поддерживает перегрузка по имени параметра типа или по именам ограничений
     * Так же переопределяемый виртуальный метод не может менять ограничения, а может только изменять имена параметров типов
     * СLR позволяет определять 3 типа ограничений:
        * Основное
        * Дополнительное
        * Ограничение конструктора
     */

    /*
     * Основным ограничением может быть ссылочный тип, указывающий на запечатанный класс (кроме Object, Delegate, ValueType, Enum, Void)
     * При задании основного ограничения гарантируется, что указанный тип будет относиться либо к заданному типу, либо к производному от него
     * Если основное ограничение на параметр-тип не задано, по умолчанию ставится Object (однако самоустоятельное указание Object приведет к ошибке)
     * Два особых основных ограничения:
        * class - гарантирует, что тип будет ссылочным. Ему удовлетворяют все ссылочные типы, делегаты, интерфейсы, массивы и тп
        * struct - все значимые типы и перечисления
     */

    /*
     * Дополнительных ограничений может быть ноль или несколько
     * Ограничение интерфейсного типа гарантирует, что параметр типа будет реализовывать интерфейс. (Таких ограничений может быть несколько)
     * Ограничение параметра-типа позволяет указать обобщенному методу или типу, что аргументы типа должны быть связаны определенным соотношением. (Таких ограничений может быть несколько)
     */

    internal sealed class AdditionalRestrictions {
        private static List<TBase> Convert<T, TBase>(IList<T> list)
        where T : TBase                                                 //Пример ограничения параметра-типа
        {
            List<TBase> baseList = new List<TBase>(list.Count);
            for (Int32 index = 0; index < list.Count; index++) {
                baseList.Add(list[index]);
            }
            return baseList;
        }
    }

    /*
     * Для параметра-типа можно задать не более одного ограничения конструктора
     * Ограничение конструктора гарантирует, что данный тип будет неабстарктным и иметь открытый конструктор без параметров
     * Ограничение struct и ограничение конструктора считается избыточным
     */
    internal sealed class ConstructorContraint<T> where T : new() {
        public static T Factory() {
            return new T();
        }
    }
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
