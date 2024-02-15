using Global.ZennoExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelperV2
{
    public class WorkList : Main
    {
        public WorkList(Instance instance, IZennoPosterProjectModel project) : base(instance, project)
        {
                
        }
        /// <summary>
        /// Удаляет пустые строки
        /// </summary>
        /// <param name="list"></param>
        public void DeleteEmpty(List<string> list)
        {
            string data = string.Empty;
            for (int i = list.Count - 1; i >= 0; i--)

            {
                data = list[i];
                if (String.IsNullOrWhiteSpace(data.Trim())) list.RemoveAt(i);
            }
        }

        public int CountContainsValue(List<string> list, string value)
        {
            int count = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == value) count++;
            }
            return count;
        }

        /// <summary>
        /// Удаление строк по значению
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int DeleteRow(IZennoList list, string value)
        {
            int count = 0;
            lock (SyncObjects.ListSyncer)
            {
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].Contains(value))
                    {
                        list.RemoveAt(i);
                        count++;
                        i--;
                    }
                }
            }
            project.SendToLog($"Завершение работы метода DeleteRowContains", LogType.Info, false, LogColor.Gray);
            return count;
        }

        /// <summary>
        /// Получение строки по значению
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetItemByValue(IZennoList list, string value)
        {
            lock (SyncObjects.TableSyncer)
            {
                foreach (var item in list)
                {
                    if (item.Contains(value))
                    {
                        project.SendToLog($"Завершение работы метода GetItemByValue", LogType.Info, false, LogColor.Gray);
                        return item;
                    }
                }
            }
            project.SendToLog($"Завершение работы метода GetItemByValue c результатом null", LogType.Info, false, LogColor.Gray);
            return null;
        }


    }
}
