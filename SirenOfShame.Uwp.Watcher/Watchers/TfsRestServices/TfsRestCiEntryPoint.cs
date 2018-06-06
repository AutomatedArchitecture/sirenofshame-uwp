using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace TfsRestServices
{
    public class TfsRestCiEntryPoint : ICiEntryPoint
    {
        public Task<MyBuildDefinition[]> GetProjects(GetProjectsArgs getProjectsArgs)
        {
            return new TfsRestService().GetProjects(getProjectsArgs);
        }

        public string Name => "TfsRest";

        public string DisplayName => "Microsoft TFS 2015+";

        public WatcherBase GetWatcher(SirenOfShameSettings settings)
        {
            return new TfsRestWatcher(settings, this);
        }
    }
}
