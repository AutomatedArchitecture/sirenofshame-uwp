using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.HudsonServices
{
    // todo: figure out DI
    //[Export(typeof(ICiEntryPoint))]
    public class HudsonCIEntryPoint : ICiEntryPoint
    {
        //public ConfigureServerBase CreateConfigurationWindow(SirenOfShameSettings settings, CiEntryPointSetting ciEntryPointSetting)
        //{
        //    return new ConfigureHudson(settings, this, ciEntryPointSetting);
        //}

        public string Name
        {
            get { return "Hudson"; }
        }

        public string DisplayName
        {
            get { return "Jenkins/Hudson"; }
        }

        public WatcherBase GetWatcher(SirenOfShameSettings settings)
        {
            return new HudsonWatcher(settings, this);
        }
    }
}
