using System;
using System.IO;
using System.Text;

namespace ChapterXVII.Delegates
{
    public sealed class Program
    {
        private static void StaticDelegateDemo()
        {
            Console.WriteLine("----- Static Delegate Demo -----");
            Counter(1, 3, null);
            Counter(1, 3, new Feedback(FeedbackToConsole));
            Counter(1, 3, new Feedback(FeedbackToMsgBox));

            Console.WriteLine();
        }
        private static void InstanceDelegateDemo()
        {
            Console.WriteLine("----- Instance Delegate Demo -----");
            Program p = new Program();
            Counter(1, 3, p.FeedbackToFile);

            Console.WriteLine();
        }
        private static void ChainDelegateDemo1(Program program)
        {
            Console.WriteLine("----- Chain Delegate Demo 1 -----");
            Feedback feedback1 = new Feedback(FeedbackToConsole);
            Feedback feedback2 = new Feedback(FeedbackToMsgBox);
            Feedback feedback3 = new Feedback(program.FeedbackToFile);

            Feedback feedbackChain = null;
            feedbackChain = (Feedback)Delegate.Combine(feedbackChain, feedback1);
            feedbackChain = (Feedback)Delegate.Combine(feedbackChain, feedback2);
            feedbackChain = (Feedback)Delegate.Combine(feedbackChain, feedback3);
            Counter(1, 2, feedbackChain);

            Console.WriteLine();
            feedbackChain = (Feedback)Delegate.Remove(feedbackChain, new Feedback(FeedbackToMsgBox));
            Counter(1, 2, feedbackChain);
            Console.WriteLine();
        }
        private static void ChainDelegateDemo2(Program program)
        {
            Console.WriteLine("----- Chain Delegate Demo 2 -----");
            Feedback feedback1 = new Feedback(FeedbackToConsole);
            Feedback feedback2 = new Feedback(FeedbackToMsgBox);
            Feedback feedback3 = new Feedback(program.FeedbackToFile);

            Feedback feedbackChain = null;
            feedbackChain += feedback1;
            feedbackChain += feedback2;
            feedbackChain += feedback3;
            Counter(1, 2, feedbackChain);

            Console.WriteLine();
            feedbackChain -= (Feedback)Delegate.Remove(feedbackChain, new Feedback(FeedbackToMsgBox));
            Counter(1, 2, feedbackChain);
        }
        private static void Counter(Int32 from, Int32 to, Feedback feedback)
        {
            for (Int32 val = from; val <= to; val++)
            {
                if (feedback != null)
                    feedback(val);
            }
        }
        private static void FeedbackToConsole(Int32 value)
        {
            Console.WriteLine("Item=" + value);
        }

        private static void FeedbackToMsgBox(Int32 value)
        {
            Console.WriteLine("Item=" + value + "->MsgBox");
        }
        private void FeedbackToFile(Int32 value)
        {
            Console.WriteLine("Item=" + value + "->File");
        }

        public static void M(string[] args)
        {
            StaticDelegateDemo();
            InstanceDelegateDemo();
            ChainDelegateDemo1(new Program());
            ChainDelegateDemo2(new Program());
        }
        public delegate void Feedback(Int32 value);
        /*
         * Строка выше застваляет компилятор создать полное оределение класса, которое выглядит так:
         
         internal class Feedback : System.MulticastDelegate {
            
            Конструктор
            public Feedback(Object object, IntPtr method);

            Метод, прототип которого задан в определении делегата
            public virtual void Invoke(Int32 value);

            Методы обеспечивающие асинхронный обратный вызов
            public virtual IAsyncResult BeginInvoke(Int32 value, AsyncCallback callback, Object object);

            public virtual void EndInvoke(IAsyncResult result);
        }
        * Три саиых важных поля, наследуемые от System.MulticastDelegate:
            * System.Object _target - если делегат является оболочкой статического метода, то это поле равно null, 
            если делегат является оболочкой экземплярного метода, то это поле ссылается на объект, с которым будет работать метод обратного вызова
            * System.IntPtr _methodPtr - внутреннее целочисленное значение, используемое CLR для идентификации методов обратного вызова
            * System.Object _invocationList - это поле обычно равно null, оно может ссылаться на массив делегатов при построении цепочки делегатов
        */
        internal sealed class Light
        {
            public String SwitchPosition() { return "The light is off"; }
        }
        internal sealed class Fan
        {
            public String Speed() { throw new InvalidOperationException("The fan broke due to oveheating"); }
        }
        internal sealed class Speaker {
            public String Volume() { return "The volume is loud"; }
        }
        public delegate String GetStatus();
        public static void Main() {
            GetStatus getStatus = null;
            getStatus += new GetStatus(new Light().SwitchPosition);
            getStatus += new GetStatus(new Fan().Speed);
            getStatus += new GetStatus(new Speaker().Volume);
            Console.WriteLine(GetComponentStatusReport(getStatus));
        }
        private static String GetComponentStatusReport(GetStatus status) {          //Метод, позволяющий не прерывать выполнение цепочки делегатов, 
            if (status == null) return null;                                        //если что-то пошло не так с одним из них

            StringBuilder report = new StringBuilder();

            Delegate[] aarayOfDelegates = status.GetInvocationList();

            foreach (GetStatus getStatus in aarayOfDelegates) {
                try {
                    report.AppendFormat("{0}{1}{1}", getStatus(), Environment.NewLine);
                }
                catch (InvalidOperationException e) {
                    Object component = getStatus.Target;
                    report.AppendFormat(
                        "Failde to get status from {1}{2}{0} Error: {3}{0}{0}",
                        Environment.NewLine,
                        ((component == null) ? "" : component.GetType() + "."),
                        getStatus.Method.Name,
                        e.Message);
                }
            }
            return report.ToString();
        }

        /*
         * В пространстве имен System определено 17 делегатов, принимающих до 16 аргументов:
            * public delegate void Action();
            * public delegate void Action<T>(T obj);
              ...
         * А так же 17 делегатов, возвращающих значение:
            * public delegate TResult Func<TResult>();
            * public delegate TResult Func<T, TResult>(T obj);
              ...
         */

        /*
         * Упрощенный синтаксис работы с делегатами:
            * 1. Не создаем объект делегата
            * 2. Не определяем метод обратного вызова -> Лямбда-выражения
                * Лямбда выражения представлляют собой (T1 arg1, ... TN argN) => { M инструкций, содеращих от 0 до N аргументов, определенных слева от "=>"}
            * 3. Не создаем обертку для локальных переменных для передачи их методу обратного вызова
         */
    }
}
