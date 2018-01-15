using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class WatcherStartManager : StartManagerBase
    {
        private SirenOfShameSettings _sirenOfShameSettings;

        public override async Task Start()
        {
            await base.Start();
            await InitializeLogging();
        }

        private async Task InitializeLogging()
        {
            var watcherLogManager = ServiceContainer.Resolve<WatcherLogManager>();
            await watcherLogManager.Initialize();
            var log = MyLogManager.GetLog(typeof(WatcherStartManager));
            await log.Info("Watcher Logging Initialized");
        }

        public async Task RegisterSirenOfShameSettings()
        {
            var settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
            _sirenOfShameSettings = await settingsIoService.GetFromDiskOrDefault();
            ServiceContainer.Register(() => _sirenOfShameSettings);
        }

        protected override void RegisterServices()
        {
            ServiceContainer.Register(() => new WatcherLogManager());
            ServiceContainer.Register(() => new RulesEngine());
            ServiceContainer.Register(() => new SettingsIoService());
            ServiceContainer.Register(() => new SosDb());
        }

        public override void Stop()
        {
        }
    }
}