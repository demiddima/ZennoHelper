using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoHelper
{
    public class ZennoHelper
    {
        public Log log;
        public WebDesktop webDesktop;
        public WebPhone webPhone;
        public Instance instance;
        public IZennoPosterProjectModel project;

        public ZennoHelper(Instance _instance, IZennoPosterProjectModel _project) 
        { 
            instance = _instance;
            project = _project;
            log = new Log(_instance,_project);
            webDesktop = new WebDesktop(_instance, _project);
            webPhone = new WebPhone(_instance, _project);
        }

    }
}
