﻿using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Wintellect, PublicKey=123123123123213")]
[assembly: InternalsVisibleTo("Microsoft, PublicKey=1231vgavhad3123123213")]

namespace BasicInformationAboutMembersAndTypes
{
    public static class AStaticClass
    {
        public static void AStaticMethod() { }

        public static String AStaticProperty
        {
            get { return s_AStaticField; }
            set { s_AStaticField = value; }
        }
        private static string s_AStaticField;

        public static event EventHandler AStaticEvent;
    }
    public sealed class SomeType
    {
        private class SomeNestedType { }                        //Вложенный тип

        private const Int32 c_SomeConstant = 1;                 //Константа
        private readonly String m_SomeReadOnlyField = "2";      //readonly поле
        private static Int32 s_SomereadWriteField = 3;          //Статическое изменяемое поле

        static SomeType() { }                                   //Конструктор типа

        public SomeType(Int32 x) { }                            //Конструкторы экземпляров
        public SomeType() { }

        public String InstanceMethod() { return null; }         //Метод
        public void Main() { }

        public Int32 SomeProp {                                 //Свойство
            get { return 0; }
            set { }
        }

        public event EventHandler SomeEvent;                    //Событие

    }
    class Program
    {
        static void Main(string[] args)
        {
                        //Члены типа

            /*
             * В типе можно определить следующие члены:
             * Константа
             * Поле
             * Конструктор экземпляров
             * Конструктор типа
             * Метод
             * Перегруженный оператор
             * Оператор преобразования
             * Свойство
             * Событие
             * Тип
            */


                        //Видимость типа

            /*
             * puublic - доступен из любой сборки
             * internal - только из сборки, в которой оперделён
             * по умолчанию используется internal
             */


                        //Дружественные сборки

            //Выше указан способ сделать эту сборку дружественной для Microsoft, Wintellect по соответствующим публичным ключам


                        //Доступ к членам типов

            /*
             * Модификаторы доступа к члену:
             * private - доступен только методам, определённым в типе и вложенных в него типах
             * protected - доступен только методам в определяющем типа (и вложенных в него типах) или в одном из его производных типов, независимо от сборок
             * internal - доступен только методам в поределяющей сборке
             * protected internal - доступен только методам в определяющем типа (и вложенных в него типах) или в одном из его производных типов в определяющей сборке
             * public - доступен всем
             */


                        //Статические классы

            /*
             * Статический класс:
             * 1. Прямой потомок System.Object
             * 2. Не реализует никаких интерфейсов, т.к. методы интерфейсы могут быть вызваны только через экземпляры класса
             * 3. Определяет только статические члены (поля, методы, свойства и события)
             * 4. Не может быть использован в качестве поля, параметра, метода или локальной переменной
             */


            //Частичные классы, структуры и интерфейсы

            /*
             * Объявляются ключевым словом partial и могут располагаться в нескольких файлах
             */


                        //Ключевые слова С# и их влияние на управление версиями компонентов

            /*
             * abstract: 
                    * 1) Экземпляр такого типа создавать нельзя
                    * 2) Член такого типа необходимо переопределить и реализовать в производном типе - только после этого можно создавать экземпляры данного типа
                    * 3) Константы и поля не могут иметь такой модификатор
             * virtual:
                    * 1) Экземпляр такого типа создавать нельзя
                    * 2) Член может переопределяться в производном типе
                    * 3) Константы и поля не могут иметь такой модификатор  
             * override:
                    * 1) Экземпляр такого типа создавать нельзя
                    * 2) Член переопределяется в производном типе
                    * 3) Константы и поля не могут иметь такой модификатор
             * sealed:
                    * 1) Тип нельзя использовать в качестве базового при наследовании
                    * 2) Член такого типа нельзя переопределить в производном типе. Это ключевое слово может применяться только к методу, переопределяющему виртуальный метод
                    * 3) Константы и поля не могут иметь такой модификатор
             * new: 
                    * Применяется к вложенному типу, методу, свойству, событию, константе или полю и означает, что член никак не связан с похожим членом, который может существовать в базовом классе
             */


                        //Вызов виртуальных методов, свойств и событий в CLR

            /*
             * call - инструкция, используемая для вызова статических, экземплярныхи и вирутальных методов. Для вызова статических методов, необходима ссылка на тип, для других - на объект памяти, хранящий переменную.
             * Не производится проверка на null
             * callvirt - инструкция, используемая для вызова экземплярныхи и вирутальных методов. Для вызова необходима ссылка на объект памяти, хранящий переменную. Производится проверка на null.
             */


                        //Разумное использование видимости и модификаторов доступа к членам

            /*
             * При проектировании классов:
                1. Запечатывать, если не предназначен для наследования. Внутренний, если нет необходимости передавать другим сборкам.
                2. Поля - закрытые.
                3. Методы закрытые и невиртуальные.
                4. Создание новых вложенных типов, при решении сложных задач.
             * 1-4 выполнять, 
             */
        }
    }
}
