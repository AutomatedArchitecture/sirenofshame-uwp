using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Watchers.MockCiServerServices
{
    public class MockCiEntryPoint : ICiEntryPoint
    {
        public string Name => "Mock";

        public string DisplayName => "Mock";

        public WatcherBase GetWatcher(SirenOfShameSettings settings)
        {
            return new MockWatcher(settings);
        }

        public async Task<MyBuildDefinition[]> GetProjects(GetProjectsArgs getProjectsArgs)
        {
            await Task.Yield();
            return MockWatcher.GetBuildDefinitions()
                .Cast<MyBuildDefinition>()
                .ToArray();
        }
    }
}
