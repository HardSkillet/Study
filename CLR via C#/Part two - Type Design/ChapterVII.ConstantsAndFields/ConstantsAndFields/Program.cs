using System;

namespace Constants
{
    //Идентификатор, значение которого никогда не меняется и определяется на этапе компиляции и только для примитивных типов (либо null для непримитивных типов)
    //Т.к. константы не изменяются - они считаются статическими, т.е. часть типа, а не экземпляра
    public sealed class SomeType {
        public const SomeType Empty = null;
    }

    //Рассмотрим две сборки
    //first
    public sealed class First {
        public const Int32 M = 50;
    }
    //second
    public sealed class Program {
        public static void Method() {
            Console.WriteLine(First.M);
        }
    }
    //Если поменять значение константы в первой сборке, значение константы во второй сборке при отсутствии перекомпиляции не изменится. В таких случаях лучше использовать readonly поля
}
namespace Fields
{
    //Поле (Field) - это член данных, хранящий экземпляр значимого типа или ссылку на ссылочкный тип
    /*
     * Модификаторы для полей:
        * static - поле, является частью типа, а не экземпляра (объекта)
        * readonly - запись в поле разрешается только из кода конструктора
        * volatile - код, обращающийся к полю не должен быть оптимизирован компилятором
     */

    //Рассмотрим предыдщий пример с двумя сборками
    //first
    public sealed class First
    {
        public static readonly Int32 M = 50;        //const - static readonly
    }
    //second
    public sealed class Program
    {
        public static void Main()
        {
            Console.WriteLine(First.M);
        }
    }
    //Теперь при изменении значения поля в первой сборке не обязательно перекомпилировать вторую, чтобы ожидать корректного результата
    public sealed class SomeType {
        public static readonly Random s_random = new Random();  //Статическое неизменяемое поле

        private static Int32 s_numberOfWrites = 0;              //Статическое изменяемое поле

        public readonly String Pathname = "Untitled";           //Неименяемое экземплярное поле

        private System.IO.FileStream m_fs;                      //Изменяемое экземплярное поле

        public SomeType(String pathname) {                      //Конструктора экземпляра
            this.Pathname = pathname;                           //Это возможно, т.к. код расположен в конструкторе
        }

        public String DoSomething() {                           //Метод
            s_numberOfWrites += 1;
            return Pathname;
        }
    }
    public sealed class AType {
        public static readonly Char[] InvalidChars = new char[] { 'A', 'B', 'C' };
    }
    public sealed class AnotherType {
        public static void M()
        {
            AType.InvalidChars[0] = 'Z';
            AType.InvalidChars[1] = 'X';
            AType.InvalidChars[2] = 'Y';        //Меняем значение, а не ссылку readonly
            //AType.InvalidChars = new char[2]; - ошибка
        }
    }
}
