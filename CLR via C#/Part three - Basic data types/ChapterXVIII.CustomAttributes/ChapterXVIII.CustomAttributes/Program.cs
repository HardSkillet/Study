using System;
using System.Reflection;
using ComparingAttributeInstances;

/*
 * C# позволяет применять настраиваемые атрибуты только к исходному коду, определяещему такие элементы как
    * сборки
    * модули
    * типы (классы, структуры, перечисления, интерфейсы, делегаты)
    * поля
    * методы (в том числе конструкторы)
    * параметры методов
    * возвращаемые значения методов
    * свойства
    * события
    * параметры обобщенного типа
 */

[assembly: SomeAttr]    //Применяется к сборке
[module: SomeAttr]      //Применяется к модулю
[type: SomeAttr]        //Применяется к типу
internal sealed class SomeType<[typevar: SomeAttr] T> {     //Применяется к переменной обобщенного типа
    [field: SomeAttr]               //Применяется к полю
    public Int32 SomeField = 0;

    [return: SomeAttr]              //Применяется к возвращаемому значению
    [method: SomeAttr]              //Применяется к методу
    public Int32 SomeMethod(
        [param: SomeAttr]           //Применяется к параметру
        Int32 SomeParam)
    { return SomeParam; }

    [property: SomeAttr]            //Применяется к свойству
    public String SomeProp {
        [method: SomeAttr]          //Применяется к механизму доступа get
        get { return null; }
    }

    [event: SomeAttr]               //Применяется к событиям
    [field: SomeAttr]               //Применяется к полям, созданным компилятором
    [method: SomeAttr]              ////Применяется к созданным методам add и remove

    public event EventHandler SomeEvent;
}

/*
 * Настраиваемый атрибут - это всего лишь экземпляр типа
 * Должен прямо или косвенно наследоваться от System.Attribute
 */

/*
 [DllImport("Kernel23", CharSet = CharSet.Auro, SetLastError = true)] - синтаксис вызова конструктора атрибута(т.к. атрибут это класс)
 * "Kernel23" - передается в качестве параметра. Параметры конструктора называются позиционными
 * CharSet = CharSet.Auro, SetLastError = true - присвоение значений открытым экземплярынм полям. Параметры, задающие поля или свойства называются именованными

 */
internal class SomeAttrAttribute : Attribute
{
}

//Создание собственного настраиваемого атрибута
namespace Attributes
{
    [AttributeUsage(AttributeTargets.Enum, Inherited = false)]      //Атрибут, применяющийся к атрибуту. Указывает компилятору, 
    public class FlagsAttribute : Attribute                         //что наш кастомный атрибут применяется только к перечислимым типам, и не будет применятся к производным классам
    {
        public FlagsAttribute() { }
    }

    public sealed class SomeClass
    {
        //Применяется ли к заданному типу экзмепляр  тип FlagsAttribute
        public void SomeMethod() {
            if (this.GetType().IsDefined(typeof(FlagsAttribute), false)) {
                //Да
            }
            else {
                //Нет
            }
        }
    }
    /*
     * Методы класса System.Reflection.CustomAttributeExtensions:
        * IsDefined - возвращает true при наличии хотя бы одного экзмепляра указанного класса, производного от Attribute, связанного с целью.
        Работает быстро, т.к. не создает (не десереиализует) никаких экземпляров класса Attribute.
        * GetCustomAttributes - возвращает массив, каждый элемент которого является экземпляром указанного атрибута. 
        Десериализует каждый экземпляр с использованием указанных при компиляции параметров/свойств.
        Если цель не имеет экземпляров заданного класса, возвращает ссылку на пустую коллекцию.
        * GetCustomAttribute - возвращает экземпляр указанного класса атрибута. 
        Десериализует экземпляр с использованием указанных при компиляции параметров/свойств.
        Если цель не имеет экземпляров заданного класса, генерирует исключение.
     */
}
namespace ComparingAttributeInstances {
    [Flags]
    internal enum Accounts {
        Savings = 0x0001,
        Checking = 0x0002,
        Brokerage = 0x0004
    }
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class AccountsAttribute : Attribute {
        private Accounts m_accounts;
        public AccountsAttribute(Accounts accounts)
        {
            m_accounts = accounts;
        }
        public override Boolean Match(object obj)
        {
            //Если в базовом классе реализован метод Match и это не класс Attribute,
            //расскоментировать следующую строку
            //if (!base.Match(obj)) return false;

            //Т.к. this не равен null, если obj не равен null, объекты не совпадают
            //Можно удалить, если базовый тип корректно реализует Match
            if (obj == null) return false;

            //Если типы не равны, то не равны и экземпляры
            //Можно удалить, если базовый тип корректно реализует Match
            if (this.GetType() != obj.GetType()) return false;

            //Привидение obj к нашему типу для доступа к полям
            //Приведение всегда работает, т.к. объекты одного типа
            AccountsAttribute other = (AccountsAttribute)obj;

            //Сравнение полей - является ли accounts this подмножеством acoounts other
            if ((other.m_accounts & m_accounts) != m_accounts) return false;
            
            //Объекты совпадают
            return true;
        }
        public override Boolean Equals(object obj)
        {
            //Если в базовом классе реализован метод Match и это не класс Attribute,
            //расскоментировать следующую строку
            //if (!base.Equals(obj)) return false;

            //Т.к. this не равен null, если obj не равен null, объекты не совпадают
            //Можно удалить, если базовый тип корректно реализует Equals
            if (obj == null) return false;

            //Если типы не равны, то не равны и экземпляры
            //Можно удалить, если базовый тип корректно реализует Equals
            if (this.GetType() != obj.GetType()) return false;

            //Привидение obj к нашему типу для доступа к полям
            //Приведение всегда работает, т.к. объекты одного типа
            AccountsAttribute other = (AccountsAttribute)obj;

            //Сравнение полей
            if (other.m_accounts != m_accounts) return false;
            return true;
        }
        public override Int32 GetHashCode()
        {
            return (Int32)m_accounts;
        }
    }
    [Accounts(Accounts.Savings)]
    internal sealed class ChildAccount {
        
    }
    [Accounts(Accounts.Savings | Accounts.Checking | Accounts.Brokerage)]
    internal sealed class AdultAccount { }
}
namespace ChapterXVIII.CustomAttributes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CanWriteCheck(new ChildAccount());
            CanWriteCheck(new AdultAccount());
            CanWriteCheck(new Program());
        }
        private static void CanWriteCheck(Object obj) {
            //Создание и инициализация типа атрибута
            Attribute checking = new AccountsAttribute(Accounts.Checking);

            //Создание экземпляра атрибута применяемого к типу
            Attribute validAccounts = obj.GetType().GetCustomAttribute<AccountsAttribute>(false);

            //Сравнение с помощью метода Match
            if ((validAccounts != null) && checking.Match(validAccounts))
            {
                Console.WriteLine("{0} types can write checks.", obj.GetType());
            }
            else {
                Console.WriteLine("{0} types can NOT write checks.", obj.GetType());
            }   
        }
    }
}
