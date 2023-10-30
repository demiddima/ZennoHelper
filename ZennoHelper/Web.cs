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

        protected Instance instance;
        protected IZennoPosterProjectModel project;

        public Web(Instance newInstance, IZennoPosterProjectModel newProject)
        {
            instance = newInstance;
            project = newProject;
        }
        /// <summary>
        /// Поиск элемента по XPath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual HtmlElement GetElement(string xpath, string logGood, int timeout =  100,
            int endCycle = 10, int index = 0, bool showInPosterGood = false)
        {
            for (int i = 0; i < endCycle; i++)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    var log = new Log(instance, project);
                    log.LogGoodEnd(logGood, showInPosterGood);
                    return element;
                }
                Thread.Sleep(timeout);
            }
            throw new Exception($"GetElement error: element '{xpath}' не найден за {endCycle} попыток по {timeout} миллисекунд каждая");
        }
        /// <summary>
        /// Получение html-элемента по его атрибуту
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attrName"></param>
        /// <param name="attrValue"></param>
        /// <param name="searchKind"></param>
        /// <param name="logGood"></param>
        /// <param name="endCycle"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual HtmlElement GetElementAttribute(string tag, string attrName, string attrValue,
            string searchKind, string logGood, int timeout = 500, int endCycle = 10, int index = 0, bool showInPosterGood = false)

        {
            for (int i = 0; i < endCycle; i++)
            {
                HtmlElement element = instance.ActiveTab.FindElementByAttribute(tag, attrName, attrValue, searchKind, index);
                if (!element.IsVoid)
                {
                    var log = new Log(instance, project);
                    log.LogGoodEnd(logGood, showInPosterGood);
                    return element;
                }
                Thread.Sleep(timeout);
            }
            throw new Exception($"GetElementAttribute error: element '{tag}[@{attrName} = '{attrValue}']' не найден за {endCycle} попыток по {timeout} миллисекунд каждая");
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
            int timeout = 50, int endCycle = 5, int index = 0, bool showInPosterGood = false)
        {
            for(int i = 0; i < endCycle; i++)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                    return true;
                Thread.Sleep(timeout);
            }
            return false;
        }

        /// <summary>
        /// Проверка value элемента на указанные данные
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="showInPosterGood"></param>
        /// <exception cref="Exception"></exception>
        public virtual void CheckValueElement(HtmlElement element, string text,
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
        public virtual void SetValue(string xpath,string logGoodFind, string text, string logGoodCheck, 
            int timeout = 250, int endCycle = 2, string emulation = "None", bool addend = false, 
            int indexFind = 0, bool showInPosterGoodFind = false, bool showInPosterGoodCheck = false)

        {
            HtmlElement element = GetElement(xpath,logGoodFind,timeout,endCycle, indexFind, showInPosterGoodFind); 

            element.SetValue(text, emulation, append: addend);

            CheckValueElement(element, text, logGoodCheck, showInPosterGoodCheck);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="referrer"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        public virtual HtmlElement NavigateWithoutTry(string url, string xpath,
            string logGood, string referrer = null, int timeout = 10000, int endCycle = 10, int index = 0,
            bool showInPosterGood = false)
        {
            instance.ActiveTab.Navigate(url, referrer);
            HtmlElement element = GetElement(xpath, logGood, timeout, endCycle, index, showInPosterGood);
            return element;
        }
        /// <summary>
        /// Переход на сайт с проверкой загрузки через GetElement с 3 попытками
        /// </summary>
        /// <param name="url"></param>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="referrer"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual HtmlElement NavigateWithTry(string url, string xpath,
            string logGood, string referrer, int timeout = 1000, int endCycle = 10, int index = 0,
            bool showInPosterGood = false)
        {
            HtmlElement element = null;
            for (int i = 0; i < 3; i++)
            {
                instance.ActiveTab.Navigate(url, referrer);

                try
                {
                    element = GetElement(xpath, logGood, timeout, endCycle, index, showInPosterGood);
                    break;
                }
                catch
                {
                    continue;
                }
            }

            if (element != null)
            {
                return element;
            }
            else
            {
                throw new Exception($"NavigateWithTry error: element '{xpath}' не найден при загрузки страницы за 3 попытки по {timeout * endCycle / 1000} секунд");
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
           int timeout = 500, int endCycle = 10, int index = 0, string emulation = "None", bool addend = false, bool showInPosterGood = false)
        {
            HtmlElement element = GetElement(xpath, logGood, timeout, endCycle, index, showInPosterGood);

            element.SetValue(text, emulation, append: addend);
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
