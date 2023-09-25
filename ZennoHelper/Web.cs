using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace ZennoHelper
{
    public class Web
    {

        private Instance instance;
        private IZennoPosterProjectModel project;

        public Web(Instance newInstance, IZennoPosterProjectModel newProject)
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
        public virtual HtmlElement GetElement(string xpath, int timeout = 25, int index = 0)
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
        public virtual HtmlElement GetElement(string xpath, string logGood,
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
        /// Проверка существование html-элемента по его XPath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        public virtual bool BoolElement(string xpath, string logGood,
            int timeout = 15, int index = 0, bool showInPosterGood = false)
        {
            DateTime timeoutDT = DateTime.Now.AddSeconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                    return true;
                Thread.Sleep(250);
            }
            return false;
        }

        /// <summary>
        /// Проверка value элемента на указанные данные
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <exception cref="Exception"></exception>
        public void CheckValueElement(HtmlElement element, string text,
            string logGood, bool showInPosterGood = false)
        {
            string valueElement = element.GetValue();
            if (valueElement.Contains(text))
            {
                var log = new Log(instance, project);
                log.LogGoodEnd(logGood, showInPosterGood);
            }
            else
            {
                throw new Exception($"CheckValueElement error: element '{element.GetAttribute("innerHtml")}' c значением '{text}' не найден");
            }
        }

        /// <summary>
        /// Вставка value в элемент с проверкой
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="emulation"></param>
        /// <param name="addend"></param>
        /// <param name="showInPosterGood"></param>
        public virtual void SetValue(HtmlElement element, string text,
            string logGood, string emulation = "None",
            bool addend = false, bool showInPosterGood = false)
        {
            element.SetValue(text, emulation, append: addend);

            CheckValueElement(element, text, logGood, showInPosterGood);
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
        public virtual void SetValue(string xpath, string text, string logGood,  
            string emulation = "None", bool addend = false, int index = 0, bool showInPosterGood = false)
        {
            HtmlElement element = GetElement(xpath, 1, index);

            element.SetValue(text, emulation, append: addend);

            CheckValueElement(element, text, logGood, showInPosterGood);
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
        public virtual HtmlElement NavigateWithoutTry(string url, string xpath,
            string logGood, string referrer = null, int timeout = 25, int index = 0,
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
        public virtual HtmlElement NavigateWithTry(string url, string xpath,
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

        /// <summary>
        /// Вставка value в ещё не найденный элемент без проверки
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="emulation"></param>
        /// <param name="addend"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        public virtual void SetValueWithoutCheck(string xpath, string text, string logGood,
            string emulation = "None", bool addend = false, int index = 0, bool showInPosterGood = false)
        {
            HtmlElement element = GetElement(xpath, 1, index);

            element.SetValue(text, emulation, append: addend);
            var log = new Log(instance, project);
            log.LogGoodEnd(logGood, showInPosterGood);

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
        public virtual void SetValueWithoutCheck(HtmlElement element, string text,
            string logGood, string emulation = "None",
            bool addend = false, bool showInPosterGood = false)
        {
            element.SetValue(text, emulation, append: addend);
            var log = new Log(instance, project);
            log.LogGoodEnd(logGood, showInPosterGood);
        }


    }
}
