using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace PrimitiveReferenceAndValueTypes
{
    internal class SomeRef {
        public Int32 x;
    }

    internal struct SomeVal
    {
        public Int32 x;
    }
    internal interface IChangeBoxedPoint {
        void Change(Int32 x, Int32 y);
    }
    internal struct Point : IComparable, IChangeBoxedPoint {
        public Int32 x;
        public Int32 y;

        public void Change(Int32 a, Int32 b) {
            x = a;
            y = b;
        }
        public Point(Int32 a, Int32 b) {            //Конструктор
            x = a;
            y = b;
        }
        public override string ToString()           //Переопределённый ToString()
        {
            return String.Format("({0}, {1})", x, y); 
        }
        public Int32 CompareTo(Point other) {       //
            return Math.Sign(Math.Sqrt(x * x + y * y) - Math.Sqrt(other.x * other.x + other.y * other.y));
        }

        public int CompareTo(object o)              //
        {
            if (o.GetType() != GetType())
            {
                throw new ArgumentException("o is not a Point");
            }
            return CompareTo((Point)o);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
                        //Примитивные типы в языках программирования

            byte one = 1;
            short two = 2;
            ushort three = 3;
            int four = 4;
            uint five = 5;
            long six = 6;
            ulong seven = 7;
            char eigth = '8';
            float nine = 9.0f;      //System.Single <=> float
            double ten = 10.0;
            bool eleven = true;
            decimal twelve = 12.0M;
            string thirteen = "13";
            object fourteen = null;
            dynamic fifteen = null;
            //1-15 - примитивные типы

            //Разрешено неявное приведение примитивных типов, если это "безопасно"
            Int32 i = 5;
            Int64 l = i;
            Single s = i;
            Byte j = (Byte)i;
            Int16 v = (Int16)s;

            Single u = 6.8f;
            Int32 g = (Int32)u;     //g = 6 -> в C# дробная часть вседа отбрасывается

            Console.WriteLine(123.ToString() + 456.ToString());     //123456
            //Выражения, состоящие из литералов, вычисляются на этапе компиляции
            Int32 x = 100 + 20 + 3;     //В готовом коде х = 123


                        //Проверяемые и непроверяемые операции для примитивных типов

            Byte b = 100;
            b = (Byte)(200 + b);        //b = 44 
            //b = checked((Byte)(b + 250));   //OverflowException
            //По умолчанию не идет проверки на переполнение - оператор unchecked
            checked {
                byte r = 100;
                //r += 200;
            }


                        //Ссылочные и значимые типы

            SomeRef r1 = new SomeRef();
            SomeVal v1 = new SomeVal();
            r1.x = 5;                       //Разыменование указателя
            v1.x = 5;                       //Изменение в стеке
            SomeRef r2 = r1;
            SomeVal v2 = v1;
            v2.x = 3;
            r2.x = 6;
            Console.WriteLine(v1.x + " " + r1.x + " " + v2.x + " " + r2.x);     //5 6 3 6

            SomeVal v3 = new SomeVal();
            SomeVal v4;
            Int32 a1 = v3.x;        //нет ошибки, а1 == 0
                                    //Int32 a2 = v4.x;        ошибка, х - не инициализированно


            //Как CLR упарвояет размещением полей для типа

            /*
             [StructLayout(LayoutKind.Auto)]            //разрешаем CLR уcтановить порядок полей для этого типа самостоятельно.
            internal struct SomeValueType
            {
                //поля
            }

            [StructLayout(LayoutKind.Sequential)]       //не разрешаем CLR установить порядок полей для этого типа самостоятельно.
            internal struct SomeValueType2
            {
                //поля
            }
             */


                        //Упаковка и распаковка значимых типов

            ArrayList a = new ArrayList();
            Point p;
            for (Int32 k = 0; k < 10; k++) {
                p.x = p.y = k;
                a.Add(p);                       //Упаковка значимого типа и добавление ссылки в ArrayList
            }
            
            p = (Point)a[0];                    //Распаковка - переходим по указателю и копируем в стек

            Int32 x1 = 5;
            Object o = x1;
            Int16 y1 = (Int16)(Int32)o;         //Версия без каста к Int32 корректно работать не будет - InvalidCastException

            p = new Point();
            p.x = p.y = 1;
            o = p;                              //Упаковка
            p = (Point)o;                       //Распаковка
            p.x = 2;                            //Изменения значения
            o = p;                              //Упаковка
            //У Object нет поля х, поэтому, чтобы изменить значение мы должны произвести упаковку и распаковку

            Int32 val = 5;
            Object obj = val;                           //Упаковка
            v = 123;    
            Console.WriteLine(v + "," + (Int32)obj);    //Упаковка v для Concat, obj - распаковывается без копирования, а затем снова упаковывается

            Console.WriteLine(v);                       //Нет упаковки, т.к. существует перегрузка Console.Writeline(Int32 a);

            Point p1 = new Point(10, 10);
            Point p2 = new Point(20, 20);
            
            Console.WriteLine(p1.ToString());           //p1 не пакуется - ToString() - виртуальный метод

            Console.WriteLine(p1.CompareTo(p2));        //p1 и p2 не пакуются, т.к. существует перегрузка метода CompareTo(Point p) для Point

            IComparable c = p1;                         //p1 - пакуется, ссылка размещается в с
            Console.WriteLine(c.GetType());             //"Point"

            Console.WriteLine(p1.CompareTo(c));         //c не пакуется, вызывается CompareTo(object)

            Console.WriteLine(c.CompareTo(p2));         //p2 пакутеся, т.к. вызывается CompareTo(object)

            p2 = (Point)c;                              //Распаковка с, копирование в p2

            Console.WriteLine(p2.ToString());


                        //Изменение полей в упакованных значимых типах посредством интерфейсов (и почему этого лучше не делать)

            Point p3 = new Point(1, 1);

            Console.WriteLine(p3);                  //(1, 1)

            p3.Change(2, 2);
            Console.WriteLine(p3);                  //(2, 2)

            o = p3;
            Console.WriteLine(o);                   //(2, 2)

            ((Point)o).Change(3, 3);                //(2, 2)
            Console.WriteLine(o);

            ((IChangeBoxedPoint)p).Change(4, 4);    //(2, 2), GC при вовзрате из метода удалил упакованный Point
            Console.WriteLine(p);

            ((IChangeBoxedPoint)o).Change(5, 5);    //(5, 5), упаковки нет, интерфейс позволяет изменять упакованный объект
            Console.WriteLine(o);


                        //Равенство и тождество объектов

            //Стандартная реализация метода Equals() - проверка на тождество, а не на равенство
            //Для ValueType Equals атвоматически переопределён через отражения, что работает медленно!
            p = new Point(1, 1);
            p1 = new Point(1, 1);
            Console.WriteLine(p.Equals(p));
            Console.ReadKey();


                        //Хэш-коды объектов

            /*
             * С# не разрешает переопределить только один из методов GetHashCode(), Equals(). их всегда нужно переопределять в паре
             * Правила алгоритма вычисления Хэш-кодов:
             * 1. Случайное распределение
             * 2. Отказ от GHC Object и ValueType (они низкопроизводительны)
             * 3. В алгоритме должно использоваться минимум одно экземплярное поле
             * 4. Поля, используемые в алгоритме, в идеале, не должны изменяться
             * 5. Алгоритм должен быть максимально быстрым
             * 6. Объекты с одинаковыми значениями дожны возвращать одинаковый Хэш-код
            */


            //Примитивный тип данных dynamic
        }
    }
}
