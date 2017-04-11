using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Services
{
    /// <summary>
    /// Responsible for starting all services who must be started when the parent process
    /// starts and stopping them gracefully when the parent process ends.
    /// </summary>
    public abstract class ServerStartManager : StartManagerBase
    {
        private MessageRelayService _messageRelayService;
        private MessageCommandProcessor _messageCommandProcessor;
        private SirenDeviceService _sirenDeviceService;
        private readonly ILog _log = MyLogManager.GetLog(typeof(ServerStartManager));
        private IWebServer _webServer;

        public override async Task Start()
        {
            try
            {
                await base.Start();
                await RegisterSirenOfShameSettings();
                SetDependencies();
                _webServer.Start();
                _sirenDeviceService.StartWatching();
                await StartMessageRelayService();
                await StartCiWatcher();
            }
            catch (Exception ex)
            {
                _log.Error("Error during startup", ex);
            }
        }

        private void SetDependencies()
        {
            _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
            _messageCommandProcessor = ServiceContainer.Resolve<MessageCommandProcessor>();
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();
            _webServer = ServiceContainer.Resolve<IWebServer>();
        }

        private async Task StartCiWatcher()
        {
            var rulesEngineService = ServiceContainer.Resolve<RulesEngineService>();
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
                _log.Error("Unable to start message rleay service", ex);
            }
        }

        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register(() => new MessageRelayService());
            ServiceContainer.Register(() => new SirenDeviceService());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
            ServiceContainer.Register(() => new MessageCommandProcessor());

            // Services
            ServiceContainer.Register(() => new CiEntryPointSettingService());
            ServiceContainer.Register<CryptographyServiceBase>(() => new CryptographyService());
            ServiceContainer.Register(() => new RulesEngineService());
        }

        public override void Stop()
        {
            base.Stop();
            _messageRelayService.CloseConnection();
        }
    }
}
