using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class StartManagerBase
    {
        private SirenOfShameSettings _sirenOfShameSettings;

        public virtual async Task Start()
        {
            RegisterServices();
            await Task.Yield();
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
            ServiceContainer.Register(() => new SosDb());
        }

        public virtual void Stop()
        {
        }
    }
}