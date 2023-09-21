using System;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.Emulation;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelper
{
    public class WebPhone : Web
    {
       
        private Instance instance;
        private IZennoPosterProjectModel project;

        public WebPhone(Instance newInstance, IZennoPosterProjectModel newProject) : base(newInstance, newProject)
        {
            instance = newInstance;
            project = newProject;
        }
        /// <summary>
        /// Получение html-элемента по его XPath с вызовом исключения в случае не нахождения
        /// </summary>
        /// <param name="xpath">>Путь XPath для элемента</param>
        /// <param name="timeout">Кол-во времени для ожидания элемента</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override HtmlElement GetElement(string xpath, int timeout = 25, int index = 0)
        {
            return base.GetElement(xpath, timeout, index);
        }
        /// <summary>
        /// Проверка существование html-элемента по его XPath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        public override bool BoolElement(string xpath, string logGood, int timeout = 15, int index = 0, bool showInPosterGood = false)
        {
            return base.BoolElement(xpath, logGood, timeout, index, showInPosterGood);
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
        public override HtmlElement GetElement(string xpath, string logGood,
            int timeout = 15, int index = 0, bool showInPosterGood = false)
        {
            return base.GetElement(xpath, logGood, timeout, index, showInPosterGood);
        }

        /// <summary>
        /// Нажатие по элементу через поиск по xpath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        public void Touch(string xpath, int timeout = 25, int index = 0)
        {
            HtmlElement element = GetElement(xpath, timeout, index);
            Touch(element);
        }
        /// <summary>
        /// Нажатие по элементу, который уже найден
        /// </summary>
        /// <param name="element"></param>
        public void Touch(HtmlElement element)
        {
            instance.ActiveTab.Touch.Touch(element);
        }
        /// <summary>
        ///  Установка value элемента через эмуляцию клавиатуры, с предварительным нажатием на него и проверкой после установки
        /// </summary>
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="index"></param>
        /// <param name="latency">Задержка между вводимыми символами</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
		public void SetValueFull(string xpath, string text,
            string logGood, int index = 0, int latency = 20, bool showInPosterGood = false)
        {
            HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
            Touch(element);
            instance.SendText(text, latency);

            var web = new Web(instance, project);
            web.CheckValueElement(element, text, logGood, showInPosterGood);
        }
        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры, с предварительным нажатием на него и проверкой после установки
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="latency">Задержка между вводимыми символами</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        public void SetValueFull(HtmlElement element, string text,
            string logGood, int latency = 20, bool showInPosterGood = false)
        {
            Touch(element);
            instance.SendText(text, latency);

            var web = new Web(instance, project);
            web.CheckValueElement(element, text, logGood, showInPosterGood);
        }

        /// <summary>
        /// Вставка value в элемент с проверкой
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="emulation"></param>
        /// <param name="addend"></param>
        /// <param name="showInPosterGood"></param>
        public override void SetValue(HtmlElement element, string text,
            string logGood, string emulation = "None",
            bool addend = false, bool showInPosterGood = false)
        {
            base.SetValue(element,text, logGood, emulation, addend, showInPosterGood);
        }
        /// <summary>
        /// Вставка value в ещё не найденный элемент с проверкой
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="emulation"></param>
        /// <param name="addend"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        public override void SetValue(string xpath, string text, string logGood, string emulation = "None", bool addend = false, int index = 0, bool showInPosterGood = false)
        {
            base.SetValue(xpath, text, logGood, emulation, addend, index, showInPosterGood);
        }


        /// <summary>
        /// Переход на сайт с проверкой загрузки через GetElement и выводом сообщения в лог
        /// </summary>
        /// <param name="url">Ссылка для перехода</param>
        /// <param name="xpath">Элемент, по которому будет определяться загрузка сайта</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="referrer">Ссылка с которой якобы совершён переход на url</param>
        /// <param name="timeout">Кол-во времени для ожидания элемента, по которому проверяется загрузка url</param>
        /// <param name="index">Индекс XPath для элемента, по которому проверяется загрузка url</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод ошибки в ЗенноПостер</param>
        /// <returns></returns>
        public override HtmlElement NavigateWithoutTry(string url, string xpath,
            string logGood, string referrer, int timeout = 25, int index = 0,
            bool showInPosterGood = false)
        {
            return base.NavigateWithoutTry(url, xpath, logGood, referrer, timeout, index, showInPosterGood);
        }
        /// <summary>
        /// Переход на сайт с проверкой загрузки через GetElement с 3 попытками и выводом сообщения в лог
        /// </summary>
        /// <param name="url">Ссылка для перехода</param>
        /// <param name="xpath">Элемент, по которому будет определяться загрузка сайта</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="referrer">Ссылка с которой якобы совершён переход на url</param>
        /// <param name="timeout">Кол-во времени для ожидания элемента, по которому проверяется загрузка url</param>
        /// <param name="index">Индекс XPath для элемента, по которому проверяется загрузка url</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод ошибки в ЗенноПостер</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override HtmlElement NavigateWithTry(string url, string xpath,
            string logGood, string referrer, int timeout = 25, int index = 0,
            bool showInPosterGood = false)
        {
            return base.NavigateWithTry(url, xpath, logGood, referrer, timeout, index, showInPosterGood);
        }
    }
}
