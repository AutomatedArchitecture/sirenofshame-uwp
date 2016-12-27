using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher
{
    public class GetProjectsArgs
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public interface ICiEntryPoint
    {
        string Name { get; }
        string DisplayName { get; }
        WatcherBase GetWatcher(SirenOfShameSettings settings);
        Task<MyBuildDefinition[]> GetProjects(GetProjectsArgs getProjectsArgs);
    }
}
