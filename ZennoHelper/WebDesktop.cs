using System;
using System.IO;
using System.Reflection;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.Emulation;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelper
{
    public class WebDesktop : Web
    {
         
        public Instance instance;
        public IZennoPosterProjectModel project;

        public WebDesktop(Instance newInstance, IZennoPosterProjectModel newProject) : base(newInstance, newProject)
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
        /// ФулКлик мышью по уже найденному html-элементу с предварительной наводкой на него
        /// </summary>
        /// <param name="element">HtmlElement</param>
        public void FullClick(HtmlElement element)
        {
            instance.ActiveTab.FullEmulationMouseMoveToHtmlElement(element);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");
        }
        /// <summary>
        /// ФулКлик мышью по не найденному html-элементу с предварительной наводкой на него
        /// </summary>
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="index">Индекс XPath для элемента</param>
        public void FullClick(string xpath, int index = 0)
        {
            HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
            instance.ActiveTab.FullEmulationMouseMoveToHtmlElement(element);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");
        }
        /// <summary>
        /// ФулКлик по координатам
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void FullClick(int x, int y)
        {
            instance.ActiveTab.FullEmulationMouseMove(x, y);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");

        }
        /// <summary>
        /// Клик мышью по уже найденному html-элементу с выбором уровня эмуляции
        /// </summary>
        /// <param name="element">HtmlElement</param>
        /// <param name="emulation">Уровень эмуляции: None, Middle, Full, SuperEmulation</param>
        public void Click(HtmlElement element, string emulation = "None")
        {
            element.RiseEvent("click", emulation);
        }

        /// <summary>
        /// Клик мышью по не найденному html-элементу с выбором уровня эмуляции
        /// </summary>
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <param name="emulation">Уровень эмуляции: None, Middle, Full, SuperEmulation</param>
        public void Click(string xpath, int index = 0, string emulation = "None")
        {
            HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
            element.RiseEvent("click", emulation);
        }

        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры с помощью ФулКлик по полю ввода ещё не найденного элемента
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="latency">Задержка между вводимыми символами</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        /// <exception cref="Exception"></exception>
        public void SetValueFull(string xpath, string text,
            string logGood, int latency = 0, int index = 0, bool showInPosterGood = false)
        {
            HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
            FullClick(element);
            instance.SendText(text, latency);

            var web = new Web(instance, project);
            web.CheckValueElement(element, text, logGood, showInPosterGood);
        }
        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры с помощью ФулКлик по полю ввода уже найденного элемента
        /// <param name="element"></param>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="latency">Задержка между вводимыми символами</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        /// <exception cref="Exception"></exception>
        public void SetValueFull(HtmlElement element, string text,
            string logGood, int latency = 0, bool showInPosterGood = false)
        {
            FullClick(element);
            instance.SendText(text, latency);

            var web = new Web(instance, project);
            web.CheckValueElement(element, text, logGood, showInPosterGood);
        }

        /// <summary>
        /// Вставка value в элемент с проверкой
        /// </summary>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="emulation">Уровень эмуляции для ввода: None, Middle, Full, SuperEmulation</param>
        /// <param name="addend">false - ввод нового значения, true - дописывать значение</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        /// <exception cref="Exception"></exception>
        public override void SetValue(HtmlElement element, string text,
            string logGood, string emulation = "None",
            bool addend = false, bool showInPosterGood = false)
        {
            base.SetValue(element, text, logGood, emulation, addend, showInPosterGood);
         
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
        /// Вставка value в уже найденный элемент без проверки
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="emulation"></param>
        /// <param name="addend"></param>
        /// <param name="showInPosterGood"></param>
        public override void SetValueWithoutCheck(HtmlElement element, string text, string logGood, string emulation = "None", bool addend = false, bool showInPosterGood = false)
        {
            base.SetValueWithoutCheck(element, text, logGood, emulation, addend, showInPosterGood);
        }
        /// <summary>
        /// Вставка value c ещё не найденный элемент без проверки
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="emulation"></param>
        /// <param name="addend"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        public override void SetValueWithoutCheck(string xpath, string text, string logGood, string emulation = "None", bool addend = false, int index = 0, bool showInPosterGood = false)
        {
            base.SetValueWithoutCheck(xpath, text, logGood, emulation, addend, index, showInPosterGood);
        }

        /// <summary>
        /// Переход на сайт с проверкой загрузки через GetElement
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
        /// Переход на сайт с проверкой загрузки через GetElement с 3 попытками
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

        /// <summary>
        /// ФулКлик с ожиданием появления элемента после клика через XPath
        /// </summary>
        /// <param name="fullClickXpath"></param>
        /// <param name="waitElement"></param>
        /// <param name="timeout"></param>
        public void FullClickWithCheck(string fullClickXpath, string waitElement,  int timeout = 5)
        {
            FullClick(fullClickXpath, 0);
            GetElement(waitElement, timeout, 0);
        }
        /// <summary>
        /// ФулКлик с ожиданием появления элемента после клика с уже найденным элементом
        /// </summary>
        /// <param name="clickElement"></param>
        /// <param name="waitElement"></param>
        /// <param name="timeout"></param>
        public void FullClickWithCheck(HtmlElement clickElement, string waitElement, int timeout = 5)
        {
            FullClick(clickElement);
            GetElement(waitElement, timeout, 0);
        }
        /// <summary>
        /// Клик с ожиданием появления элемента после клика через XPath
        /// </summary>
        /// <param name="clickXpath"></param>
        /// <param name="waitElement"></param>
        /// <param name="emulation"></param>
        /// <param name="timeout"></param>
        public void ClickWithCheck(string clickXpath, string waitElement, string emulation = "None", int timeout = 5)
        {
            Click(clickXpath, 0, emulation);
            GetElement(waitElement, timeout, 0);
        }
        /// <summary>
        /// Клик с ожиданием появления элемента после клика с уже найденным элементом
        /// </summary>
        /// <param name="clickElement"></param>
        /// <param name="waitElement"></param>
        /// <param name="timeout"></param>
        /// <param name="emulation"></param>
        public void ClickWithCheck(HtmlElement clickElement, string waitElement, int timeout = 5, string emulation = "None")
        {
            Click(clickElement, emulation);
            GetElement(waitElement, timeout, 0);
        }
    }
}
