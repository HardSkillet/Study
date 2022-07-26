using System;
using System.Collections.Generic;

namespace PropertiesWithoutParameters 
{
    public sealed class Employee
    {
        Int32 Age;
        String Name;
        public Employee(Int32 a, String n) {
            Age = a;
            Name = n;
        }
    }
    public sealed class EployeeProps {
        private Int32 m_Age;
        private String m_Name;
        public String Name {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public Int32 Age {
            get { return m_Age; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value.ToString(), "Smth");
                m_Age = value;
            }
        }
    }
    //CLR поддерживает статические, абстрактные, виртуальные и экземплярные свойства
    //У каждого свойства есть имя и тип (не Void). Нельзя перегружать свойства
    //Определяя свойства обычно описывают пару методов get и set, каждый из которых можно опустить. Компилятор автоматически генерирует методы с припиской get_/set_ к имени переменной
    public sealed class EmployeeAuto {
        public Int32 m_Age;
        public string Name { get; set; }        //Автоматически реализуемое свойство
        public Int32 Age
        {
            get { return m_Age; }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("value", value.ToString(), "Smth");
                m_Age = value;
            }
        }
        EmployeeAuto demo = new EmployeeAuto { Name = "Nik", Age = 21 };  //Сокращенный вызов для класса, в котором определены свойства
    }
    public sealed class Classroom {
        private List<String> m_students = new List<string>();
        public List<String> Students { get { return m_students; } }
        public Classroom() { }
    }
    public sealed class SomeClass {
        public static void NoMain() { 
            Classroom classroom = new Classroom             //Код преобразуется в создание объекта и 3х вызовов метода Add -> classroom.Students.Add("Nik");
            {
                Students = { "Nik", "NewName3", "Name" }
            };
            foreach (var student in classroom.Students)
            {
                Console.WriteLine(student);
            }
        }       
    }
}
namespace AnonymousTypes {
    public static class Program {
        public static void NoMain()
        {
            var o1 = new { Name = "Nik", Age = 21 };     //Анонимный тип с полями Name, Age
            String Name = "Nikita";
            Int32 Age = 21;
            var o2 = new { Name, Age };
            
            o1 = o2;                                    //Тип о1 и о2 одинаков, т.к. у них совпадают сигнатуры полей и сами поля

            var people = new[] {                        //Массив явных типов из анонимных типов
                o1,
                o2,
                new {Name ="Kirill", Age =22 },
                new {Name ="Kirill2", Age =23 },
                new {Name ="Kirill3", Age =24}
            };
        }

        /*
         * var o = new { property1 = expression1, ..., propertyN = expressionN };
         * 1. Компилятор определяет тип каждого выражения из кода выше
         * 2. Создает закрытые поля этих типов
         * 3. Для каждого типа поля создает открытые свойства только для чтения
         * 4. Создает конструкторы для всех этих выражений
         * 5. Переопределяет методы Equals(), GetHashCode(), ToString()
         */

        //Анонимные типы используются при работе c LINQ
    }
}
namespace Type_System.Tuple
{
    //В пространстве имен System определено несколько обобщенных типов Tuple, отличающиеся только количеством обобщенных параметров
    //Tuple<T1>, ......, Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>
    public static class Program
    {
        public static Tuple<Int32, Int32> SomeMethod(Int32 a, Int32 b) {            //Пример использования Tuple для возврата нескольких значений из метода
            
            //Some usefull code

            return new Tuple<Int32, Int32>(a, b);
        }
        public static void Method() {
            Int32 a = 5;
            Int32 b = 6;
            var c = SomeMethod(a, b);
            a = c.Item1;
            b = c.Item2;
        }
    }
}
namespace PropertiesWithParameters {
    public sealed class BitArray {
        private Byte[] m_byteArray;             //Закрытый байтовый массив, хранящий биты
        private Int32 m_numBits;

        public BitArray(Int32 numBits) {        //Конструктор, выделяющий память и устанавливающий все биты в 0

            //Проверка аргументов
            if (numBits <= 0) throw new ArgumentOutOfRangeException("numBits must be > 0");

            //Сохраняем число битов
            m_numBits = numBits;

            //Выделяем байты
            m_byteArray = new Byte[(m_numBits + 7) / 8];
        }

        public Boolean this[Int32 bitPos] {         //Индексатор (свойство с параметрами)
            get
            {
                //Проверка аргументов
                if ((bitPos < 0) || (bitPos >= m_numBits))
                    throw new ArgumentOutOfRangeException("bitPos");
                
                //Возвращаем состояние индексируемого бита
                return (m_byteArray[bitPos / 8] & (1 << (bitPos % 8))) != 0;
            }
            set {

                //Проверка аргументов
                if ((bitPos < 0) || (bitPos >= m_numBits))
                    throw new ArgumentOutOfRangeException("bitPos");

                if (value)
                {
                    m_byteArray[bitPos / 8] = (Byte)
                        (m_byteArray[bitPos / 8] | (1 << (bitPos % 8)));
                } else {
                    m_byteArray[bitPos / 8] = (Byte)
                        (m_byteArray[bitPos / 8] & ~(1 << (bitPos % 8)));
                }
            }
        }
    }
    public static class Program {
        public static void Main()
        {
            BitArray ba = new BitArray(14);
            for (Int32 i = 0; i < 14; i++)
            {
                ba[i] = (i % 2 == 0);
            }
            for (Int32 i = 0; i < 14; i++)
            {
                Console.WriteLine("Bit " + i + " is " + (ba[i] ? "On" : "Off"));
            }
        }
    }

    /*
     * В C# индексаторы рассматриваются как возможность перегрузить оператор []
     */
}