using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelper
{
    public static class Log
    {
        public static Instance instance;
        public static IZennoPosterProjectModel project;

        /// <summary>
        /// Лог с ошибкой определён по-умолчанию: SendToLog(log, LogType.Error,true ,LogColor.Red)
        /// </summary>
        /// <param name="log">Сообщение в лог</param>
        /// <param name="showInPoster">Разрешить или запретить вывод ошибки в ЗенноПостер</param>
        public static void LogBadEnd(string log, bool showInPoster = true)
        {
            project.SendToLog(log, LogType.Error, showInPoster, LogColor.Red);
        }

        /// <summary>
        /// Лог с информацией определён по-умолчанию: project.SendToLog(log, LogType.Info, true, colorGood);)
        /// </summary>
        /// <param name="log">Сообщение в лог</param>
        /// <param name="showInPoster">Разрешить или запретить вывод информации в ЗенноПостер</param>
        ///  <param name="colorGood">Выбор цвета для лога</param>
        public static void LogGoodEnd(string log, bool showInPoster = false, LogColor colorGood = LogColor.Green)
        {
            project.SendToLog(log, LogType.Info, showInPoster, colorGood);
        }

        /// <summary>
        /// Лог с предупреждением(определён: SendToLog(log, LogType.Warning, true, colorGood))
        /// </summary>
        /// <param name="log">Сообщение в лог</param>
        /// <param name="showInPoster">Разрешить или запретить вывод предупреждения в ЗенноПостер</param>
        /// <param name="colorGood">Выбрать цвет для предупреждения</param>
        public static void LogWarningEnd(string log, bool showInPoster = true)
        {
            project.SendToLog(log, LogType.Warning, showInPoster, LogColor.Yellow);
        }
    }
}
