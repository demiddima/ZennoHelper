using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelperV2
{
    public class Management : Main
    {
        public Management(Instance instance, IZennoPosterProjectModel project) : base(instance, project)
        {
        }

        public void StopTemplate()
        {
            ZennoPoster.SetTries(new Guid(project.TaskId), 0);
            ZennoPoster.StopTask(Guid.Parse(project.TaskId));
        }

    }
}
