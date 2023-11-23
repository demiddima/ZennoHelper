using System;
using System.IO;
using System.Reflection;
using System.Threading;
using ZennoLab.CommandCenter;
using ZennoLab.Emulation;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelperV2
{
    public class WebPhone : Web
    {
        public WebPhone(Instance instance, IZennoPosterProjectModel project) : base(instance, project)
        {

        }


        /// <summary>
        /// Нажатие по элементу через поиск по xpath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="logGood"></param>
        /// <param name="timeout"></param>
        /// <param name="endCycle"></param>
        /// <param name="index"></param>
        /// <param name="showInPosterGood"></param>
        public void Touch(string xpath, string logGood = "Элемент для Touch найден", int timeout = 10, int endCycle = 2, int index = 0, bool showInPosterGood = false)
        {
            HtmlElement element = GetElement(xpath, logGood, timeout, endCycle, index, showInPosterGood);
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
        /// Установка value элемента через эмуляцию клавиатуры, с предварительным нажатием на него и проверкой после установки
        /// </summary>
        /// <param name="xpathTouch"></param>
        /// <param name="text"></param>
        /// <param name="latency"></param>
        /// <param name="logGoodTouch"></param>
        /// <param name="timeoutTouch"></param>
        /// <param name="endCycleTouch"></param>
        /// <param name="indexTouch"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="showInPosterGoodTouch"></param>
        /// <param name="showInPosterGoodCheck"></param>
		public void SetValueFullTouch(string xpathTouch, string text, int latency = 20,
            string logGoodTouch = "Элемент для Touch найден", int timeoutTouch = 10, int endCycleTouch = 2, int indexTouch = 0, 
            string logGoodCheck = "Value элемента изменено!", bool showInPosterGoodTouch = false, bool showInPosterGoodCheck = false)
        {
            HtmlElement element = GetElement(xpathTouch, logGoodTouch, timeoutTouch, endCycleTouch, indexTouch, showInPosterGoodTouch);
            Touch(element);
            instance.SendText(text, latency);

            CheckValueElement(element, text, logGoodCheck, showInPosterGoodCheck);
        }
        /// <summary>
        /// Установка value элемента через эмуляцию клавиатуры, с предварительным нажатием на него и проверкой после установки
        /// </summary>
        /// <param name="element"></param>
        /// <param name="text"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="latency"></param>
        /// <param name="showInPosterGoodCheck"></param>
        public void SetValueFullTouch(HtmlElement element, string text,
            string logGoodCheck = "Value элемента изменено!", int latency = 20, bool showInPosterGoodCheck = false)
        {
            Touch(element);
            instance.SendText(text, latency);

            CheckValueElement(element, text, logGoodCheck, showInPosterGoodCheck);
        }

        /// <summary>
        /// Нажатие через xpath с проверкой и попытками 
        /// </summary>
        /// <param name="xpathTouch"></param>
        /// <param name="xpathCheck"></param>
        /// <param name="logGoodTouch"></param>
        /// <param name="timeoutTouch"></param>
        /// <param name="endCycleTouch"></param>
        /// <param name="indexTouch"></param>
        /// <param name="logGoodCheck"></param>
        /// <param name="timeoutCheckElement"></param>
        /// <param name="endCycleCheck"></param>
        /// <param name="indexCheck"></param>
        /// <param name="showInPosterGoodTouch"></param>
        /// <param name="showInPosterGoodCheck"></param>
        /// <exception cref="Exception"></exception>
        public void TouchWithCheck(string xpathTouch, string xpathCheck,
            string logGoodTouch = "Элемент для Touch найден", int timeoutTouch = 10, int endCycleTouch = 2, int indexTouch = 0, 
           string logGoodCheck = "Touch по элементу удался!", int timeoutCheckElement = 1000, int endCycleCheck = 10, int indexCheck = 0, 
           bool showInPosterGoodTouch = false, bool showInPosterGoodCheck = false)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    HtmlElement element = GetElement(xpathTouch, logGoodTouch, timeoutTouch, endCycleTouch, indexTouch, showInPosterGoodTouch);
                    Touch(element);

                    GetElement(xpathCheck, logGoodCheck, timeoutCheckElement, endCycleCheck, indexCheck, showInPosterGoodCheck);

                    break;
                }
                catch (Exception ex)
                {
                    if (i == 2)
                    {
                        throw new Exception("Ошибка выполнения Touch: " + ex.Message);
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
        public override void DownloadFile(string pathFile, string xpathFullClick, string xpathCheckDownload, string logGoodFullClick = "Элемент загрузки файла для FullClick найден!", int timeoutFullClick = 10, int endCycleFullClick = 2, int indexFullClick = 0, string logGoodCheckDownload = "Файл успешно загружен!", int timeoutCheckDownload = 500, int endCycleCheckDownload = 10, int indexCheckDownload = 0, bool showInPosterGoodFullClick = false, bool showInPosterGoodCheckDownload = false)
        {
            instance.SetFileUploadPolicy("ok", "");
            instance.SetFilesForUpload(pathFile);

            Touch(xpathFullClick,logGoodFullClick, timeoutFullClick, endCycleFullClick, indexFullClick, showInPosterGoodFullClick);

            GetElement(xpathCheckDownload, logGoodCheckDownload, timeoutCheckDownload, endCycleCheckDownload, indexCheckDownload, showInPosterGoodCheckDownload);
        }



    }
}
