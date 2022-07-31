using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ChapterXX.Exceptions
{
    internal sealed class SomeClass
    {
        internal void SomeMethod()
        {
            try
            {
                //Код, требующий корректного восстановления или очистки ресурсов
            }
            catch (InvalidOperationException)
            {
                //Кода восстановления после исключения InvalidOperationException
            }
            catch (IOException)
            {
                //Кода восстановления после исключения IOException
            }
            catch
            {
                //Кода восстановления после остальных исключений   
                throw;
            }
            finally
            {
                //Код очистки ресурсов после операция, начатых в блоке try. Выполняется всегда, независимо от наличия исключения
            }
        }

        /*
         * Открытые свойства типа System.Exception:
            * Message - Текст с описанием причины исключения
            * Data - Ссылка на набор пар параметр-значение
            * Source - Имя сборки, сгенерировавшей исключение
            * StackTrace - Имена и сигнатуры методов, вызов которых стал причиной исключения
            * TargetSite - Имя метода, ставшего причиной исключения
            * HelpLink - Адрес документации с информацией об исключении
            * InnerExeption - Указывает на предыдущее исключение, если текущее исключение было вброшено при обработке предыдущего
            * HResult - 32-х разрядное значение, для интеграции управляемого кода с машинным
         */

        /*
         * Если в блоке catch написать "throw e;", где е - перехваченное исключение, то информация о начальной точке исключения будет потеряна.
         * Напротив, если в блоке catch написать "throw;" - информация о начальной точке исключения будет передана выше по стеку.
         */
    }

    [Serializable]
    public abstract class ExceptionArgs
    {
        public virtual String Message { get { return String.Empty; } }
    }

    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable
    where TExceptionArgs : ExceptionArgs
    {
        private String c_args = "Args";
        private readonly TExceptionArgs m_args;
        public TExceptionArgs Args { get { return m_args; } }
        public Exception(String message = null, Exception innerException = null) : this(null, message, innerException) { }
        public Exception(TExceptionArgs args, String message = null, Exception innerException = null) : base(message, innerException)
        {
            m_args = args;
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        private Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            m_args = (TExceptionArgs)info.GetValue(c_args, typeof(TExceptionArgs));
        }

        [SecurityPermission(SecurityAction.LinkDemand,
            Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(c_args, m_args);
            base.GetObjectData(info, context);
        }

        public override string Message
        {
            get
            {
                String baseMsg = base.Message;
                return (m_args == null) ? baseMsg : baseMsg + " (" + m_args.Message + ")";
            }
        }

        public override bool Equals(Object obj)
        {
            Exception<TExceptionArgs> other = obj as Exception<TExceptionArgs>;
            if (obj == null) return false;
            else return Object.Equals(m_args, other.m_args) && base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public sealed class DiskFullExceptionArgs : ExceptionArgs
    {
        private readonly String m_diskpath;
        public DiskFullExceptionArgs(String diskpath) { m_diskpath = diskpath; }
        public String DiskPath { get { return m_diskpath; } }
        public override string Message
        {
            get { return (m_diskpath == null) ? base.Message : "DiskPath=" + m_diskpath; }
        }
    }

    [Serializable]
    public sealed class DiskFullExceptionArgsVersionTwo : ExceptionArgs
    { }
    /*
     * Приемы работы с исключениями
        * Активное использование блоков finally (например для закрытия открытых файлов). 
        Некоторые конструкции, код которых автоматически помещается в блок try, a код очистки в блок finally,
        а именно:
            * lock - внутри блока finally снимается блокировка
            * using - внутри блока finally для объекта вызывается метод Dispose
            * foreach - внутри блока finally для объекта IEnumerator вызывается метод Dispose
            * деструктор - внутри блока finally вызывается метод Finalize базового класса
        * Не надо перехватывать все исключения
            * Допустимо перехватывать исключение, обрабатывать в блоке catch при условии, что в конце этого блока будет повторно сгенерировано исключение
            * Допустим перехват исключения в одном потоке и повторная его генерация в другом
        * Корректное восстановление после исключения
        * Отмена незавершенных операций при невосстановимых исключениях
        * Сокрытие деталей реализации для сохранения контракта
     */

    /*
     * Необоработанные исключения
     * Обнаружив в процессе выполнения поток с необработанным исключением CLR немедленно уничтожает этот поток
     */


    //Метод RuntimeHelpers.PrepareConstrainedRegions(); размещенный перед блоком try, позволяет выполнить загрузку всех типов/статических конструкторов в блоках catch/finally

    /*
     * Контракты - механизм декларативного докумиентирования решений, принятых в ходе проектирования кода, внутри самого метода. Контракты бывают трех типов:
        * предусловия - использутются для проверки аргументов
        * постусловия - используются для проверки состояния завершения метода вне зависимости от того, нормально он завершился или с исключением
        * инваринты = позволяют удостовериться, что данные объекта находятся в хорошем состоянии на протяжении всей жизни этого объекта
     */

    public sealed class Item { /*...*/ }

    public sealed class ShoppingCart {
        private List<Item> m_cart = new List<Item>();
        private Decimal m_totalCost = 0;

        public ShoppingCart() { }
        public void AddItem(Item item) {
            AddItemHelper(m_cart, item, ref m_totalCost);
        }
        private static void AddItemHelper(List<Item> m_cart, Item newItem, ref Decimal totalCost) {
            //Предусловия
            Contract.Requires(newItem != null);
            Contract.Requires(Contract.ForAll(m_cart, s => s != newItem));

            //Постусловия
            Contract.Ensures(Contract.Exists(m_cart, s => s == newItem));
            Contract.Ensures(totalCost >= Contract.OldValue(totalCost));
            Contract.EnsuresOnThrow<IOException>(
                totalCost == Contract.OldValue(totalCost));

            //Какие-то операции, способные вызвать IOException
            m_cart.Add(newItem);
            totalCost += 1.00M;
        }

        //Инавриант
        [ContractInvariantMethod]
        private void ObjectInvariant() {
            Contract.Invariant(m_totalCost >= 0);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                throw new Exception<DiskFullExceptionArgs>(new DiskFullExceptionArgs(@"C:\"), "The disk is full");
            }
            catch (Exception<DiskFullExceptionArgs> e)
            {
                Console.WriteLine(e.Message + Environment.NewLine + e.TargetSite);
            }

        }
    }
}
