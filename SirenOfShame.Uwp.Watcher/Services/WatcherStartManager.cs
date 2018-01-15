using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class WatcherStartManager : StartManagerBase
    {
        private SirenOfShameSettings _sirenOfShameSettings;

        public async Task RegisterSirenOfShameSettings()
        {
            var settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
            _sirenOfShameSettings = await settingsIoService.GetFromDiskOrDefault();
            ServiceContainer.Register(() => _sirenOfShameSettings);
        }

        protected override void RegisterServices()
        {
            ServiceContainer.Register(() => new RulesEngine());
            ServiceContainer.Register(() => new SettingsIoService());
            ServiceContainer.Register(() => new SosDb());
        }

        public override void Stop()
        {
        }
    }
}