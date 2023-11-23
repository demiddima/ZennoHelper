using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
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

    }
}
