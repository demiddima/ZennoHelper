using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelper
{
    public static class WebPhone
    {

        public static Instance instance;
        public static IZennoPosterProjectModel project;
        /// <summary>
        /// Получение html-элемента по его XPath с вызовом исключения в случае не нахождения
        /// </summary>
        /// <param name="xpath">>Путь XPath для элемента</param>
        /// <param name="timeout">Кол-во времени для ожидания элемента</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static HtmlElement GetElement(string xpath, int timeout = 25, int index = 0)
        {
            DateTime timeoutDT = DateTime.Now.AddSeconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                    return element;
                Thread.Sleep(250);
            }
            throw new Exception($"GetElement error: element '{xpath}' не найден за {timeout} секунд");
        }

        /// <summary>
        /// Получение html-элемента по его XPath с вызовом исключения в случае не нахождения, и выводом сообщений в лог
        /// </summary>
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="timeout">Кол-во времени для ожидания элемента</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static HtmlElement GetElement(string xpath, string logGood,
            int timeout = 15, int index = 0, bool showInPosterGood = false)
        {
            DateTime timeoutDT = DateTime.Now.AddSeconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    Log.LogGoodEnd(logGood, showInPosterGood);
                    return element;
                }
                Thread.Sleep(250);
            }
            throw new Exception($"GetElement error: element '{xpath}' не найден за {timeout} секунд");
        }

        /// <summary>
        /// Нажатие по элементу через поиск по xpath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        public static void Touch(string xpath, int timeout = 25, int index = 0)
        {
            HtmlElement element = GetElement(xpath, timeout, index);
            Touch(element);
        }
        /// <summary>
        /// Нажатие по элементу, который уже найден
        /// </summary>
        /// <param name="element"></param>
        public static void Touch(HtmlElement element)
        {
            instance.ActiveTab.Touch.Touch(element);
        }

        /// <summary>
		/// Печатание value элемента через поиск по xpath, с предварительным нажатием на него
		/// </summary>
		/// <param name="xpath"></param>
		/// <param name="text"></param>
		/// <param name="timeout"></param>
		/// <param name="index"></param>
		public static void SetValue(string xpath, string text, int timeout = 25, int index = 0)
        {
            Touch(xpath, timeout, index);
            instance.SendText(text, 20);
        }

        /// <summary>
        /// Печатание value уже найденном элементу, с предварительным нажатием на него
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        public static void SetValue(HtmlElement element, string text)
        {
            Touch(element);
            instance.SendText(text, 20);
        }
    }
}
