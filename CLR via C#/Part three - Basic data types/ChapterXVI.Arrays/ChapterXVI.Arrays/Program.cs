using System;

namespace ChapterXVI.Arrays
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            Int32[] a = new Int32[3];   //Массив интов длины 3
            
            Int32[,] a2 = new Int32[2, 3];  //Двумерный массив интов 2 на 3
            
            Int32[][] a3 = new Int32[2][];  //Массив массивов интов длины 2
            a3[0] = new Int32[2];           //Вложенный массив интов длины 2
            a3[1] = new Int32[3];           //Вложенный массив интов длины 3

            String[] names2 = { "Shrek", "Nikita", null };   //Один из способов инициализации массива

            var names3 = new[] { new { Name = "Shrek" }, new { Name = "Nikita" } }; //Инициализация массива анонимными типами
            foreach (var k in names3) {
                Console.WriteLine(k.Name);
            }

            var names = new[] { "Shrek", "Nikita", null };   //Один из способов инициализации массива

            Object[] obj = names;       //Допустимо привидение типов

            Object[] obj2 = new Object[3];

            //В C# не поддерживается привидение массивов с элементами значимыми типами, но это ограничение можно обойти

            Array.Copy(a, obj2, a.Length);
            /*
             * Метод Copy выполянет следующие действия:
                * Упаков элементов значимого типа
                * Распаковка элементов ссылочного типа при преобразовании к значимому
                * Расширение примитивных значимых типов (инт к даблу)
             * Метод Copy можно использовать, чтобы передать массив в метод не по ссылке (т.е. без доступа к изменениям в исходном массиве) 
             */


            //Создание массива с ненулевой нижней границей
            Int32[] lowerbound = { 2005, 1 };
            Int32[] lengths = { 5, 4 };
            Decimal[,] quarterleRevenue = (Decimal[,])Array.CreateInstance(typeof(Decimal), lengths, lowerbound);
            Console.WriteLine("{0, 4} {1, 9} {2, 9} {3, 9} {4, 9}", "Year", "Q1", "Q2", "Q3", "Q4");

            Int32 firstYear = quarterleRevenue.GetLowerBound(0);
            Int32 lastYear = quarterleRevenue.GetUpperBound(0);
            Int32 firstQuarter = quarterleRevenue.GetLowerBound(1);
            Int32 lastQuarter = quarterleRevenue.GetUpperBound(1);
            for (Int32 year = firstYear; year <= lastYear; year++) {
                Console.Write(year + " ");
                for (Int32 quarter = firstQuarter; quarter <= lastQuarter; quarter++) {
                    Console.Write("{0,9:C} ", quarterleRevenue[year, quarter]);
                }
                Console.WriteLine();
            }
        }
    }
}
