using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class StartManagerBase
    {
        public void Configure()
        {
            RegisterServices();
        }

        protected virtual void RegisterServices()
        {
            ServiceContainer.Register(() => new RulesEngine());
            ServiceContainer.Register(() => new SettingsIoService());
            ServiceContainer.Register(() => 
                new SettingsIoService().GetFromDiskOrDefault().Result);
        }
    }
}