using Events;
using EventTrackingType;
using ExplicitEventLoggingControl;
using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Разработка типа, поддерживающего события:
 * Этап 1. Определение типа для хранения всей дополнительной информации, передаваемой получателям уведомлеия о событии
    * В соответствии с соглашением, классы, содержащие информацию о событиях, передаваемую обработчику события должны наследоваться от System.EventArgs и должны заканчиваться на EventArgs 
 * Этап 2. Определение члена-события
    * Каждому члену-события назначаются область действия, тип делегата, указывающий на прототип вызывающего метода и имя
 * Этап 3. Определение метода, ответственного за уведомление зарегестрированных объектов о событии
 * Этап 4. Определение метода, преобразующего входную информацию в желаемое событие
 */

namespace Events {
    //Этап 1. Определение типа для хранения всей дополнительной информации, передаваемой получателям уведомлеия о событии
    internal class NewMailEventArgs : EventArgs {
        private readonly String m_from, m_to, m_subject;
        public NewMailEventArgs(String from, String to, String subject) {
            m_from = from; m_to = to; m_subject = subject;
        }
        public String From { get { return m_from; } }
        public String To { get { return m_to; } }
        public String Subject { get { return m_subject; } }
    }

    internal class MailManager {

        //Этап 2. Определение члена-события
        public event EventHandler<NewMailEventArgs> NewMail;

        //Этап 3. Определение метода, ответственного за уведомление зарегестрированных объектов о событии
        protected virtual void OnNewMail(NewMailEventArgs e) {

            //Сохраняем ссылку во временной переменной для обеспечения безопасности потока
            EventHandler<NewMailEventArgs> temp = Volatile.Read(ref NewMail);

            //Если есть объекты, подписанные на уведомление о событии - уведомляем их
            if (temp != null) temp(this, e);
        }

        //Этап 4. Определение метода, преобразующего входную информацию в желаемое событие
        public void SimulateNewMail(String from, String to, String subject) {

            //Создаем объект для хранения информации
            NewMailEventArgs e = new NewMailEventArgs(from, to, subject);

            //Вызываем виртуальный метод, уведомляющий объект о событии
            OnNewMail(e);
        }
    }

    /*
     * Строка public event EventHandler<NewMailEventArgs> NewMail; преобразуется в:
     * 1. ЗАКРЫТОЕ поле делегата, инициализированное значением null
     * 2. ОТКРЫТЫЙ метод add_Xxx - позволяющий объектам регистрироваться для получения уведомлений о событии
     * 3. ОТКРЫТЫЙ метод remove_Xxx - позволяющий объектам отменять регистрацию 
     */
}

//Создание типа отслеживающего событие

namespace EventTrackingType {
    internal sealed class Fax {
        public Fax(MailManager mm) {

            //Регистрируем обратный вызов для события NewMail
            mm.NewMail += FaxMsg;           
        }

        private void FaxMsg(Object sender, NewMailEventArgs e) {

            //Симуляция отправки факса
            Console.WriteLine("Faxing mail message:");
            Console.WriteLine("From={0}, To={1}, Subject={2}", e.From, e.To, e.Subject);
        }

        public void Unregister(MailManager mm) {
            
            //Отписываемся от уведомления о событии NewMail
            mm.NewMail -= FaxMsg;
        }
    }
}

namespace ExplicitEventLoggingControl {

    //Класс для безопасноти типов
    public sealed class EventKey : Object { }

    public sealed class EventSet {
        
        //Отображение EventKey в Delegate
        private readonly Dictionary<EventKey, Delegate> m_events = new Dictionary<EventKey, Delegate>();

        //Добавление и компоновка делегата с существующим ключом EventKey
        public void Add(EventKey eventKey, Delegate handler) {
            Monitor.Enter(m_events);
            Delegate d;
            m_events.TryGetValue(eventKey, out d);
            m_events[eventKey] = Delegate.Combine(d, handler);
            Monitor.Exit(m_events);
        }

        //Удаление делегата из EventKey
        public void Remove(EventKey eventKey, Delegate handler) {
            Monitor.Enter(m_events);
            Delegate d;
            if (m_events.TryGetValue(eventKey, out d)) {
                d = Delegate.Remove(d, handler);
                if (d != null) m_events[eventKey] = d;
                else m_events.Remove(eventKey);
            }
        }

        //Информирование о событии обозначенного ключа EventKey
        public void Raise(EventKey eventKey, Object sender, EventArgs e) {
            Delegate d;
            Monitor.Enter(m_events);
            m_events.TryGetValue(eventKey, out d);
            Monitor.Exit(m_events);

            if (d != null) {
                d.DynamicInvoke(new Object[] { sender, e });
            }
        }
    }

    //Этап 1.
    public class FooEventArgs : EventArgs { }

    public class TypeWithLotsOfEvents {
        
        //Коллекция событий
        private readonly EventSet m_eventSet = new EventSet();

        //Свойство защищенное для получения событий
        protected EventSet EventSet { get { return m_eventSet; } }

        //Уникальный ключ
        protected static readonly EventKey s_fooEventKey = new EventKey();

        //Этап 2.
        //Добавление удаление делегата из коллекции
        public event EventHandler<FooEventArgs> Foo {
            add { m_eventSet.Add(s_fooEventKey, value); }
            remove { m_eventSet.Remove(s_fooEventKey, value); }
        }

        //Этап 3.
        protected virtual void OnFoo(FooEventArgs e) {
            m_eventSet.Raise(s_fooEventKey, this, e);
        }

        //Этап 4.
        public void SimulateFoo() {
            OnFoo(new FooEventArgs());
        }
    }
}

namespace ChapterXI.Events
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test part one
            var a = new MailManager();                
            var b = new Fax(a);                         
            a.SimulateNewMail("Me", "You", "Hello!");
            b.Unregister(a);
            a.SimulateNewMail("Me", "You", "Bye!");

            //Test part two
            var twle = new TypeWithLotsOfEvents();
            twle.Foo += HandlerFooEvent;
            twle.SimulateFoo();
        }
        private static void HandlerFooEvent(Object sender, FooEventArgs e) {
            Console.WriteLine("Handling Foo Event here...");
        }
    }
}
