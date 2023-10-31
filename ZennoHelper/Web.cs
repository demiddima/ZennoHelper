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

namespace ZennoHelper
{
    public class Web : Main
    {

        public Web(Instance instance, IZennoPosterProjectModel project) :base(instance, project)
        {

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
                    project.SendToLog(logGood, LogType.Info, showInPosterGood, LogColor.Green);
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
        /// Проверка существование html-элемента по его XPath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool BoolElement(string xpath, string logGood,
            int timeout = 50, int endCycle = 5, int index = 0)
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
            project.SendToLog(logGood, LogType.Info, showInPosterGood, LogColor.Green);
        }

        /// <summary>
        /// ФулКлик мышью по уже найденному html-элементу с предварительной наводкой на него
        /// </summary>
        /// <param name="element">HtmlElement</param>
        private void FullClick(HtmlElement element)
        {
            instance.ActiveTab.FullEmulationMouseMoveToHtmlElement(element);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        private void FullClick(string xpath, string logGood, int timeout = 10, int endCycle = 2, int index = 0, bool showInPosterGood = false)
        {
            HtmlElement element = GetElement(xpath, logGood, timeout, endCycle, index, showInPosterGood);
            instance.ActiveTab.FullEmulationMouseMoveToHtmlElement(element);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");
        }
        /// <summary>
        /// ФулКлик по координатам
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void FullClick(int x, int y)
        {
            instance.ActiveTab.FullEmulationMouseMove(x, y);
            instance.ActiveTab.FullEmulationMouseClick("left", "click");

        }
        /// <summary>
        /// Клик мышью по уже найденному html-элементу с выбором уровня эмуляции
        /// </summary>
        /// <param name="element">HtmlElement</param>
        /// <param name="emulation">Уровень эмуляции: None, Middle, Full, SuperEmulation</param>
        private void Click(HtmlElement element, string emulation = "None")
        {
            element.RiseEvent("click", emulation);
        }

        /// <summary>
        /// Клик мышью по не найденному html-элементу с выбором уровня эмуляции
        /// </summary>
        /// <param name="xpath">Путь XPath для элемента</param>
        /// <param name="index">Индекс XPath для элемента</param>
        /// <param name="emulation">Уровень эмуляции: None, Middle, Full, SuperEmulation</param>
        private void Click(string xpath, string logGood, int timeout = 10, int endCycle = 2, int index = 0, string emulation = "None", bool showInPosterGood = false)
        {
            HtmlElement element = GetElement(xpath, logGood, timeout, endCycle, index, showInPosterGood);
            element.RiseEvent("click", emulation);
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
        private void SetValueFull(string xpathFullClick, string text, int latency = 20,
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
        private void SetValueFull(HtmlElement element, string text, int latency = 20,
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
        private void FullClickWithCheck(string fullClickXpath, string xpathCheckElement,
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
        /// <param name="logGood"></param>
        /// <param name="endCycle"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGood"></param>
        /// <exception cref="Exception"></exception>
        private void FullClickWithCheck(HtmlElement fullClickElement, string xpathCheckElement, int timeoutCheckElement,
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
        /// <param name="clickXpath"></param>
        /// <param name="xpathCheckElement"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="emulation"></param>
        /// <param name="logGood"></param>
        /// <param name="endCycle"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="indexClickElement"></param>
        /// <param name="showInPosterGood"></param>
        /// <exception cref="Exception"></exception>
        private void ClickWithCheck(string xpathClick, string xpathCheckElement,
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
        /// <param name="timeoutCheckElement"></param>
        /// <param name="emulation"></param>
        /// <param name="logGood"></param>
        /// <param name="endCycle"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGood"></param>
        /// <exception cref="Exception"></exception>
        private void ClickWithCheck(HtmlElement clickElement, string xpathCheckElement, string emulation = "None",
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
        /// Нажатие клавиши ENTER
        /// </summary>
        private void ENTER()
        {
            instance.WaitFieldEmulationDelay();
            instance.SendText("{ENTER}", 15);
        }
        /// <summary>
        /// Нажатие клавиши TAB
        /// </summary>
        private void TAB()
        {
            instance.WaitFieldEmulationDelay();
            instance.SendText("{TAB}", 15);
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
            string logGoodFullClick = "Элемент загрузки файла для FullClick найден!", int timeoutFullClick = 10, int endCycleFullClick = 2, int indexFullClick = 0,
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
