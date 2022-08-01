using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace ChapterXXIII.Reflection
{
    public class Something
    {
        /*
         * Методы:
         * System.Reflection.Assembly.Load - принимает в качестве параметра имя сборки и пытается ее выгрузить в текущий домен
         * AppDomain.Load - принимает в качестве параметра имя сборки и пытается ее выгрузить в текущий домен, но требует наличие всех частей идентифицирующих сборку
         * System.Reflection.Assembly.LoadFrom - принимает в качестве параметра путь до сборки пытается ее выгрузить в текущий домен. Может принимать URL в качестве параметра
         * System.Reflection.Assembly.LoadFile - требует регистрации на метод обраного вызова AssemblyResolve
         * System.Reflection.Assembly.ReflectionOnlyLoad - инструмент для анализа метаданных сборки без исполнения кода сборки
         * System.Reflection.Assembly.ReflectionOnlyLoadFrom - инструмент для анализа метаданных сборки без исполнения кода сборки
         */

        //Пример метода обратного вызова для регистрации зависимых сборок на этапе выполнения
        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
        {
            String dllName = new AssemblyName(args.Name).Name + ".dll";

            var assem = Assembly.GetExecutingAssembly();
            String resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));
            if (resourceName == null) return null;

            using (var stream = assem.GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
        /*
         * Недостатки механизма отражений
         * 1. Отстутствие контроля безопасности типов на этапе компиляции
         * 2. Медленная производительность из-за постоянного поиска подстроки (имени типа) в строке (метаданных сборки)
         */

        //Свойство ExportedTypes класса Assembly содержит в себе типы, определенные в сборке assemblyName

        public static void Go()
        {
            LoadAssemblies();

            var allTypes =
                (from a in AppDomain.CurrentDomain.GetAssemblies()
                 from t in a.ExportedTypes
                 where typeof(Object).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo())
                 orderby t.Name
                 select t).ToArray();
            Console.WriteLine(WalkInheritanceHierarchy(new StringBuilder(), 0, typeof(Object), allTypes));
        }
        private static StringBuilder WalkInheritanceHierarchy(
            StringBuilder sb, Int32 indent, Type baseType, IEnumerable<Type> allTypes)
        {
            String spaces = new String(' ', indent * 3);
            sb.AppendLine(spaces + baseType.FullName);
            foreach (var t in allTypes)
            {
                if (t.GetTypeInfo().BaseType != baseType) continue;
                WalkInheritanceHierarchy(sb, indent + 1, t, allTypes);
            }
            return sb;
        }
        private static void LoadAssemblies()
        {
            String[] assemblies = {
                "System,                        PublicKeyToken={0}",
                "System.Core,                   PublicKeyToken={0}",
                "System.Data,                   PublicKeyToken={0}",
                "System.Design,                 PublicKeyToken={1}",
                "System.DirectoryServices,      PublicKeyToken={1}",
                "System.Drawing,                PublicKeyToken={1}",
                "System.Drawing.Design,         PublicKeyToken={1}",
                "System.Management,             PublicKeyToken={1}",
                "System.Messaging,              PublicKeyToken={1}",
                "System.Runtime.Remoting,       PublicKeyToken={0}",
                "System.Security,               PublicKeyToken={1}",
                "System.ServiceProcess,         PublicKeyToken={1}",
                "System.Web,                    PublicKeyToken={1}",
                "System.Web.RegularExpressions, PublicKeyToken={1}",
                "System.web.Services,           PublicKeyToken={1}",
                "System.Xml,                    PublicKeyToken={0}",
            };

            String EcmaPublicKeyToken = "b77a5c561934e089";
            String MSPublicKeyToken = "b03f5f7f11d50a3a";

            Version version = typeof(System.Object).Assembly.GetName().Version;
            foreach (String a in assemblies)
            {
                String AssemblyIdentity =
                    String.Format(a, EcmaPublicKeyToken, MSPublicKeyToken) + ", Culture=neutral, Version=" + version;
                Assembly.Load(AssemblyIdentity);
            }
        }
        /*
         * Механизмы создания экземпляра типа с помощью отражения
         * 1. Метод CreateInstance класса System.Activator
            * Версия метода, передающая объект Type, является более предпочтительной. Передается набор аргументов конструктора и возвращается ссылка на объект
            * Версия метода, передающая объект String, требует еще и строку идентифицирующую сборку
         * 2. Метод CreateInstanceFrom класса System.Activator
            * Необходимо передавать строку с именем типа и сборки, в которой он находится.
            * Так же, как версия метода CreateInstance, принимающего строку, возвращает ссылку на ObjectHandle
         * 3. Методы объекта System.AppDomain
            * CreateInstance
            * CreateInstanceAndUnwrap
            * CreateInstanceFrom
            * CreateInstanceFromAndUnwrap
            * Работают так же как статические методы System.Activator, но позволяют задать домен приложения, в котором нужно создать объект
         * 4. Экземплярный метод Invoke объекта System.Reflection.ConstructorInfo
            * При помощи ссылки на объект TypeInfo можно привязаться к некоторому конструктору и получить ссылку на объект ConstructorInfo,
            чтобы затем вызвать метод Invoke. Новый объект всешда создается в вызывающем домене и возвращается ссылка на новый объект.
         * 5. Для массивов и делегатов есть отдельные методы (выше указаные методы не работают для создания массива/делегата). CreateInstance
         */

        /*
         * Создание прилложениц с поддержкой подключаемых компонентов:
         * 1. Создать сборку хосат, определяющего интерфейс с методами, обеспечивающими взаимодействие приложения с подключаемыми компнентами
         * 2. Разработчики сторонних приложений смогут ссылаться на интерфейсную сборку
         * 3. Создание сборки, в которой определяются типы приложения
         */
        internal sealed class Dictionary<Tkey, TValue> { }
        internal sealed class Temp
        {
            internal static void CreateGenericType()
            {
                //Создание экземпляра обобщенного типа
                //Получаем ссылку на объект Type обобщенного типа
                Type openType = typeof(Dictionary<,>);

                //Закрываем обобщенный тип, используя метод MakeGenericType()
                Type closedType = openType.MakeGenericType(
                    new Type[] { typeof(String), typeof(Int32) });

                //Создание экземпляра закрытого типа
                var o = Activator.CreateInstance(closedType);
                Console.WriteLine(o.GetType());
            }
            public static void PrintMemberInfo()
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly a in assemblies)
                {
                    Show(0, "Assembly: {0}", a);
                    foreach (Type t in a.ExportedTypes)
                    {
                        Show(1, "Type : {0}", t);
                        foreach (MemberInfo mi in t.GetTypeInfo().DeclaredMembers)
                        {
                            String typeName = String.Empty;
                            if (mi is Type) typeName = "(Nested) Type";
                            if (mi is FieldInfo) typeName = "FieldInfo";
                            if (mi is MethodInfo) typeName = "MethodInfo";
                            if (mi is ConstructorInfo) typeName = "ConstructorInfo";
                            if (mi is PropertyInfo) typeName = "PropertyInfo";
                            if (mi is ConstructorInfo) typeName = "ConstructorInfo";
                            if (mi is EventInfo) typeName = "EventInfo";
                            Show(2, "{0} : {1}", typeName, mi);
                        }
                    }
                }
            }
            private static void Show(Int32 indent, String format, params Object[] args)
            {
                Console.WriteLine(new String(' ', 3 * indent) + format, args);
            }
        }
    }
    internal sealed class SomeType
    {
        private Int32 m_someField;
        public SomeType(ref Int32 x) { x *= 2; }
        public override String ToString() { return m_someField.ToString(); }
        public Int32 SomeProp
        {
            get { return m_someField; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value");
                m_someField = value;
            }
        }
        public event EventHandler SomeEvent;
        private void NoCompilerWarnings() { SomeEvent.ToString(); }
    }
    public sealed class Program
    {
        static void Main(string[] args)
        {
            Type t = typeof(SomeType);
            BindToMemberThenInvokeTheMember(t);
            Console.WriteLine();

            BindToMemberCreateDelegateToMemberThenInvokeTheMember(t);
            Console.ReadKey();
        }
        private static void BindToMemberThenInvokeTheMember(Type t) {
            Console.WriteLine("BindToMemberThenInvokeTheMember");

            //Создание экземпляра
            Type ctorArgument = Type.GetType("System.Int32&");  //или typeof(Int32).MakeByRefType();

            ConstructorInfo ctor = t.GetTypeInfo().DeclaredConstructors.First(
                c => c.GetParameters()[0].ParameterType == ctorArgument);
            Object[] args = new Object[] { 12 };                //Аргументы конструктора
            Console.WriteLine("x before constructor called:" + args[0]);
            Object obj = ctor.Invoke(args);
            Console.WriteLine("Type: " + obj.GetType());
            Console.WriteLine("x after constructor returns:" + args[0]);

            //Чтение и запись в поле
            FieldInfo fi = obj.GetType().GetTypeInfo().GetDeclaredField("m_someField");
            fi.SetValue(obj, 33);
            Console.WriteLine("someField: " + fi.GetValue(obj));

            //Вызов метода
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            String s = (String)mi.Invoke(obj, null);
            Console.WriteLine("ToString: " + s);

            //Чтение и запись свойства
            PropertyInfo pi = obj.GetType().GetTypeInfo().GetDeclaredProperty("m_someField");
            try { pi.SetValue(obj, 0, null); }
            catch (TargetInvocationException e) {
                if (e.InnerException.GetType() != typeof(ArgumentOutOfRangeException)) throw;

                Console.WriteLine("Property set catch");
            }
            pi.SetValue(obj, 2, null);
            Console.WriteLine("SomeProp: " + pi.GetValue(obj, null));

            //Добавление и удаление делегата для события
            EventInfo ei = obj.GetType().GetTypeInfo().GetDeclaredEvent("SomeEvent");
            EventHandler eh = new EventHandler(EventCallback);
            ei.AddEventHandler(obj, eh);
            ei.RemoveEventHandler(obj, eh);
        }

        private static void BindToMemberCreateDelegateToMemberThenInvokeTheMember(Type t) {
            Console.WriteLine("BindToMemberCreateDelegateToMemberThenInvokeTheMember");

            //Создание экземпляра
            Object[] args = new Object[] { 12 };                //Аргументы конструктора
            Console.WriteLine("x before constructor called:" + args[0]);
            Object obj = Activator.CreateInstance(t, args);
            Console.WriteLine("Type: " + obj.GetType());
            Console.WriteLine("x after constructor returns:" + args[0]);


            //Нельзя создавать делегата для поля

            //Вызов метода
            
            MethodInfo mi = obj.GetType().GetTypeInfo().GetDeclaredMethod("ToString");
            var toString = mi.CreateDelegate(typeof(Func<String>), obj);
            String s = toString.ToString();
            Console.WriteLine("ToString: " + s);
        }
        private static void EventCallback(Object sender, EventArgs e) { }
    }
}
