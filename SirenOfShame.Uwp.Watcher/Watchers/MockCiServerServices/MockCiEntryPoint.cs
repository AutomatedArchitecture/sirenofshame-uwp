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
    }
}
