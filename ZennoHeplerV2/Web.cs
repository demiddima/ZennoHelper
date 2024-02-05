using Global.ZennoExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;
using static System.Net.Mime.MediaTypeNames;

namespace ZennoHelperV2
{
    public class Web : Main
    {

        public Web(Instance instance, IZennoPosterProjectModel project) :base(instance, project)
        {

        }


        #region Базовые методы

        /// <summary>
        /// Получение элемента
        /// </summary>
        /// <param name="xpath">Путь xpath к элементу</param>
        /// <param name="timeout">Время в течении которого элемент будет искаться</param>
        /// <param name="wait">Перерыва между попытками получить элемент</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement GetElement(string xpath, int timeout = 1000, int wait = 250)
        {

            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, 0);
                if (!element.IsVoid)
                {
                    project.SendToLog($"GetElement  confirming: элемент {xpath} получен", LogType.Info, false, LogColor.Blue);
                    return element;
                }
                Thread.Sleep(wait);
            }

            throw new Exception($"GetElement error: элемент '{xpath}' не получен за {timeout} миллисекунд каждая");
        }

        #endregion

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
            string searchKind, string logGood, int timeout = 1000, int endCycle = 10, int index = 0, 
            bool showInPosterGood = false)

        {
            for (int i = 0; i < endCycle; i++)
            {
                HtmlElement element = instance.ActiveTab.FindElementByAttribute(tag, attrName, attrValue, searchKind, index);
                if (!element.IsVoid)
                {
                    project.SendToLog(logGood, LogType.Info, showInPosterGood, LogColor.Green);
                    return element;
                }
                Thread.Sleep(timeout);
            }
            throw new Exception($"GetElementAttribute error: element '{tag}[@{attrName} = '{attrValue}']' не найден за {endCycle} попыток по {timeout} миллисекунд каждая");
        }
        /// <summary>
        /// Поиск элемента по положительному размеру без коллекции 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement GetElementBySize(string xpath, string logGood = "GetElementBySize элемент для проверки размера найден", int timeout = 100,
            int endCycle = 10, int index = 0, bool showInPosterGood = false)
        {
            for (int i = 0; i < endCycle; i++)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                string width = element.GetAttribute("width");
                if (int.Parse(width) > 0)
                {
                    project.SendToLog(logGood, LogType.Info, showInPosterGood, LogColor.Green);
                    return element;
                }
                Thread.Sleep(timeout);
            }
            throw new Exception($"GetElementBySize error: element '{xpath}' не найден за {endCycle} попыток по {timeout} миллисекунд каждая");
        }

        /// <summary>
        /// Поиск элемента по положительному размеру в коллекции
        /// </summary>
        /// <param name="xpathCollection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement GetElementByWidthInCollection(string xpathCollection)
        {
            HtmlElementCollection collection = instance.ActiveTab.FindElementsByXPath(xpathCollection);

            if (collection.Count > 0)
            {
                foreach (var el in collection)
                {
                    int width = int.Parse(el.GetAttribute("width"));
                    if (width > 0)
                    {
                        return el;
                    }
                }               
            }
            throw new Exception($"GetElementByWidthInCollection: в коллекции по xpath {xpathCollection} не было найден ни одного элемента с положительной шириной");

        }

        /// <summary>
        /// Проверка элемента на наличие
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool BoolElement(string xpath,
                  int timeout = 5000, int index = 0, int wait = 300)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    project.SendToLog($"BoolElement confirming: элемент {xpath} существует", LogType.Info, false, LogColor.Gray);
                    return true;
                }
                Thread.Sleep(wait);
            }
            project.SendToLog($"BoolElement error: элемент {xpath} не существует", LogType.Warning, false, LogColor.Orange);
            return false;
        }

        /// <summary>
        /// Проверка элемента на наличие, с исключения в случае его не нахождения
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool BoolElementWithError(string xpath,
                 int timeout = 5000, int index = 0, int wait = 300)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);
            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    project.SendToLog($"BoolElementWithError confirm: элемент {xpath} существует", LogType.Info, false, LogColor.Gray);
                    return true;
                }
                Thread.Sleep(wait);
            }
            throw new Exception($"BoolElementWithError error: элементв '{xpath}' не существует");
        }


        /// <summary>
		/// Проверка на исчезновение ещё не найденного элемента, с исключением в случае не исчезновения
		/// </summary>
		/// <param name="xpath"></param>
		/// <param name="timeout"></param>
		/// <param name="index"></param>
		/// <param name="wait"></param>
		/// <returns></returns>
		public bool DisappearElementWithError(string xpath, int timeout = 5000, int index = 0, int wait = 500)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);

            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    Thread.Sleep(wait);
                    continue;
                }
                project.SendToLog($"DisappearElementWithError confirming: элемент {xpath} исчез", LogType.Info, false, LogColor.Gray);
                return true;
            }
            throw new Exception($"DisappearElementWithError error: элементв '{xpath}' не исчез за {timeout} миллисекунд");
        }

        /// <summary>
        /// Проверка на исчезновение уже найденного элемента, с исключением в случае не исчезновения
        /// </summary>
        /// <param name="element"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public bool DisappearElementWithError(HtmlElement element, int timeout = 5000, int index = 0, int wait = 500)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);

            while (DateTime.Now < timeoutDT)
            {
                if (!element.IsVoid)
                {
                    Thread.Sleep(wait);
                    continue;
                }
                project.SendToLog($"DisappearElementWithError confirming: элемент {element.InnerHtml} исчез", LogType.Info, false, LogColor.Gray);
                return true;
            }
            throw new Exception($"DisappearElementWithError error: элементв '{element.InnerHtml}' не исчез за {timeout} миллисекунд");
        }

        /// <summary>
        /// Проверка на исчезновение элемента
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public bool DisappearElement(string xpath, int timeout = 5000, int index = 0, int wait = 500)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);

            while (DateTime.Now < timeoutDT)
            {
                HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath, index);
                if (!element.IsVoid)
                {
                    Thread.Sleep(wait);
                    continue;
                }
                project.SendToLog($"DisappearElement confirming: элемент {xpath} исчез", LogType.Info, false, LogColor.Gray);
                return true;
            }
            project.SendToLog($"DisappearElement error: элемент {xpath} не исчез", LogType.Warning, false, LogColor.Orange);
            return false;
        }

        /// <summary>
        /// Ожидание исчезновения уже найденного элемента
        /// </summary>
        /// <param name="element"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public bool DisappearElement(HtmlElement element, int timeout = 5000, int wait = 500)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);

            while (DateTime.Now < timeoutDT)
            {
                if (!element.IsVoid)
                {
                    Thread.Sleep(wait);
                    continue;
                }
                project.SendToLog($"DisappearElement confirming: элемент {element.InnerHtml} исчез", LogType.Info, false, LogColor.Gray);
                return true;
            }
            throw new Exception($"DisappearElement error: элементв '{element.InnerHtml}' не исчез за {timeout} миллисекунд");

        }
        /// <summary>
        /// Проверка value элемента на указанные данные
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        /// <param name="logGood"></param>
        /// <param name="showInPosterGood"></param>
        /// <exception cref="Exception"></exception>
        public void CheckValueElement(HtmlElement element, string text,
            string logGood = "CheckValueElement значение элемента совпадает с проверяемым", bool showInPosterGood = false)
        {
            string valueElement = element.GetValue();
            if (valueElement.Contains(text))
            {
                project.SendToLog(logGood, LogType.Info, showInPosterGood, LogColor.Green);
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
        public void SetValue(HtmlElement element, string text,
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
        public void SetValue(string xpath,string logGoodFind, string text, string logGoodCheck, 
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
        public HtmlElement NavigateWithoutTry(string url, string xpath,
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
        public HtmlElement NavigateWithTry(string url, string xpath,
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
        public void SetValueWithoutCheck(string xpath, string text, string logGood,
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
        public void SetValueWithoutCheck(HtmlElement element, string text,
            string logGood, string emulation = "None",
            bool addend = false, bool showInPosterGood = false)
        {
            element.SetValue(text, emulation, append: addend);
            project.SendToLog(logGood, LogType.Info, showInPosterGood, LogColor.Green);
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
        /// ФулКлик по элементу и получение его с помощью метода GetElement
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public HtmlElement FullClick(string xpathFullClick, int timeout = 1000, int index = 0, int wait = 250)
        {
            HtmlElement element = GetElement(xpathFullClick, timeout, index, wait);
            instance.ActiveTab.FullEmulationMouseMoveToHtmlElement(element);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");
            project.SendToLog($"FullClick confirming: по элементу {xpathFullClick}", LogType.Info, false, LogColor.Gray);
            return element;
        }
        /// <summary>
        /// ФулКлик по уже найденному элементу
        /// </summary>
        /// <param name="element"></param>
        public void FullClick(HtmlElement element)
        {
            instance.ActiveTab.FullEmulationMouseMoveToHtmlElement(element);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");
            project.SendToLog($"FullClick confirming: по элементу {element.InnerHtml}", LogType.Info, false, LogColor.Gray);
        }
        /// <summary>
        ///  Клик по уже найденному элементу с выбором эмуляции
        /// </summary>
        /// <param name="elementClick"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <param name="emulation"></param>
        public void Click(HtmlElement elementClick, int timeout = 1000,
             int index = 0, int wait = 250, string emulation = "None")
        {
            elementClick.RiseEvent("click", emulation);
            project.SendToLog($"Click confirming: по элементу {elementClick.InnerHtml}", LogType.Info, false, LogColor.Gray);
        }

        /// <summary>
        /// Клик по элементу с выбором эмуляции и получением его с помощью метода GetElement
        /// </summary>
        /// <param name="xpathClick"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <param name="emulation"></param>
        /// <returns></returns>
        public HtmlElement Click(string xpathClick, int timeout = 1000,
             int index = 0, int wait = 250, string emulation = "None")
        {
            HtmlElement element = GetElement(xpathClick, timeout, index, wait);
            element.RiseEvent("click", emulation);
            project.SendToLog($"Click confirming: по элементу {xpathClick}", LogType.Info, false, LogColor.Gray);
            return element;
        }

        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры с помощью ФулКлик по полю ввода ещё не найденного элемента
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="text"></param>
        /// <param name="latency"></param>
        /// <param name="logGoodFullClick"></param>
        /// <param name="timeoutFullClick"></param>
        /// <param name="endCycleFullClick"></param>
        /// <param name="indexFullClick"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="showInPosterGoodFullClick"></param>
        /// <param name="showInPosterGoodCheck"></param>
        public void SetValueFull(string xpathFullClick, string text, int latency = 20,
            string logGoodFullClick = "Элемент для FullClick найден", int timeoutFullClick = 10, int endCycleFullClick = 2, int indexFullClick = 0,
            string logGoodCheck = "Value элемента изменено!", bool showInPosterGoodFullClick = false, bool showInPosterGoodCheck = false)
        {
            HtmlElement element = GetElement(xpathFullClick, logGoodFullClick, timeoutFullClick, endCycleFullClick, indexFullClick, showInPosterGoodFullClick);
            FullClick(element);
            instance.SendText(text, latency);

            CheckValueElement(element, text, logGoodCheck, showInPosterGoodCheck);
        }
        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры с помощью ФулКлик по полю ввода уже найденного элемента
        /// <param name="element"></param>
        /// <param name="text">Вводимое значение</param>
        /// <param name="logGood">Сообщение в лог, если выполнено</param>
        /// <param name="latency">Задержка между вводимыми символами</param>
        /// <param name="showInPosterGood">Разрешить или запретить вывод удачного выполнения в ЗенноПостер</param>
        /// <exception cref="Exception"></exception>
        public void SetValueFull(HtmlElement element, string text, int latency = 20,
            string logGoodCheck = "Value элемента изменено!", bool showInPosterGoodCheck = false)
        {
            FullClick(element);
            instance.SendText(text, latency);

            CheckValueElement(element, text, logGoodCheck, showInPosterGoodCheck);
        }
        /// <summary>
        /// Установка значения для атрибута в ещё не найденном элементе
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="nameAttr"></param>
        /// <param name="value"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        public void SetValueAttribute(string xpath, string nameAttr, string value,
            string logGood = "Элемент для SetAttribute найден", int timeout = 10, int endCycle = 2, int index = 0,
            bool showInPosterGood = false)
        {
            var element = GetElement(xpath, logGood, timeout,endCycle,index, showInPosterGood );
            element.SetAttribute(nameAttr, value);
        }
        /// <summary>
        /// Установка значения для атрибута в уже найденом элементе
        /// </summary>
        /// <param name="element"></param>
        /// <param name="nameAttr"></param>
        /// <param name="value"></param>
        public void SetValueAttribute(HtmlElement element, string nameAttr, string value)
        {
            element.SetAttribute(nameAttr, value);
        }

        /// <summary>
        /// ФулКлик через xpath с проверкой и попытками 
        /// </summary>
        /// <param name="fullClickXpath"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="logGoodClickXpath"></param>
        /// <param name="indexFullClickElement"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="endCycleCheckElement"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGoodCheckElement"></param>
        /// <exception cref="Exception"></exception>
        public void FullClickWithCheck(string fullClickXpath, string xpathCheckElement,
            string logGoodClickXpath = "Элемент для FullClick найден", int indexFullClickElement = 0,
            string logGoodCheck = "FullClickWithCheck по элементу удался!", int timeoutCheckElement = 1000,
            int endCycleCheckElement = 10, int indexCheckElement = 0, bool showInPosterGoodCheckElement = false)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    FullClick(fullClickXpath, logGoodClickXpath, index: indexFullClickElement);

                    GetElement(xpathCheckElement, logGoodCheck, timeoutCheckElement, endCycleCheckElement, indexCheckElement, showInPosterGoodCheckElement);

                    break;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        throw new Exception("Ошибка выполнения FullClickWithCheck: " + ex.Message);
                    }
                    continue;
                }
            }
        }
        /// <summary>
        /// ФулКлик по уже найденному элементу с проверкой и попытками 
        /// </summary>
        /// <param name="fullClickElement"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="endCycleCheckElement"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGoodCheckElement"></param>
        /// <exception cref="Exception"></exception>
        public void FullClickWithCheck(HtmlElement fullClickElement, string xpathCheckElement, int timeoutCheckElement,
           string logGoodCheck = "FullClickWithCheck по элементу удался!", int endCycleCheckElement = 10,
           int indexCheckElement = 0, bool showInPosterGoodCheckElement = false)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    FullClick(fullClickElement);

                    GetElement(xpathCheckElement, logGoodCheck, timeoutCheckElement, endCycleCheckElement, indexCheckElement, showInPosterGoodCheckElement);

                    break;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        throw new Exception("Ошибка выполнения FullClickWithCheck: " + ex.Message);
                    }
                    continue;
                }
            }
        }
        /// <summary>
        /// Клик по элементу по его xpath с выбором эмуляции, проверкой и попытками 
        /// </summary>
        /// <param name="xpathClick"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="logGoodClick"></param>
        /// <param name="indexClick"></param>
        /// <param name="emulation"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="endCycleCheckElement"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGoodCheckElement"></param>
        /// <exception cref="Exception"></exception>
        public void ClickWithCheck(string xpathClick, string xpathCheckElement,
            string logGoodClick = "Click по элементу удался!", int indexClick = 0, string emulation = "None",
            string logGoodCheck = "ClickWithCheck по элементу удался!", int timeoutCheckElement = 0, int endCycleCheckElement = 10, int indexCheckElement = 0,
           bool showInPosterGoodCheckElement = false)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    Click(xpathClick, logGoodClick, index: indexClick, emulation: emulation);

                    GetElement(xpathCheckElement, logGoodCheck, timeoutCheckElement, endCycleCheckElement, indexCheckElement, showInPosterGoodCheckElement);

                    break;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        throw new Exception("Ошибка выполнения ClickWithCheck: " + ex.Message);
                    }
                    continue;
                }
            }
        }
        /// <summary>
        /// Клик по уже найденному элементу с выбором эмуляции, проверкой и попытками 
        /// </summary>
        /// <param name="clickElement"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="emulation"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="endCycleCheckElement"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGoodCheckElement"></param>
        /// <exception cref="Exception"></exception>
        public void ClickWithCheck(HtmlElement clickElement, string xpathCheckElement, string emulation = "None",
           string logGoodCheck = "ClickWithCheck по элементу удался!", int timeoutCheckElement = 1000, int endCycleCheckElement = 10,
           int indexCheckElement = 0, bool showInPosterGoodCheckElement = false)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    Click(clickElement, emulation);

                    GetElement(xpathCheckElement, logGoodCheck, timeoutCheckElement, endCycleCheckElement, indexCheckElement, showInPosterGoodCheckElement);

                    break;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        throw new Exception("Ошибка выполнения ClickWithCheck: " + ex.Message);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// Нажатие на кнопку клавиатуры в цикле без проверки
        /// </summary>
        /// <param name="button"></param>
        /// <param name="endCycle"></param>
        public void ButtonInCycle(string button,int endCycle = 1)
        {
            for (int i = 0; i < endCycle; i++)
            {
                instance.WaitFieldEmulationDelay();
                instance.SendText($"{button}", 15);
            }
            
        }
        
        /// <summary>
        /// Нажатие на кнопку клавиатуры в циклве и проверкой
        /// </summary>
        /// <param name="button"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="endCycleButton"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="endCycleCheckElement"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGoodCheckElement"></param>
        /// <exception cref="Exception"></exception>
        public void ButtonInCycleWithCheck(string button, string xpathCheckElement, int endCycleButton = 1,
            string logGoodCheck = "ButtonInCycleWithCheck нажатие на клавишу дало нужный результат", int timeoutCheckElement = 300, 
            int endCycleCheckElement = 10, int indexCheckElement = 0, bool showInPosterGoodCheckElement = false)
        {
            for (int i = 0; i < endCycleButton; i++)
            {
                try
                {
                    instance.WaitFieldEmulationDelay();
                    instance.SendText($"{button}", 15);

                    GetElement(xpathCheckElement, logGoodCheck, timeoutCheckElement, endCycleCheckElement, indexCheckElement, showInPosterGoodCheckElement);
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        throw new Exception("Ошибка выполнения ButtonInCycleWithCheck: " + ex.Message);
                    }
                    continue;
                }                
            }
        }

        /// <summary>
        /// Загрузка файла на сервер с проверкой
        /// </summary>
        /// <param name="pathFile"></param>
        /// <param name="xpathFullClick"></param>
        /// <param name="xpathCheckDownload"></param>
        /// <param name="logGoodFullClick"></param>
        /// <param name="timeoutFullClick"></param>
        /// <param name="endCycleFullClick"></param>
        /// <param name="indexFullClick"></param>
        /// <param name="logGoodCheckDownload"></param>
        /// <param name="timeoutCheckDownload"></param>
        /// <param name="endCycleCheckDownload"></param>
        /// <param name="indexCheckDownload"></param>
        /// <param name="showInPosterGoodFullClick"></param>
        /// <param name="showInPosterGoodCheckDownload"></param>
        public virtual void DownloadFile(string pathFile, string xpathFullClick, string xpathCheckDownload,
            string logGoodFullClick = "Элемент загрузки файла для FullClick найден!", int timeoutFullClick = 100, int endCycleFullClick = 2, int indexFullClick = 0,
            string logGoodCheckDownload = "Файл успешно загружен!", int timeoutCheckDownload = 500, int endCycleCheckDownload = 10, int indexCheckDownload = 0,
            bool showInPosterGoodFullClick = false, bool showInPosterGoodCheckDownload = false)
        {
            instance.SetFileUploadPolicy("ok", "");
            instance.SetFilesForUpload(pathFile);

            FullClick(xpathFullClick,logGoodFullClick, timeoutFullClick, endCycleFullClick, indexFullClick, showInPosterGoodFullClick);

            GetElement(xpathCheckDownload, logGoodCheckDownload, timeoutCheckDownload, endCycleCheckDownload, indexCheckDownload, showInPosterGoodCheckDownload);
        }

        /// <summary>
        /// Парсинг атрибутов по xpath с возможностью выбрать содержание
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="attr"></param>
        /// <param name="contains"></param>
        /// <returns></returns>
        public List<string> Parsing(string xpath, string attr, string contains = "", ZennoLab.InterfacesLibrary.Enums.Parser.FilterType filterType = ZennoLab.InterfacesLibrary.Enums.Parser.FilterType.None)
        {
            // Получаем атрибут attr всех элементов, соответствующих пути xpath
            var attributes = ZennoPoster.Parser.ParseByXpath(instance.ActiveTab, ZennoLab.InterfacesLibrary.Enums.Parser.SourceType.Dom, xpath, attr, true).ToList();
            // Фильтруем элементы
            attributes.Filter(filterType, contains);
            // Выбираем элементы из диапазона "all"
            attributes.Range("all");

            return attributes;
        }

  


    }
}
