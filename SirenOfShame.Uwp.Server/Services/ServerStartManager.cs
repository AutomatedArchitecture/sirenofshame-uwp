using System;
using System.Threading.Tasks;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Services
{
    /// <summary>
    /// Responsible for starting all services who must be started when the parent process
    /// starts and stopping them gracefully when the parent process ends.
    /// </summary>
    public abstract class ServerStartManager : WatcherStartManager
    {
        private ServerMessageRelayService _messageRelayService;
        private MessageCommandProcessor _messageCommandProcessor;
        private SirenDeviceService _sirenDeviceService;
        private ILog _log;
        private IWebServer _webServer;

        public override async Task Start()
        {
            try
            {
                await base.Start();
                await InitializeLogging();
                await RegisterSirenOfShameSettings();
                SetDependencies();
                _webServer.Start();
                _sirenDeviceService.StartWatching();
                await StartMessageRelayService();
                await StartCiWatcher();
            }
            catch (Exception ex)
            {
                _log?.Error("Error during startup", ex);
            }
        }

        private async Task InitializeLogging()
        {
            await WatcherLogManager.Initialize();
            _log = MyLogManager.GetLog(typeof(ServerStartManager));
            await _log.Info("Initializing Server Logging");
        }

        private void SetDependencies()
        {
            _messageRelayService = ServiceContainer.Resolve<ServerMessageRelayService>();
            _messageCommandProcessor = ServiceContainer.Resolve<MessageCommandProcessor>();
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();
            _webServer = ServiceContainer.Resolve<IWebServer>();
        }

        private async Task StartCiWatcher()
        {
            var rulesEngineService = ServiceContainer.Resolve<RulesEngineWatcher>();
            await rulesEngineService.StartCiWatcher();
        }

        public async Task StartMessageRelayService()
        {
            _messageCommandProcessor.StartWatching();
            try
            {
                await _messageRelayService.Open();
            }
            catch (Exception ex)
            {
                await _log.Error("Unable to start message rleay service", ex);
            }
        }

        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register(() => new ServerMessageRelayService());
            ServiceContainer.Register(() => new SirenDeviceService());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
            ServiceContainer.Register(() => new MessageCommandProcessor());

            // Services
            ServiceContainer.Register(() => new CiEntryPointSettingService());
            ServiceContainer.Register<CryptographyServiceBase>(() => new CryptographyService());
            ServiceContainer.Register(() => new RulesEngineWatcher());
        }

        public override void Stop()
        {
            base.Stop();
            _messageRelayService.CloseConnection();
        }
    }
}
