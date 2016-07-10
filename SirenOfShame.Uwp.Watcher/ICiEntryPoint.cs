using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher
{
    public interface ICiEntryPoint
    {
        string Name { get; }
        string DisplayName { get; }
        WatcherBase GetWatcher(SirenOfShameSettings settings);
    }
}
