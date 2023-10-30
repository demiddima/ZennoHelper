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
         
        public WebDesktop(Instance instance, IZennoPosterProjectModel project) :base(instance, project)
        {

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
        /// 
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        public void FullClick(string xpath,string logGood, int timeout = 10, int endCycle = 2, int index = 0, bool showInPosterGood = false)
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
        public void Click(string xpath, string logGood, int timeout = 10, int endCycle = 2, int index = 0, string emulation = "None", bool showInPosterGood = false)
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
            int endCycleCheckElement = 10, int indexCheckElement = 0,bool showInPosterGoodCheckElement = false)
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
        /// <param name="timeoutCheckElement"></param>
        /// <param name="emulation"></param>
        /// <param name="logGood"></param>
        /// <param name="endCycle"></param>
        /// <param name="indexCheckElement"></param>
        /// <param name="showInPosterGood"></param>
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
        /// Нажатие клавиши ENTER
        /// </summary>
        public void ENTER()
        {
            instance.WaitFieldEmulationDelay();
            instance.SendText("{ENTER}", 15);
        }
        /// <summary>
        /// Нажатие клавиши TAB
        /// </summary>
        public void TAB()
        {
            instance.WaitFieldEmulationDelay();
            instance.SendText("{TAB}", 15);
        }


        public override void CheckValueElement(HtmlElement element, string text, string logGood, bool showInPosterGood = false)
        {
            base.CheckValueElement(element, text, logGood, showInPosterGood);
        }

    }
}
