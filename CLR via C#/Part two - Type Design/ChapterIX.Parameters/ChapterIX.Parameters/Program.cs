using System;

namespace OptionalAndNamedParameters            //Arguments*
{


    public static class Program
    {
        private static Int32 s_n = 0;
        private static void M(Int32 x = 9, String s = "A",
            DateTime dt = default(DateTime), Guid guid = new Guid())
        {
            Console.WriteLine("x = {0}, s ={1}, dt = {2}, guid = {3}", x, s, dt, guid);
        }
        static void Again(string[] args)
        {
            M();
            M(8, "X");
            M(5, guid: Guid.NewGuid(), dt: DateTime.Now);
            M(s: (s_n++).ToString(), x: s_n++);
        }

        /*
         * Правила использования параметров:
            * Значения по умолчанию указываются для параметров методов, кострукторов методов и парметрических свойств
            * Параметры без значений по умолчанию должны быть слева от параметров, имеющих значения по умолчанию
            * Задавать значения по умолчанию можно только для примитивных типов и ссылочных, поддерживающий null. Для произвольных значимых типов можно использовать ключевые слова default или new
            * Запрещается переименовывать параметрические переменные, т.к. это влечет за собой необходимость редактирования вызывающего кода
            * В качестве значений по умолчанию лучше использовать 0 или null в сочетании с тернарным оператором
            * Для параметров помеченных ключевыми словами ref и out значения по умолчанию не задаются
         */
    }
}
namespace ImplicityTypedLocalVariables {
    public static class SomeType {
        private static void ShowVariableType<T>(this T t) {
            Console.WriteLine(typeof(T));
        }
        public static void Main() {
            var name = "Jeff";
            name.ShowVariableType();
        }
    }
    /*
     * При помощи ключевого слова var нельзя определит тип параметра метода, тип поля.
     */
}
namespace PassingParametersToAMethodByReference {
    //По умолчанию CLR предполагает, что все параметры методов передаются по значению
    /*
     * ключевые слова out и ref дают возможность передавать параметры по ссылке
     * out - разрешает передавать не инициализированный параметр
     * ref - параметр обязательно должен быть инициализирован
     */
    public sealed class Program {
        public static void Main() {
            Int32 x;
            GetVal(x);                              //Инициализация х не обязательна
            Console.WriteLine(x);                   //Выводится 10
            AddVal(x);                              //Требуется инициализация
            Console.WriteLine(x);                   //Выводится 30
        }
        private static void GetVal(out Int32 v) {
            v = 10;
        }
        private static void AddVal(ref Int32 v) {
            v += 20;
        }
    }
}