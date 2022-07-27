using InheritanceInClassesAndInterfaces;
using System;
using System.Collections;
/*
* В интерфейсе можно определить:
* Именованный набор сигнатур методов
* События
* Свойства
* В интерфейсе нельзя определить:
* Экземплярные поля
* Конструкторы
* C# не позволяет определить статические члены
* Интерфейсы могут иметь модификатор доступа
* Насследование в интерфейсах. ICollection<T> наследует интерфейсы IEnumerable, IEnumerable<T>. Это значит:
* Любой класс, наследующий ICollection<T> должен реализовать все методы, определенные в ICollection<T>, IEnumerable, IEnumerable<T>
* Любой код, ожидающий объект, тип которого реализует ICollection<T>, может быть уверен, что это объект реализует и IEnumerable, IEnumerable<T>
*/
namespace InheritanceInClassesAndInterfaces {
    public interface IComparable<T> {
        Int32 CompareTo(T other);
    }
    public sealed class Point : IComparable<Point> {
        private Int32 m_x, m_y;
        public Point(Int32 x, Int32 y) {
            m_y = y;
            m_x = x;
        }
        public Int32 CompareTo(Point other)
        {
            return Math.Sign(Math.Sqrt(other.m_x * other.m_x + other.m_y * other.m_y) - Math.Sqrt(m_x * m_x + m_y * m_y));
        }
        public sealed override string ToString()
        {
            return String.Format("({0}, {1})", m_x, m_y);
        }
    }
    internal class Base : IDisposable
    {
        public void Dispose() { Console.WriteLine("Base's Dispose"); }
    }
    internal class Derived : Base, IDisposable {
        new public void Dispose() { 
            Console.WriteLine("Derived's Dispose"); 
            //base.Dispose(); Для вызова базового 
        }
    }
    class Program
    {
        static void M(string[] args)
        {
            Base b = new Base();
            b.Dispose();                    //Base's Dispose
            ((IDisposable)b).Dispose();     //Base's Dispose

            Derived d = new Derived();
            d.Dispose();                    //Derived's Dispose
            ((IDisposable)d).Dispose();     //Derived's Dispose

            b = new Derived();
            b.Dispose();                    //Base's Dispose
            ((IDisposable)b).Dispose();     //Derived's Dispose
        }
    }
}

namespace AboutCallingInterfaceMethods {
    class Program
    {
        static void M(string[] args)
        {
            //Используя переменную s, могу вызвать метод любого интерфейса, определенного в стринг, а так же методы типов String, Object
            String s = "Nikita";

            //Используя переменную cloneable, могу вызвать метод интерфейса ICloneable, а так же методы типa Object
            ICloneable cloneable = s;

            //Используя переменную comparable, могу вызвать метод интерфейса IComparable, а так же методы типa Object
            IComparable comparable = s;

            //Используя переменную enumerable, могу вызвать метод интерфейса IEnumerable, если объект приводимого типа реализует IEnumerable, а так же методы типa Object
            IEnumerable enumerable = (IEnumerable)comparable;
        }
    }
    //По умолчанию, интерфейсная версия метода и определяемая в классе, наследующем интерфейс идентичны
    //Но это можно изменить определив в наследующем классе интерфейсный метод отдельно. Например: void IDisposable.Dispose() {...} - определение интерфейсного метода
}

/*
 * Преимущества обобщенных интерфейсов:
 * 1. Безопасность типов на стадии компиляции
 * 2. Меньшее количество операций упаковки при работе со значимыми типами
 * 3. Возможность реализации классом одного интерфейса многократно
 */
namespace MultipleInterfacesWithTheSameSignaturesAndNames {
    public interface IWindow
    {
        Object GetMenu();
    }
    public interface IRestaurant {
        Object GetMenu();
    }

    public sealed class MarioPizzeria : IWindow, IRestaurant
    {
        Object IRestaurant.GetMenu()
        {
            Console.WriteLine("IRestaurant");
            return null;
        }
        Object IWindow.GetMenu()
        {
            Console.WriteLine("IWindow");
            return null;
        }
        Object GetMenu()
        {
            Console.WriteLine("GetMenuSimple");
            return null;
        }
        public static void Main() {
            var a = new MarioPizzeria();
            a.GetMenu();                    //GetMenuSimple

            IWindow window = a;
            window.GetMenu();               //IWindow
            
            IRestaurant restaurant = a;
            restaurant.GetMenu();           //IRestaurant
        }
    }
}

namespace EIMI {
    public interface IComparable {
        Int32 CompareTo(Object other);
    }
    internal struct SomeValueType : IComparable
    {
        private Int32 m_x;
        public SomeValueType(Int32 x) { m_x = x; }
        public Int32 CompareTo(Object other)
        {
            return (m_x - ((SomeValueType)other).m_x);
        }
    }
    internal struct SomeValueTypeSafe : IComparable
    {
        private Int32 m_x;
        public SomeValueTypeSafe(Int32 x) { m_x = x; }
        public Int32 CompareTo(SomeValueTypeSafe other)
        {
            return (m_x - other.m_x);
        }
        Int32 IComparable.CompareTo(object other) {
            return CompareTo((SomeValueTypeSafe)other);
        }
    }

    public static class Program {
        public static void Main() {

            SomeValueType v = new SomeValueType(0); 
            Object o = new Object();
            Int32 n = v.CompareTo(v);                   //Упаковка
            n = v.CompareTo(o);                         //Ошибка во время исполнения. Нарушена целостность типов

            SomeValueTypeSafe v2 = new SomeValueTypeSafe(0);
            Object o2 = new Object();
            Int32 n2 = v2.CompareTo(v2);                //Нет упаковки
            n2 = ((IComparable)v2).CompareTo(o2);       //Ошибка на этапе компиляции, если не приводить к интерфейсному типу
        }
    }
}
/*
 * Минусы EIMI:
    * Отсутствие документации о том, как именно тип реализует EIMI-метод
    * При приведении к интерфейсному типу значимые экземпляры упаковываются
    * EIMI нельзя вызвать из производного типа
 */

/*
 * Дилемма разработчика: базовый класс или интерфейс?
 * Плюсы базового класса:
    * Связь потомка с предком
    * Простота использования - при наследовании от базового типа нужны будут лишь небольшие изменения, от интерфейса - реализовать каждый член
    * Четка реализация
    * Управление версиями
 */