using System;
using System.Collections.Generic;
using System.Text;

namespace InstanceConstructorsAndClasses
{
    //Конструкторы экземпляров не наследуются, в отличие от других методов
    //Для абстрактных классов компилятор создаёт конструктор по умолчанию с модификатором protected
    internal sealed class SomeType {
        private Int32 m_x;
        private String m_s;
        private Double m_d;
        private Byte m_b;

        public SomeType() {
            m_x = 5;
            m_s = "Hi there";
            m_d = 3.14591;
            m_b = 0xff;
        }
        public SomeType(Int32 x) : this() {             //Вызаывает базовые конструктор, инициализирующий поля по умолчанию, а затем изменяет поле m_x
            m_x = x;
        }

        public SomeType(String s) : this() {            //Вызаывает базовые конструктор, инициализирующий поля по умолчанию, а затем изменяет поле m_s
            m_s = s;
        }

        public SomeType(Int32 x, String s) : this() {   //Вызаывает базовые конструктор, инициализирующий поля по умолчанию, а затем изменяет поля m_x и m_s
            m_x = x;
            m_s = s;
        }
    }
}
namespace InstanceConstructorsAndStructures {
    //C# не определяет для значимых типов конструкторы по умолчанию
    //Конструкторы экземпляра значимого типа выполняются только при явном вызове
    //С# не позволяет создать конструктор без параметров для экземпляров значимых типов (допускается для типа), а так же инициализацию экземплярных полей
    //Любой конструктор значимого типа должен инициализировать все поля
    public struct SomeType {
        private Int32 m_x;
        private Int32 m_y;
        /* ERROR!
        public SomeType() {
            
        }
        */
        /* ERROR!
        public SomeType(Int32 x)
        {
            m_x = x;
        }
        */
        public SomeType(Int32 x)
        {
            this = new SomeType();
            m_x = x;
        }
    }
}
namespace TypeConstructors {
    //По умолчанию у типа не определяется конструктор
    //У типа не может быть более одного конструктора
    //У типа не может быть конструктора с параметрами
    //Конструкторы типов сегда должны быть закрытыми
    
    internal sealed class SomeType {
        static SomeType() { }
    }

    //Конструкторы значимых типов никогда не стоит объявлять, потому что иногда они не вызываются
    internal struct SomeValueType
    {
        public Int32 x;
        static SomeValueType() {
            Console.WriteLine("Never!");        //Не выполнится при работе метода Program.Main()
        }
    }

    public sealed class Program {
        public static void Main() {
            SomeValueType[] a = new SomeValueType[10];
            a[0].x = 123;
            Console.WriteLine(a[0].x);
        }
    }

    //Код конструктора может обращаться только к статическим полям
    //Инициализация inline статических полей значимого типа допустима, в отличие от экземплярных
    //Конструктор типа не должен вызывать конструктор базового класса

    public sealed class SomeTypeQ {
        static public Int32 x = 5;
        static SomeTypeQ() {
            x = 3;
        }
    }
    //х будет равен 3. Т.е. сначала идёт inline инициализации, затем статический конструктор.
}
namespace OverloadOperatorMethods {
    //CLR ничего не знает об операторах, они определяются языком. Т.е. CLR считает из обычными методами, определёнными для конкретных типов
    //Спецификация CLR требует, чтобы перегруженные операторные методы были открытыми и статическими + возвращаемый тип совпадал с типом хотя бы одного из параметров
    public sealed class Complex {
        private Int32 r;
        private Int32 i;
        public static Complex operator +(Complex c1, Complex c2)
        {
            Complex c = new Complex
            {
                r = c1.r + c2.r,
                i = c1.i + c2.i
            };
            return c;
        }
    }
}
namespace ConversionOperatorMethods {



    public sealed class Program {
        //Пусть мы хотим создать тип Rational, к которому хотим приводить типы Int32, Single...
        /// <summary>
        /// //////////////////////////////////////////////////////////////////////////////
        /// </summary>
        public struct Rational {
            private Int32 z;
            private Decimal q;
            public Rational(Int32 num)              //Создаёт Rational из Int32
            {
                z = num;
                q = 0;
            }
            public Rational(Single num)             //Создаёт Rational из Single
            {
                z = (Int32)Math.Truncate(num);
                q = (decimal)num - z;
            }
            public Int32 ToInt32()                  //Преобразует Rational в Int32
            {
                return z;
            }
            public Single ToSingle()                //Преобразует Rational в Single
            {
                return (Single)(z + q);
            }

            //<явное или неявное преобразование (implicit/explicit)> opertor <Возвращаемый тип>(<Исходный тип> name) {...}

            public static implicit operator Rational(Int32 num)     //Неявно создаёт Rational из Int32 и возвращает полученный объект
            {
                return new Rational(num);
            }
            public static implicit operator Rational(Single num)    //Неявно создаёт Rational из Single и возвращает полученный объект
            {
                return new Rational(num);
            }
            public static explicit operator Int32(Rational r)       //Явно возвращает объект типа Int32, полученный из Rational 
            {
                return r.ToInt32();
            }
            public static explicit operator Single(Rational r)      //Явно возвращает объект типа Single, полученный из Rational 
            {
                return r.ToSingle();
            }
        }
        
        public static void Main() {
            Rational k = 3;           
            Int32 c = 10000000;
            Rational p = 1.23f;
            Single f = 7.45f;
            c = (Int32)k;
            f = (Single)p;
            c = (Int32)p;
            f = (Single)k;
        }
    }    
}
namespace ExtencionMethods {
    public static class StringBuilderExtension {                                //Статический метод сатического класса с первым параметром this <Расширяемый тип> <name>
        public static Int32 IndexOf(this StringBuilder sb, Char value) {
            for (Int32 i = 0; i < sb.Length; i++) {
                if (sb[i] == value) {
                    return i;
                }
            }
            return -1;
        }
    }
    public sealed class Program {
        static public void Main() {
            StringBuilder s = new StringBuilder("kek");
            Int32 a = s.IndexOf('k');
        }
    }

    /*
     * Правила и рекомендации по созданию методов расширений:
     * C# не поддерживает свойств расшиерний, событий, операторв и тп
     * Методы расширения должны быть объявлены в статическом необобщенном классе. Хотя бы один параметр и только первый параметр отмечен ключевым словом this
     * Метод расширения должен быть объявлен в статическом классе первого уровня (не вложенном)
     * Не создавать МР с первым параметром System.Object, т.к. будет расширением для всех проихводных типов
     * Т.к. МР - статический, то CLR не проверяет переменную на null
     */

    //Возможно определение МР для интерфейсов:
    public static class IEnumerableExtension {
        public static void ShowItems<T>(this IEnumerable<T> collection) {       //Можно вызвать для любого класса, реализующего IEnumerable
            foreach (var c in collection)
            {
                Console.WriteLine(c);
            }
        }
    }
    //Делегаты + Атрибут расширения
}
namespace PartialMethods {

    //Первый файл
    internal sealed partial class Base {
        private String m_name;

        partial void OnNameChanging(String value);      //Объявление с определением частичного метода

        public String Name {
            get { return m_name; }
            set {
                OnNameChanging(value.ToUpper());        //Проверяем, что изменение не привело к обNULLению
                m_name = value;
            }
        }
    }

    //Второй файл
    internal sealed partial class Base {
        partial void OnNameChanging(string value)       //Написанный метод
        {
            if (String.IsNullOrEmpty(value))
            throw new ArgumentNullException("value");
        }
    }

    /*
     * Правила и рекомендации при создании частичных методов (ЧМ):
     * ЧМ могут объявляться только внутрит частичного класса или структуры
     * ЧМ всегда возвращает void и не содержит параметров с ключевым словом out
     * У ЧМ должны быть идентичные сигнатуры
     */
}
