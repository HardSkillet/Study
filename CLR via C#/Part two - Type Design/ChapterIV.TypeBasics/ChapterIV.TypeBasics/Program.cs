using System;
using WW = Wintellect.Widget;
using MW = Microsoft.Widget;

namespace Microsoft
{
    public class Widget {
    
    }
}
namespace Wintellect {
    public class Widget {
        
    }
}

namespace Chapter_IV.TypeBasics
{
    internal class Employee : System.Object {
        
    }

    internal class Manager : Employee { 
    
    }

    class Program
    {
        public static void PromoteEmployee(object o) {
            Employee e = (Employee)o;
        }

        static void Main(string[] args)
        {
                        //Все типы производные от System.Object


            Employee temp = new Employee();
            var tempTwo = new Employee();
            var tempThree = temp;
            Console.WriteLine(temp.Equals(tempTwo));        //False
            Console.WriteLine(temp.Equals(tempThree));      //True
            Console.WriteLine(temp.GetHashCode());          //58225482
            Console.WriteLine(temp.ToString());             //Chapter_IV.TypeBasics.Employee
            Console.WriteLine(temp.GetType());              //Chapter_IV.TypeBasics.Employee <namespace>.<class>

            /*
            Все объекты создаются с помощью оператора new, который выполняет следующий действия:
            1. Вычисление количества байтов памяти для хранения объекта с его указателями на объект-тип и индексом блока синхронизации
            2. Выделение памяти в управляемой куче. Байты инициализируются нулями
            3. Инициализация указателя на объект-тип и индекса блока синхронизации
            4. Вызов консктруктора типа с заданными параметрами
            После чего new возвращает указатель на созданный объект 
             */


                        //Привидение типов

            Object o = new Employee(); //Привидение типов не требуется, т.к. Object базовый тип для Employee
            Employee e = (Employee) o; //Привидение типов требуется, т.к. Employee производный от Object
            
            Manager m = new Manager();
            PromoteEmployee(m);                             //Manager производный от Employee - метод работает     
            DateTime newYears = new DateTime(2022, 1, 1);   //DateTime не производный от Employee - System.InvalidCastException
            PromoteEmployee(newYears);


                        //Првидение типов в C# с помощью операторов is и as

            o = new Object();
            Boolean b1 = (o is Object);     //True
            Boolean b2 = (o is Employee);   //False
            if (o is Employee) {            //Проверка совместимости о с Employee
                e = (Employee)o;
                /* ... */
            }
            e = o as Employee;              //Если о совместим с Employee, то е != null
            if (e != null) {
            /* ... */
            }
            e.ToString();                   //Может привести к NullReferenceException, если е - null


                        //Пространство имен и сборки

            Microsoft.Widget w = new Microsoft.Widget();        //Обязательно указание namespace, т.к. Widget существует в 2ух пространствах имен
            Wintellect.Widget ww = new Wintellect.Widget();
            WW www = new WW();                                  //Альтернативной способ с использованием using WW = Wintellect.Widget;
        }


                        //Связь между сборками и пространством имен

        /*
            Пространство имен и сборка (файл, в котором реализован тип) не обязательно связаны друг с другом. Различные типы,
        принадлежащие одному пространству имен могут лежать в разных сборках, а одна сборка может содержать типы разных пространств имен.
         */



    }
}
