using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

namespace ChapterXXIV.Serialization
{
    internal static class QuickStart {
        private static Object DeserializeRfomMemory(Stream stream) {
            //Задание форматирования при сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            //Заставляем модуль форматирования десериализовать объекты из потока
            return formatter.Deserialize(stream);
        }
        private static MemoryStream SerializeToMemory(Object objectGraph) {
            //Конструирование потока, который будет содержать сериализованные объекты
            MemoryStream stream = new MemoryStream();

            //Задание форматирования при сериализации
            BinaryFormatter formatter = new BinaryFormatter();

            //Заставляем модуль форматирование сериализовать объекты в поток
            formatter.Serialize(stream, objectGraph);

            //Возвращение потока сериализованных объектов вызывающему методу
            return stream;
        }
        public static void SerializeAndDeserialize() {
            //Создание графа объектов для последующей сериализации в птоко
            var objectGraph = new List<String> { "Nikita", "Baranov", "2000/07/12" };

            Stream stream = SerializeToMemory(objectGraph);

            //Обнуляем все для данного примера
            stream.Position = 0;
            objectGraph = null;

            //Десериализация объектов и проверка их работоспособности
            objectGraph = (List<String>)DeserializeRfomMemory(stream);
            foreach (var s in objectGraph) Console.WriteLine(s);
        }

        //Пример метода, использующего сериализацию для создании глубокой копии(клона) объекта
        private static object DeepClone<T>(T original) {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Context = new StreamingContext(StreamingContextStates.Clone);

                formatter.Serialize(memoryStream, original);

                memoryStream.Position = 0;

                return (T)formatter.Deserialize(memoryStream);
            }
        }
        internal struct Point {
            public Int32 x, y;
        }
        private static void OptInSerialization() {
            Point pt = new Point { x = 1, y = 2 };
            using (var stream = new MemoryStream()) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, pt);                    //SerializationException, т.к. Point не имеет атрибута, позволяющего его сериализовать
            }
        }
        [Serializable]
        internal class Circle
        {
            private Double m_radius;

            [NonSerialized]
            private Double m_area;          //Помечен, как несериализуемый
            public Circle(Double radius) {
                m_radius = radius;
                m_area = Math.PI * m_radius * m_radius;
            }
            [OnDeserialized]
            private void OnDeserialized(StreamingContext context) {
                m_area = Math.PI * m_radius * m_radius;
            }
        }

        [Serializable]
        internal class Base {
            protected String m_name = "Nikita";
            public Base() { /*.*/ }
        }
        //Пример реализации метода GetObjectData интерфейса ISerializable и его конструктора,
        //обеспечивающего сериализацию полей базового типа
        internal class Derived : Base, ISerializable {
            private DateTime m_date = DateTime.Now;
            public Derived() { /*.*/ }
            [SecurityPermissionAttribute(
                SecurityAction.Demand, SerializationFormatter = true)]
            private Derived(SerializationInfo info, StreamingContext context) {
                Type baseType = this.GetType().BaseType;
                MemberInfo[] mi = FormatterServices.GetSerializableMembers(baseType, context);

                for (Int32 i = 0; i < mi.Length; i++) {
                    FieldInfo fi = (FieldInfo)mi[i];
                    fi.SetValue(this, info.GetValue(baseType.FullName + "+" + fi.Name, fi.FieldType));
                }
                m_date = info.GetDateTime("Date");
            }
            [SecurityPermissionAttribute(
                SecurityAction.Demand, SerializationFormatter = true)]
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
                info.AddValue("Date", m_date);
                Type baseType = this.GetType().BaseType;
                MemberInfo[] mi = FormatterServices.GetSerializableMembers(baseType, context);
                for (Int32 i = 0; i < mi.Length; i++)
                {
                    info.AddValue(baseType.FullName + "+" + mi[i].Name,
                        ((FieldInfo)mi[i]).GetValue(this));
                }
            }
            public override String ToString()
            {
                return String.Format("Name={0}, Date={1}", m_name, m_date);
            }
        }
        [Serializable]
        public sealed class Singleton : ISerializable {
            private static readonly Singleton theOneObject = new Singleton();
            public String Name = "Nikita";
            public DateTime Date = DateTime.Now;

            private Singleton() { }
            public static Singleton GetSingleton() { return theOneObject; }

            [SecurityPermissionAttribute(
                SecurityAction.Demand, SerializationFormatter = true)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
                info.SetType(typeof(SingletonSerializationHelper));
            }

            [Serializable]
            private sealed class SingletonSerializationHelper : IObjectReference
            {
                public object GetRealObject(StreamingContext context)
                {
                    return Singleton.GetSingleton();
                }
            }
            private static void SingltonSerializationTest() {
                Singleton[] a1 = { Singleton.GetSingleton(), Singleton.GetSingleton() };
                Console.WriteLine("Do both elements refer to the same object? " + (a1[0] == a1[1]));

                using (var stream = new MemoryStream()) {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, a1);
                    stream.Position = 0;
                    Singleton[] a2 = (Singleton[])formatter.Deserialize(stream);
                    Console.WriteLine("Do both elements refer to the same object? " + (a2[0] == a2[1]));
                    Console.WriteLine("Do both elements refer to the same object? " + (a1[0] == a2[0]));
                }
            }
        }
        private static void SerializationSurrogateDemo() {
            using (var stream = new MemoryStream()) {
                IFormatter formatter = new BinaryFormatter();

                SurrogateSelector ss = new SurrogateSelector();

                ss.AddSurrogate(typeof(DateTime), formatter.Context, new UniversalToLocalTimeSerializationSurrogate());

                formatter.SurrogateSelector = ss;

                DateTime localTimeBeforeSerialize = DateTime.Now;
                formatter.Serialize(stream, localTimeBeforeSerialize);

                stream.Position = 0;
                Console.WriteLine(new StreamReader(stream).ReadToEnd());

                stream.Position = 0;
                DateTime localTimeAfterSerialize = (DateTime)formatter.Deserialize(stream);

                Console.WriteLine("LocalTimeBeforeSerialize={0}", localTimeBeforeSerialize);
                Console.WriteLine("LocalTimeAfterSerialize{0}", localTimeAfterSerialize);

            }
        }
    }

    internal sealed class UniversalToLocalTimeSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Date", ((DateTime)obj).ToUniversalTime().ToString("u"));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return DateTime.ParseExact(
                info.GetString("Date"), "u", null).ToLocalTime();
        }
    }
}
