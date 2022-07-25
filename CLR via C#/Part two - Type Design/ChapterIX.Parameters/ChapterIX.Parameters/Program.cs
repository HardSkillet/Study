using System;
using System.IO;

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
    public sealed class ProgramValue {
        public static void Main() {
            Int32 x;
            GetVal(out x);                              //Инициализация х не обязательна
            Console.WriteLine(x);                       //Выводится 10
            Int32 y = 10;
            AddVal(ref y);                              //Требуется инициализация
            Console.WriteLine(x);                       //Выводится 30
            AddVal(y);
        }
        private static void GetVal(out Int32 v) {
            v = 10;
        }
        private static void AddVal(ref Int32 v) {
            v += 20;
        }
        private static void AddVal(Int32 v) {
            v += 10;
        }
        //Разрешается использовать перегрузку методов отличающихся только наличием ref/out. Т.к. ref и out компилируются в один IL-код, то нельзя перегружать метод отличающийся только ref или out
    }
    public sealed class PorgramReference {
        //Стоит использовать ключевые слова ref и out с ссылочными типами, только если внутри метода создаются новые объекты и необходимо вернтуть ссылку на них
        public static void Main() {
            FileStream fs;
            StartProcessingFiles(out fs);
            for (; fs != null; ContinueProcessingFiles(ref fs)) {
                fs.Read("smth");
            }
        }
        private static void StartProcessingFiles(out FileStream fs) {
            fs = new FileStream("Something");
        }
        private static void ContinueProcessingFiles(ref FileStream fs) {
            fs.Close();
            if (noMoreFilesToProcess) fs = null;
            else fs = new FileStream("Something");
        }

        //Код представленный ниже не будет работать для свопа двух строк без промежуточных переменных типа Object, т.к. переменные передаваемые в метод по ссылке должны быть одного типа(Object в данном случае)
        public static void Swap(ref Object a, ref Object b)
        {
            Object t = b;
            b = a;
            a = t;
        }
    }
}
namespace PassingAVariableNumberOfParameters {
    public sealed class SomeType {
        public static Int32 Add(params Int32[] values) {
            Int32 sum = 0;
            if (values != null) {
                for (Int32 i = 0; i < values.Length; i++) {
                    sum += values[i];
                }
            }
            return sum;
        }
        Int32 a = Add(1, 2, 3, 4, 5);                   //15
        Int32 b = Add(new Int32[] { 1, 2, 3, 4, 5 });   //15
        //Таким образом, мы можем понять, что сначала выделяется место и создаётся массив параметров, а затем происходит выполнение метода
        //Ключевым словом param может быть помечен только последний параметр метода и должен указывать на одномерный массив произвольного типа
    }
}
namespace TypesOfParametersAndReturnValues {
    //Для типа параметра метода рекомендуется выбирать более слабый тип, чтобы была возможность работать с большим числом типов. Например IEnumerable<T>, вместо ICollection<T>, IList<T>
    //Для типа возвращаемого методом объекта лучше выбирать более сильный тип
}