using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class StartManagerBase
    {
        private SirenOfShameSettings _sirenOfShameSettings;

        public void Configure()
        {
            RegisterServices();
        }

        public async Task RegisterSirenOfShameSettings()
        {
            var settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
            _sirenOfShameSettings = await settingsIoService.GetFromDiskOrDefault();
            ServiceContainer.Register(() => _sirenOfShameSettings);
        }

        protected virtual void RegisterServices()
        {
            ServiceContainer.Register(() => new RulesEngine());
            ServiceContainer.Register(() => new SettingsIoService());
        }
    }
}