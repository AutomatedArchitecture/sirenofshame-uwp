using System;
using System.Threading.Tasks;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class UiStartManager : StartManagerBase
    {
        private ILog _log;

        public override async Task Start()
        {
            try
            {
                await base.Start();
                await InitializeLogging();
                await OpenMessageRelayConnection();
                StartMessageDistributor();
                await _log.Info("StartupManager Startup Complete");
            }
            catch (Exception ex)
            {
                _log?.Error("Error during startup", ex);
            }
        }

        private async Task OpenMessageRelayConnection()
        {
            var connection = ServiceContainer.Resolve<UiMessageRelayService>();
            try
            {
                await connection.Open();
            }
            catch (Exception ex)
            {
                // failing quietly is probably ok for now since the connection will
                //  attempt to re-open itself again on next send.  It just means
                //  we won't be able to receive messages
                await _log.Error("Error opening connection on startup", ex);
            }

        }

        private void StartMessageDistributor()
        {
            var messageDistributorService = ServiceContainer.Resolve<UiCommandProcessor>();
            messageDistributorService.StartWatching();
        }


        private async Task InitializeLogging()
        {
            var uiLogManager = ServiceContainer.Resolve<UiLogManager>();
            await uiLogManager.Initialize();
            _log = MyLogManager.GetLog(typeof(UiStartManager));
            await _log.Info("UI Logging Initialized");
        }

        protected override void RegisterServices()
        {
            ServiceContainer.Register(() => new UiLogManager());
            ServiceContainer.Register(() => new UiMessageRelayService());
            ServiceContainer.Register(() => new UiCommandProcessor());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
            ServiceContainer.Register(() => new NavigationService());
            ServiceContainer.Register(() => new NetworkService());
            ServiceContainer.Register(() => new GettingStartedService());
            ServiceContainer.Register(() => new AppSettingsService());
        }
    }
}
