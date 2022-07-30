using System;

namespace ChapterXV.EnumTypes
{
    internal sealed class SomeEnum
    {
        //Перечислимые типы - набор пар, состоящий из символьных имен и соответствующих им значений
        //[Flags] - для создания битовых флагов
        internal enum Color : Byte  //В основе типа лежит Byte
        {
            White,  //0
            Red,    //1            
            Green,  //2
            Blue,   //3
            Orange  //4
        }
        /*
         * У перечислимых типов не может иметь экземплярных методов (кроме методов расширений)
         * Компилятор считает перечислимые типы примитивными
         * С# поддерживает привидение одного перечислимого типа к другому
         */
        internal static void Main(string[] args)
        {
            Color color = Color.Blue;                                   //Создание экземпляра типа Color
            Color[] colors = (Color[])Enum.GetValues(typeof(Color));    //Получение всех значений типа
            foreach (Color c in colors) {                               //Вывод всех идентификаторов и числовых значений
                Console.WriteLine("{0, 5:D}\t{0:G}", c);
            }
        }
        //Битовые флаги
    }
}
