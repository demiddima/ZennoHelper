using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelperV2
{
    public class Main
    {
        protected Instance instance;
        protected IZennoPosterProjectModel project;

        public Main(Instance newInstance, IZennoPosterProjectModel newProject)
        {
            instance = newInstance;
            project = newProject;
        }
    }
}
