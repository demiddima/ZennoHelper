using Global.ZennoExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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


        #region Базовые простые методы

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
        public HtmlElement GetElementAttribute(string tag, string attrName, string attrValue,
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
        /// Получение значения арибута ещё не найденного элемента
        /// </summary>
        /// <param name="xpathElement"></param>
        /// <param name="nameAttribute"></param>
        /// <returns></returns>
        public string GetValueAttribute(string xpathElement, string nameAttribute)
        {
            HtmlElement element = instance.ActiveTab.FindElementByXPath(xpathElement, 0);
            string value = element.GetAttribute(nameAttribute).Trim();
            project.SendToLog($"GetValueAttribute confirming: значение атрибута '{nameAttribute}' элемента '{xpathElement}' получено", LogType.Info, false, LogColor.Gray);

            return value;
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
        /// Проверка value элемента на указанные данные
        /// </summary>
        /// <param name="element"></param>
        /// <param name="xpath"></param>
        /// <param name="text"></param>
        /// <exception cref="Exception"></exception>
        public void CheckValueElement(HtmlElement element, string xpath, string text)
        {
            string valueElement = element.GetValue();
            if (valueElement.Contains(text))
            {
                project.SendToLog($"CheckValueElement confirming: у элемента '{xpath}' совпадает value с назначенным '{text}'", LogType.Info, false, LogColor.Gray);
            }
            else
            {
                throw new Exception($"CheckValueElement error: element '{xpath}' c значением '{text}' не найден");
            }
        }


        /// <summary>
        /// Получение элемента из коллекции по его положительной ширине, с исключением в случае не нахождения+
        /// </summary>
        /// <param name="xpathCollection"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement GetElementByWidthPositiveInCollection(string xpathCollection, int timeout = 500, int wait = 200)
        {

            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);

            while (DateTime.Now < timeoutDT)
            {
                HtmlElementCollection collection = instance.ActiveTab.FindElementsByXPath(xpathCollection);
                if (collection.Count > 0)
                {
                    foreach (var el in collection)
                    {
                        int width = int.Parse(el.GetAttribute("width"));
                        if (width > 0)
                        {
                            project.SendToLog($"GetElementByWidthPositiveInCollection confirming: элемент из коллекции {xpathCollection} с положительной шириной получен", LogType.Info, false, LogColor.Gray);
                            return el;
                        }
                    }
                }
                Thread.Sleep(wait);
            }
            throw new Exception($"GetElementByWidthInCollection error: в коллекции по xpath {xpathCollection} не было найдено ни одного элемента с положительной шириной");
        }

        /// <summary>
        /// Получение элемента из коллекции по его отрицательной ширине, с исключением в случае не нахождения
        /// </summary>
        /// <param name="xpathCollection"></param>
        /// <param name="timeout"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement GetElementByNegativeWidthInCollection(string xpathCollection, int timeout = 500, int wait = 200)
        {

            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);

            while (DateTime.Now < timeoutDT)
            {
                HtmlElementCollection collection = instance.ActiveTab.FindElementsByXPath(xpathCollection);
                if (collection.Count > 0)
                {
                    foreach (var el in collection)
                    {
                        int width = int.Parse(el.GetAttribute("width"));
                        if (width < 0)
                        {
                            project.SendToLog($"GetElementByNegativeWidthInCollection congirming: элемент из коллекции {xpathCollection} с отрицательной шириной получен", LogType.Info, false, LogColor.Gray);
                            return el;
                        }
                    }
                }
                Thread.Sleep(wait);
            }
            throw new Exception($"GetElementByWidthInCollection error: в коллекции по xpath {xpathCollection} не было найдено ни одного элемента с отрицательной шириной");
        }

        #endregion

        #region Базовые сложные методы

        /// <summary>
        /// Поиск на странице одного элемента из коллеции с возвратом 2-х XPath - элемент, который ищется и элемент, который к нему привязывается
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="xpathKey"></param>
        /// <param name="xpathValue"></param>
        /// <param name="timeout"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public bool GetFoundElement(Dictionary<string, string> dictionary, out string xpathKey, out string xpathValue, int timeout = 5000, int wait = 400)
        {
            DateTime timeoutDT = DateTime.Now.AddMilliseconds(timeout);
            xpathValue = String.Empty;
            xpathKey = String.Empty;

            while (DateTime.Now < timeoutDT)
            {
                foreach (var xpath in dictionary)
                {
                    HtmlElement element = instance.ActiveTab.FindElementByXPath(xpath.Key, 0);
                    if (!element.IsVoid)
                    {
                        project.SendToLog($"GetFoundElement confirming: элемент {xpath.Key} существует на текущей странице", LogType.Info, false, LogColor.Gray);
                        xpathKey = xpath.Key;
                        xpathValue = xpath.Value;
                        return true;
                    }
                }
                Thread.Sleep(wait);
            }
            project.SendToLog($"GetFoundElement confirming: не один из элементов коллекции не существует на текущей странице", LogType.Info, false, LogColor.Gray);
            return false;
        }

        /// <summary>
        /// Перебор элементов в цикле с кликом на каждый, с получением элемента изменения и проверкой атрибута у этого элемента на соответствие заданному значению
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="nameAttribut"></param>
        /// <param name="value"></param>
        /// <param name="xpathAction"></param>
        /// <param name="xpathNextAction"></param>
        public void ActionRightElements(Dictionary<string, string> dictionary, string nameAttribut, string value, string xpathAction, string xpathNextAction = null)
        {
            foreach (var xpath in dictionary)
            {
                HtmlElement element = FullClickGet(xpath.Key, xpath.Value);
                string attribut = element.GetAttribute(nameAttribut);

                if (attribut.Contains(value))
                {
                    FullClickDisappear(xpathAction);
                    project.SendToLog($"ActionRightElements confirming: элемент '{xpath.Value}' содержит в себе '{value}'", LogType.Info, false, LogColor.Gray);
                    break;
                }
                else
                {
                    if (xpathNextAction == null)
                    {
                        continue;
                    }
                    FullClickDisappear(xpathNextAction);
                    continue;
                }
            }

        }

        #endregion


        #region Клики

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
                 int wait = 250, string emulation = "None")
        {
            HtmlElement element = GetElement(xpathClick, timeout, wait);
            element.RiseEvent("click", emulation);
            project.SendToLog($"Click confirming: по элементу {xpathClick}", LogType.Info, false, LogColor.Gray);
            return element;
        }

        /// <summary>
        /// Клик по уже найденному элементу с выбором эмуляции
        /// </summary>
        /// <param name="elementClick"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <param name="emulation"></param>
        public void Click(HtmlElement elementClick, string emulation = "None")
        {
            elementClick.RiseEvent("click", emulation);
            project.SendToLog($"Click confirming: по элементу {elementClick.InnerHtml}", LogType.Info, false, LogColor.Gray);
        }


        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public HtmlElement FullClick(string xpathFullClick, int timeout = 1000, int wait = 250)
        {
            HtmlElement element = GetElement(xpathFullClick, timeout, wait);
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
		/// Клик по ещё не найденному элементу с выбором эмуляции и получением элемента проверки
		/// </summary>
		/// <param name="xpathClick"></param>
		/// <param name="xpathGet"></param>
		/// <param name="timeoutGet"></param>
		/// <param name="waitGet"></param>
		/// <param name="emulation"></param>
		/// <returns></returns>
		public HtmlElement ClickGet(string xpathClick, string xpathGet, int timeoutGet = 6000, int waitGet = 500, string emulation = "None")
        {
            project.SendToLog($"Начало работы метода ClickGet", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(xpathClick, emulation: emulation);
                    HtmlElement element = GetElement(xpathGet, timeoutGet, wait: waitGet);
                    project.SendToLog($"Завершение работы метода ClickGet", LogType.Info, false, LogColor.Green);
                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickGet: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("Ошибка выполнения ClickGet: ");
        }

        /// <summary>
        /// Клик по уже не найденному элементу с выбором эмуляции и получением элемента проверки
        /// </summary>
        /// <param name="elementClick"></param>
        /// <param name="xpathGet"></param>
        /// <param name="timeoutGet"></param>
        /// <param name="waitGet"></param>
        /// <param name="emulation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement ClickGet(HtmlElement elementClick, string xpathGet, int timeoutGet = 6000, int waitGet = 500, string emulation = "None")
        {
            project.SendToLog($"Начало работы метода ClickGet", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(elementClick, emulation: emulation);
                    HtmlElement element = GetElement(xpathGet, timeoutGet, wait: waitGet);
                    project.SendToLog($"Завершение работы метода ClickGet", LogType.Info, false, LogColor.Green);
                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickGet: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("Ошибка выполнения ClickGet: ");
        }

        /// <summary>
        /// Клик по ещё не найденному элементу с выбором эмуляции и проверкой изменения value другого элемента
        /// </summary>
        /// <param name="xpathClick"></param>
        /// <param name="xpathGet"></param>
        /// <param name="valueCheck"></param>
        /// <param name="timeoutGet"></param>
        /// <param name="waitGet"></param>
        /// <param name="emulation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement ClickCheckValue(string xpathClick, string xpathGet, string valueCheck, int timeoutGet = 10000, int waitGet = 500, string emulation = "None")
        {
            project.SendToLog($"Начало работы метода ClickGet", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(xpathClick, emulation: emulation);
                    HtmlElement element = GetElement(xpathGet, timeoutGet, wait: waitGet);
                    CheckValueElement(element, xpathGet, valueCheck);
                    project.SendToLog($"Завершение работы метода ClickGet", LogType.Info, false, LogColor.Green);
                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickGet: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("Ошибка выполнения ClickGet: ");
        }


        /// <summary>
        /// Клик по ещё не найденному элементу с выбором эмуляции и ожиданием его исчезновения
        /// </summary>
        /// <param name="xpathClick"></param>
        /// <param name="timeoutDisappear"></param>
        /// <param name="waitDisappear"></param>
        /// <param name="emulation"></param>
        /// <exception cref="Exception"></exception>
        public void ClickDisappear(string xpathClick, int timeoutDisappear = 10000, int waitDisappear = 500, string emulation = "None")
        {
            project.SendToLog($"Начало работы метода ClickDisappear", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(xpathClick, emulation: emulation);
                    DisappearElementWithError(xpathClick, timeoutDisappear, wait: waitDisappear);
                    project.SendToLog($"Завершение работы метода ClickDisappear", LogType.Info, false, LogColor.Green);
                    break;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickDisappear: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Клик по уже найденному элементу с выбором эмуляции и ожиданием исчезновения элемента
        /// </summary>
        /// <param name="elementClick"></param>
        /// <param name="xpathDisappear"></param>
        /// <param name="timeoutDisappear"></param>
        /// <param name="waitDisappear"></param>
        /// <param name="emulation"></param>
        /// <exception cref="Exception"></exception>
        public void ClickDisappear(HtmlElement elementClick, string xpathDisappear, int timeoutDisappear = 10000, int waitDisappear = 500, string emulation = "None")
        {
            project.SendToLog($"Начало работы метода ClickDisappear", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(elementClick, emulation: emulation);
                    DisappearElementWithError(xpathDisappear, timeoutDisappear, wait: waitDisappear);
                    project.SendToLog($"Завершение работы метода ClickDisappear", LogType.Info, false, LogColor.Green);
                    break;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickDisappear: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Клик с выбором эмуляции и проверкой исчезновения элемента через его отрицательную ширину
        /// </summary>
        /// <param name="xpathClick"></param>
        /// <param name="xpathCollection"></param>
        /// <param name="wait"></param>
        /// <param name="emulation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement ClickGetByNegativeWidth(string xpathClick, string xpathCollection, int wait = 500, string emulation = "None")
        {
            project.SendToLog($"Начало работы метода ClickGetByNegativeWidth", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(xpathClick, wait, emulation: emulation);
                    HtmlElement element = GetElementByNegativeWidthInCollection(xpathCollection);
                    project.SendToLog($"Завершение работы метода ClickGetByNegativeWidth", LogType.Info, false, LogColor.Green);
                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickGetByNegativeWidth: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("Ошибка выполнения ClickGetByNegativeWidth");
        }

        /// <summary>
        /// Клик с получением значения атрибута элемента
        /// </summary>
        /// <param name="xpathClick"></param>
        /// <param name="xpathGet"></param>
        /// <param name="nameGetAttr"></param>
        /// <param name="emulation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ClickGetValueAttribute(string xpathClick, string xpathGet, string nameGetAttr, string emulation = "Middle")
        {
            project.SendToLog($"Начало работы метода ClickCheckValueAttribute", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    Click(xpathClick, emulation: emulation);
                    string valueAttr = GetValueAttribute(xpathGet, nameGetAttr);
                    project.SendToLog($"Завершение работы метода ClickCheckValueAttribute", LogType.Info, false, LogColor.Green);
                    break;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения ClickCheckValueAttribute: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("Ошибка выполнения ClickCheckValueAttribute: ");
        }

        /// <summary>
        /// ФулКлик по ещё не найденному элементу с ожиданием его исчезновения
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="timeoutDisappear"></param>
        /// <param name="waitDisappear"></param>
        /// <exception cref="Exception"></exception>
        public void FullClickDisappear(string xpathFullClick, int timeoutDisappear = 5000, int waitDisappear = 500)
        {
            project.SendToLog($"Начало работы метода FullClickDisappear", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    FullClick(xpathFullClick);

                    DisappearElementWithError(xpathFullClick, timeoutDisappear, wait: waitDisappear);
                    project.SendToLog($"Завершение работы метода FullClickDisappear", LogType.Info, false, LogColor.Green);
                    break;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения FullClickDisappear: " + ex.Message);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// ФулКлик по уже найденному элементу с ожиданием исчезновения элемента, по которому был сделан клик
        /// </summary>
        /// <param name="elementFullClick"></param>
        /// <param name="xpathDisappear"></param>
        /// <param name="timeoutDisappear"></param>
        /// <param name="waitDisappear"></param>
        /// <exception cref="Exception"></exception>
        public void FullClickDisappear(HtmlElement elementFullClick, string xpathDisappear, int timeoutDisappear = 5000, int waitDisappear = 500)
        {
            project.SendToLog($"Начало работы метода FullClickDisappear", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    FullClick(elementFullClick);
                    DisappearElementWithError(xpathDisappear, timeoutDisappear, wait: waitDisappear);

                    project.SendToLog($"Завершение работы метода FullClickDisappear", LogType.Info, false, LogColor.Green);
                    break;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения FullClickDisappear: " + ex.Message);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// ФулКлик по ещё не найденному элементу и получение проверяемого элемента
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="xpathGetElement"></param>
        /// <param name="timeoutGetElement"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement FullClickGet(string xpathFullClick, string xpathGetElement,
           int timeoutGetElement = 5000, int wait = 500)
        {
            project.SendToLog($"Начало работы метода FullClickGet", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    FullClick(xpathFullClick);

                    HtmlElement element = GetElement(xpathGetElement, timeoutGetElement, wait: wait);

                    project.SendToLog($"Завершение работы метода FullClickGet", LogType.Info, false, LogColor.Green);
                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения FullClickGet: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception($"FullClickGet error: попытка кликнуть по элементу '{xpathFullClick}' и получение элемента '{xpathGetElement}' не удались");
        }


        /// <summary>
        /// Фул клик по уже найденному элементу и получение, проверяемого элемента
        /// </summary>
        /// <param name="elementFullClick"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public HtmlElement FullClickGet(HtmlElement elementFullClick, string xpathCheckElement, int timeoutCheckElement = 5000, int wait = 500)
        {
            project.SendToLog($"Начало работы метода FullClickGet", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    FullClick(elementFullClick);
                    HtmlElement element = GetElement(xpathCheckElement, timeoutCheckElement, wait: wait);

                    project.SendToLog($"Завершение работы метода FullClickGet", LogType.Info, false, LogColor.Green);
                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("Ошибка выполнения FullClickGet: " + ex.Message);

                }
                catch
                {
                    continue;
                }
            }
            throw new Exception($"FullClickGet error: попытка кликнуть по элементу '{elementFullClick.InnerHtml}' и получение элемента '{xpathCheckElement}' не удались");
        }



        #endregion

        #region Ввод значения

        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры с помощью ФулКлик по полю ввода ещё не найденного элемента
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="text"></param>
        /// <param name="latency"></param>
        /// <param name="timeoutFullClick"></param>
        /// <param name="indexFullClick"></param>
        /// <param name="wait"></param>
        public void SetValueFull(string xpathSet, string text, int latency = 20,
            int timeoutSet = 1000, int wait = 250)
        {
            project.SendToLog($"Начало работы метода SetValueFull", LogType.Info, false, LogColor.Pink);

            HtmlElement element = GetElement(xpathSet, timeoutSet, wait: wait);
            FullClick(element);
            instance.SendText(text, latency);
            CheckValueElement(element, xpathSet, text);

            project.SendToLog($"Завершение работы метода SetValueFull", LogType.Info, false, LogColor.Green);
        }
        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры с помощью ФулКлик по полю ввода уже найденного элемента
        /// </summary>
        /// <param name="elementSet"></param>
        /// <param name="text"></param>
        /// <param name="latency"></param>
        /// <param name="timeoutSet"></param>
        /// <param name="wait"></param>
        public void SetValueFull(HtmlElement elementSet, string text, int latency = 20)
        {
            project.SendToLog($"Начало работы метода SetValueFull", LogType.Info, false, LogColor.Pink);

            FullClick(elementSet);
            instance.SendText(text, latency);
            CheckValueElement(elementSet, elementSet.InnerHtml, text);

            project.SendToLog($"Завершение работы метода SetValueFull", LogType.Info, false, LogColor.Green);
        }
        /// <summary>
        /// Ввод текста с предварительным удалением значения в поле для не найденного элемента
        /// </summary>
        /// <param name="xpathFullClick"></param>
        /// <param name="text"></param>
        /// <param name="latency"></param>
        /// <param name="timeoutFullClick"></param>
        /// <param name="indexFullClick"></param>
        /// <param name="wait"></param>
        public void SetValueFullWithDelete(string xpathSet, string text, int latency = 20,
            int timeoutSet = 1000, int wait = 250)
        {
            project.SendToLog($"Начало работы метода SetValueFullWithDelete", LogType.Info, false, LogColor.Pink);

            HtmlElement element = GetElement(xpathSet, timeoutSet, wait: wait);
            int lengthValue = element.GetValue().Length;

            FullClick(element);
            ButtonInCycle("{END}");
            ButtonInCycle("{BACKSPACE}", lengthValue);


            instance.SendText(text, latency);
            ButtonInCycle("{ENTER}");

            CheckValueElement(element, xpathSet, text);

            project.SendToLog($"Завершение работы метода SetValueFullWithDelete", LogType.Info, false, LogColor.Green);
        }

        /// <summary>
        /// Ввод текста с предварительным удалением значения в поле для уже найденного элемента
        /// </summary>
        /// <param name="elementSet"></param>
        /// <param name="text"></param>
        /// <param name="latency"></param>
        public void SetValueFullWithDelete(HtmlElement elementSet, string text, int latency = 20)
        {
            project.SendToLog($"Начало работы метода SetValueFullWithDelete", LogType.Info, false, LogColor.Pink);

            int lengthValue = elementSet.GetValue().Length;

            FullClick(elementSet);
            ButtonInCycle("{END}");
            ButtonInCycle("{BACKSPACE}", lengthValue);


            instance.SendText(text, latency);
            ButtonInCycle("{ENTER}");

            CheckValueElement(elementSet, elementSet.InnerHtml, text);

            project.SendToLog($"Завершение работы метода SetValueFullWithDelete", LogType.Info, false, LogColor.Green);
        }
        /// <summary>
        /// Нажатие по клавише указанное кол-во раз
        /// </summary>
        /// <param name="button"></param>
        /// <param name="endCycle"></param>
        public void ButtonInCycle(string button, int endCycle = 1)
        {
            for (int i = 0; i < endCycle; i++)
            {
                instance.WaitFieldEmulationDelay();
                instance.SendText($"{button}", 15);
            }
            project.SendToLog($"ButtonInCycle confirming: все кнопки были нажаты", LogType.Info, false, LogColor.Gray);
        }

        /// <summary>
        /// Нажатие указанной кнопки с проверкой изменений через GetElement
        /// </summary>
        /// <param name="button"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="endCycleButton"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="wait"></param>
        public HtmlElement ButtonInCycleWithkGet(string button, string xpathCheck,
                int timeoutCheck = 3000, int wait = 500)
        {
            project.SendToLog($"Начало работы метода ButtonInCycleWithkGet", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    instance.WaitFieldEmulationDelay();
                    instance.SendText($"{button}", 15);

                    HtmlElement element = GetElement(xpathCheck, timeoutCheck, wait: wait);

                    project.SendToLog($"Завершение работы метода ButtonInCycleWithkGet", LogType.Info, false, LogColor.Green);

                    return element;
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("ButtonInCycleWithkGet error: " + ex.Message);
                }
                catch
                {
                    continue;
                }
            }
            throw new Exception("ButtonInCycleWithkGet error: элемент {xpathCheckElement} не получен");
        }

        /// <summary>
        /// Нажатие указанной кнопки с проверкой изменений через DisappearElement
        /// </summary>
        /// <param name="button"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="endCycleButton"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="wait"></param>
        public void ButtonInCycleWithDisappear(string button, string xpathCheck,
             int timeoutCheck = 3000, int wait = 500)
        {
            project.SendToLog($"Начало работы метода ButtonInCycleWithDisappear", LogType.Info, false, LogColor.Pink);
            for (int i = 0; i <= 1; i++)
            {
                try
                {
                    instance.WaitFieldEmulationDelay();
                    instance.SendText($"{button}", 15);

                    DisappearElement(xpathCheck, timeoutCheck, wait: wait);

                    project.SendToLog($"Завершение работы метода ButtonInCycleWithDisappear", LogType.Info, false, LogColor.Green);
                }
                catch (Exception ex) when (i == 1)
                {
                    throw new Exception("ButtonInCycleWithDisappear error: " + ex.Message);
                }
                catch
                {
                    continue;
                }

            }
        }



        #endregion


        #region Разное

        /// <summary>
        /// Получеть значени после регулярки
        /// </summary>
        /// <param name="text"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public string GetValueRegex(string text, string regex)
        {
            Match match = Regex.Match(text, $@"{regex}");
            return match.Value;
        }

        #endregion


        

  


    }
}
