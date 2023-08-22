using System;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelper
{
    public class WebDesktop
    {
         
        public Instance instance;
        public IZennoPosterProjectModel project;

        public WebDesktop(Instance newInstance, IZennoPosterProjectModel newProject)
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
        public HtmlElement GetElement(string xpath, int timeout = 25, int index = 0)
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
        public HtmlElement GetElement(string xpath, string logGood,
            int timeout = 15, int index = 0, bool showInPosterGood = false)
        {
            DateTime timeoutDT = DateTime.Now.AddSeconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    var log = new Log(instance, project);
                    log.LogGoodEnd(logGood, showInPosterGood);
                    return element;
                }
                Thread.Sleep(250);
            }
            throw new Exception($"GetElement error: element '{xpath}' не найден за {timeout} секунд");
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
        /// Установка значения через эмуляцию клавиатуры с помощью ФулКлик по полю ввода c ещё не найденным элементом, проверкой введённого значения и выводом в лог при завершении
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

            element = instance.ActiveTab.FindElementByXPath(xpath, index);

            string valueElement = element.GetValue();
            if (valueElement.Contains(text))
            {
                var log = new Log(instance, project);
                log.LogGoodEnd(logGood, showInPosterGood);
            }
            else
            {
                throw new Exception($"SetValue error: element '{xpath}' c значением '{text}' не найден");
            }
        }

        /// <summary>
        /// Установка значения через вызов метода element.SetValue() html-элемента с выбором уровня эмуляции, проверкой введённого значения и выводом сообщения в лог(Good - определён SendInfoToLog(); Bad - не определён)
        /// </summary>
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <param name="emulation">Уровень эмуляции для ввода: None, Middle, Full, SuperEmulation</param>
        /// <param name="addend">false - ввод нового значения, true - дописывать значение</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        /// <exception cref="Exception"></exception>
        public void SetValue(string xpath, string text,
            string logGood, int index = 0, string emulation = "None",
            bool addend = false, bool showInPosterGood = false)
        {
            HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
            element.SetValue(text, emulation, append: addend);

            element = instance.ActiveTab.FindElementByXPath(xpath, index);

            string valueElement = element.GetValue();
            if (valueElement.Contains(text))
            {
                var log = new Log(instance, project);
                log.LogGoodEnd(logGood, showInPosterGood);
            }
            else
            {
                throw new Exception($"SetValue error: element '{xpath}' c значением '{text}' не найден");
            }

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
        public HtmlElement NavigateWithoutTry(string url, string xpath,
            string logGood, string referrer, int timeout = 25, int index = 0,
            bool showInPosterGood = false)
        {
            instance.ActiveTab.Navigate(url, referrer);
            HtmlElement element = GetElement(xpath, logGood, timeout, index, showInPosterGood);
            return element;
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
        public HtmlElement NavigateWithTry(string url, string xpath,
            string logGood, string referrer, int timeout = 25, int index = 0,
            bool showInPosterGood = false)
        {
            HtmlElement element = null;
            for (int i = 0; i < 3; i++)
            {
                instance.ActiveTab.Navigate(url, referrer);

                try
                {
                    element = GetElement(xpath, timeout, index);
                    break;
                }
                catch
                {
                    continue;
                }
            }

            if (element != null)
            {
                var log = new Log(instance, project);
                log.LogGoodEnd(logGood, showInPosterGood);
                return element;
            }
            else
            {
                throw new Exception($"GetElement error: element '{xpath}' не найден за 3 попытки по {timeout} секунд");
            }
        }
    }
}
