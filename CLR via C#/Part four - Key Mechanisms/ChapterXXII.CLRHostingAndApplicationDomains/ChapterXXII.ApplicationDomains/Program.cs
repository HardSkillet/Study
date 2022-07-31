using System;
using System.Reflection;
using System.Threading;
using System.Runtime.Remoting;

/*
 * Домен - логический контейнер для набора сборок
 * Объекты, созданные одним лрменом приложений, недоступны для кода других доменов
 */


namespace ChapterXXII.ApplicationDomains
{
    [System.Runtime.InteropServices.ComVisible(true)]

    public sealed class SomeClass
    {
        private static void Marshalling()
        {
            //Получение ссылки на домен, в котором исполняется вызывающий поток
            AppDomain adCallingThreadDomain = Thread.GetDomain();

            //Каждому домену присваивается значимое имя, облегчающее отладку
            //Получаем имя домена и выводим его


            String callingDomainName = adCallingThreadDomain.FriendlyName;
            Console.WriteLine("Default AppDomain's friendly name={0}", callingDomainName);

            //Получаем и выводим сборку в домене, содержащем метод Main()
            String exeAssembly = Assembly.GetEntryAssembly().FullName;
            Console.WriteLine("Main assembly={0}", exeAssembly);

            //Определяем локальную переменную, ссылающуюся на домен
            AppDomain ad2 = null;

            //Пример 1. Доступ к объектам другого домена приложений с продвижением по ссылке
            Console.WriteLine("{0}Demo#1", Environment.NewLine);

            //Создаем новый домен(с теми же параметрами защиты и конфигурирования)
            ad2 = AppDomain.CreateDomain("AD #2", null, null);
            MarshalByRefType mbrt = null;

            //Загружаем нашу сборку в новый домен, конструируем объект и продивгаем его обратно в наш домен
            //в дейстивтельности мы получаем ссылку на представитель
            mbrt = (MarshalByRefType)
                ad2.CreateInstanceAndUnwrap(exeAssembly, typeof(MarshalByRefType).FullName);

            Console.WriteLine("Type={0}", mbrt.GetType());      //CLR неверно определяет тип

            //Убеждаемся, что получили ссылку на объект-представитель
            Console.WriteLine("IsProxy={0}", RemotingServices.IsTransparentProxy(mbrt));

            //Все выглядит так, как будто мы вызываем метод экзмепляра MarshalByRefType,
            //но на самом деле мы вызываем метод типа представителя. Именно представитель
            //переносит поток в тот домен, в котором находится объект, и вызывает метод для реального объекта
            mbrt.SomeMethod();

            //Выгружаем новый домен
            AppDomain.Unload(ad2);
            //mbrt ссылается на правильный объект-представитель;
            //объект-представитель ссылается на неправильный домен

            try {
                //Вызываем метод, определенный в типе представителя
                //Поскольку домен приложений неправильный, появляется исключение
                mbrt.SomeMethod();
                Console.WriteLine("Successful call.");
            }
            catch (AppDomainUnloadedException){
                Console.WriteLine("Failed call.");
            }

            //Пример 2. Доступ к объектам другого домена с продвижением по значению
            Console.WriteLine("{0}Demo#2", Environment.NewLine);

            //Создаем новый домен с такими же параметрами защиты и конфигурирования, как в текущем
            ad2 = AppDomain.CreateDomain("AD #2", null, null);

            //Загружаем нашу сборку в новый домен, конструируем объект и продвигаем его обратно в наш домен
            //в дейстивтельности мы получаем ссылку на представитель
            mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, typeof(MarshalByRefType).FullName);

            //Метод возвращает копию возвращенного объекта
            //Продвижения объекта происходит по значению, а не по ссылке
            MarshalByValType mbvt = mbrt.MethodWithReturn();

            //Убеждаемся, что мы не получили ссылку на объект представитель
            Console.WriteLine("IsProxy={0}", RemotingServices.IsTransparentProxy(mbvt));

            //Кажется, что мы вызываем метод объекта MarshalByRefType, и это на самом деле так
            Console.WriteLine("Returned object created " + mbvt.ToString());

            //Выгружаем новый домен
            AppDomain.Unload(ad2);
            //mbrt ссылается на действительный объект
            //выгрузка домена не имеет никакого смысла

            try
            {
                //Вызываем метод объекта, исключение не генерируется
                Console.WriteLine("Returned object created " + mbvt.ToString());
                Console.WriteLine("Successful call.");
            }
            catch (AppDomainUnloadedException)
            {
                Console.WriteLine("Failed call.");
            }


            //Пример 3. Доступ к объектам другого домена без использования механизма продвижения
            Console.WriteLine("{0}Demo#3", Environment.NewLine);

            ad2 = AppDomain.CreateDomain("AD #2", null, null);

            //Загружаем нашу сборку в новый домен, конструируем объект и продвигаем его обратно в наш домен
            //в дейстивтельности мы получаем ссылку на представитель
            mbrt = (MarshalByRefType)ad2.CreateInstanceAndUnwrap(exeAssembly, typeof(MarshalByRefType).FullName);

            //Метод возвращает объект, продвижение которого невозможно
            //Генерируется исключение
            NonMarshalableType nmt = mbrt.MethodArgAndReturn(callingDomainName);
        }
        [Serializable]
        public sealed class MarshalByRefType : MarshalByRefObject
        {
            public MarshalByRefType()
            {
                Console.WriteLine("{0} ctor running in {1}",
                    this.GetType().ToString(), Thread.GetDomain().FriendlyName);
            }

            public void SomeMethod()
            {
                Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
            }

            public MarshalByValType MethodWithReturn()
            {
                Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
                MarshalByValType t = new MarshalByValType();
                return t;
            }
            public NonMarshalableType MethodArgAndReturn(String callingDomainName)
            {
                Console.WriteLine("Calling from '{0}' to '{1}'.",
                    callingDomainName, Thread.GetDomain().FriendlyName);
                NonMarshalableType t = new NonMarshalableType();
                return t;
            }
        }
        [Serializable]
        public sealed class MarshalByValType : Object
        {
            private DateTime m_creationTime = DateTime.Now;

            public MarshalByValType()
            {
                Console.WriteLine("{0} ctor running in {1}, Created on {2:D}",
                    this.GetType().ToString(),
                    Thread.GetDomain().FriendlyName,
                    m_creationTime);
            }
            public override String ToString()
            {
                return m_creationTime.ToLongDateString();
            }
        }

        public sealed class NonMarshalableType : Object
        {
            public NonMarshalableType()
            {
                Console.WriteLine("Executing in " + Thread.GetDomain().FriendlyName);
            }
        }
        /*
         * Выгрузка домена методом Upload:
         * 1. Приостанавливает все потоки в процессе, которые когда-либо выполняли управляемый код
         * 2. Генерирует исключение ThreadAbortException, если существуют потоки, выполняющие код в этом домене. Эти потоки переходят к выполнению блоков finally. 
         Если исключение не отловлено, потоки завершают работу, но не процесс!!!
         * 3. СLR выставляет флаги на каждый объект-представителя, который ссылается на объект в выгружаемом домене
         * 4. Генерация принудительной уборки мусора
         * 5. Продолжение работы потоков
         */

        //В С# имеются свойства мониторинга доменов

        public sealed class Program {
            public static void Main() {
                Marshalling();
            }
        }
    }
}
