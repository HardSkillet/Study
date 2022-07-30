using System;
using System.Text;
using System.Threading;
/*
* Три способа преобразования Char <-> Int:
* 1. Привидение типов. Лучший вариант. Нет вызова доп методов
* 2. Convert - выполняют преобразовнаие как проверяемую операцию, чтобы не получать исключения
* 3. IConvertable - худший вариант, т.к. происходит упаковка числовых типов
*/
namespace TypeChar { 
    public static class Program
    {
        public static void M()
        {
            Double d;
            d = Char.GetNumericValue('\u0033');     
            Console.WriteLine(d.ToString());        //\u0033 -> 3

            d = Char.GetNumericValue('\u00bc');
            Console.WriteLine(d.ToString());        //\u00bc -> 0.25

            d = Char.GetNumericValue('#');
            Console.WriteLine(d.ToString());        //Любой не числовой символ -> -1

            Char c;
            Int32 n;
            c = (Char)65;
            Console.WriteLine(c);                   //A
            
            n = (Int32)c;
            Console.WriteLine(n.ToString());        //65

            c = unchecked((Char)(65536 + 65));
            Console.WriteLine(c);                   //A

            c = Convert.ToChar(65);
            Console.WriteLine(c);                   //A

            n = Convert.ToInt32(c);
            Console.WriteLine(n);                   //65

            try { c = Convert.ToChar(70000); }
            catch { Console.WriteLine("Can't convert 70000 to a Char"); }

            c = ((IConvertible)65).ToChar(null);
            Console.WriteLine(c);                   //A

            n = ((IConvertible)c).ToInt32(null);    //65
            Console.WriteLine(n);
        }
    }
}

namespace TypeString
{
    /*
     * String - примитивный тип, а значит компилятор разрешает вставлять литеральные строки непосредственно в код
     * new - (не?) может использоваться для создания строк из литералов
     */
    public sealed class Program
    {
        public static void M()
        {
            //Способы задания строк
            String s = new String("Hello");
            var s2 = "123";

            Console.WriteLine(s + Environment.NewLine + s2);    //Код выводит s, s1 в разных строках для любой системы

            String s3 = "Hi" + "there";                         //Конкатенация выполняется на этапе компиляции

            String s4 = @"C:\program\...";                   //Буквальный способ задания строки
        }
        /* 
         * В C# строки не изеняемы
         * Несколько строковых объектов могут ссылаться на один объект из литералов - интернирование строк
        
         *******************************************************************************************************
         
         * При сортировке строк любым методом рекомендуется передавать аргумент типа StringComparison ->
         * public enum StringComparison
        {
            CurrentCulture = 0;
            CurretCultureIgnoreCase = 1;
            InvariantCulture = 2;
            InvariantCultureIgnoreCase = 3;
            Ordinal = 4;
            OrdinalIgnoreCase = 5;
        }

         * В Microsoft оптимизировно сравниение строк в верхнем регистре
        
         *******************************************************************************************************
         
         * System.Globaliztion.CultureInfo:
            * en-US -> Американский английский
            * en-АU -> Автсралийский английский
            * de-DE -> Германский немецкий
         */
        public static void M() {
            String s1 = "Hello";
            String s2 = "Hello";
            Console.WriteLine(Object.ReferenceEquals(s1, s2));          //В некоторых версиях языка не происходит интернирования - False

            s1 = String.Intern(s1);
            s2 = String.Intern(s2);                                     //Явное интернирование строк
            Console.WriteLine(Object.ReferenceEquals(s1, s2));          //True
        }
    }
}

namespace StringBuild {
    //Не считается примитивным типом
    /*
     * StringBuilde - основные поля:
        * Максимальная емкость 
            * Для уже созданного объекта изменить нельзя
            * По умолчанию ~2 млрд
        * Емкость
            * Показывает размер массива символов
            * По умолчанию равна 16
            * Автоматически удваивает емкость, если не хватает места для добавления нового символа/строки, а затем копирует все элементы
        * Массив символов
            * Число символов всегда меньше или равно емкости
     * Таким образом, StringBuilder задействует память только в двух случаях
        * При динамическом построении строки превышающем емкость
        * Вызове ToString()
     * Большинство методов возвращают ссылку на тот же объект типа StringBuilder
     */
    public sealed class SomeClass {
        public static void Main(){
            //StringBuilder s1 = "kek"; Error
            StringBuilder s = new StringBuilder("Nikita");

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(new BoldInt32s(), "{0} {1} {2:M}", "Nikita", 123, DateTime.Now);
        }
        //Создание собственного средства форматирования
    }
    internal sealed class BoldInt32s : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            String s;
            IFormattable formattable = arg as IFormattable;
            if (formattable == null) s = arg.ToString();
            else s = formattable.ToString(format, formatProvider);

            if (arg.GetType() == typeof(Int32))
                return "<B>" + s + "</B>";
            return s;
        }

        public Object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return Thread.CurrentThread.CurrentCulture.GetFormat(formatType);
        }
    }
}
//Парс строки - статический метод Parse или TryParse(Возвращает true/false)

