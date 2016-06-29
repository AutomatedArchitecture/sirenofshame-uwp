using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher
{
    public interface ICiEntryPoint
    {
        //ConfigureServerBase CreateConfigurationWindow(SirenOfShameSettings settings, CiEntryPointSetting ciEntryPointSetting);
        string Name { get; }
        string DisplayName { get; }
        WatcherBase GetWatcher(SirenOfShameSettings settings);
    }
}
