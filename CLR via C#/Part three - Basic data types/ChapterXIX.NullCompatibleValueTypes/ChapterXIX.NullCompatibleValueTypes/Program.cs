using System;

namespace ChapterXIX.NullCompatibleValueTypes
{
    internal sealed class Program
    {
        internal static void Main(string[] args)
        {
            Nullable<Int32> x = 5;
            Nullable<Int32> y = null;
            Console.WriteLine("x: HasValue={0}, Value={1}", x.HasValue, x.Value);
            Console.WriteLine("y: HasValue={0}, Value={1}", y.HasValue, y.GetValueOrDefault());
            Int32? t = null;
            Int32 a = (Int32)t;
            Console.WriteLine(a);
            Int32? b = null;
            Int32 p = b ?? 123;     //p = 123
            /*
             * Вот как некоторые операторы интерпритирует C# для Null-label
                * Унарные операторы (+, ++, -. --. !, ~) Если операнд равен null, результат тоже равен null
                * Бинарные операторы (+, -, *, /, %, &, |, ^, <<, >>) - null, если один из операндов null
                    * исключение & и |
                * Операторы равенства (==, !=). Если оба оператора null, они равны. Если только один null - не равны
                * Операторы сравнения (<, >, <=, >=). Если один из операндов null - false
             */

            //При перегрузке операторов null-label наследует от значимого типа эту перегрузку
        }
    }
}
