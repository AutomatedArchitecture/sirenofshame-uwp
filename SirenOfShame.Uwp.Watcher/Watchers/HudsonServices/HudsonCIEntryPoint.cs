using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.HudsonServices
{
    public class HudsonCIEntryPoint : ICiEntryPoint
    {
        public Task<MyBuildDefinition[]> GetProjects(GetProjectsArgs getProjectsArgs)
        {
            return new HudsonService().GetProjects(getProjectsArgs);
        }

        public string Name => "Hudson";

        public string DisplayName => "Jenkins/Hudson";

        public WatcherBase GetWatcher(SirenOfShameSettings settings)
        {
            return new HudsonWatcher(settings, this);
        }
    }
}
