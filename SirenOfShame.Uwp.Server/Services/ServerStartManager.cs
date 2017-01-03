using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
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
            await base.Start();
            await RegisterSirenOfShameSettings();
            SetDependencies();
            _webServer.Start();
            _sirenDeviceService.StartWatching();
            await StartMessageRelayService();
            await StartCiWatcher();
        }

        private void SetDependencies()
        {
            _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
            _messageCommandProcessor = ServiceContainer.Resolve<MessageCommandProcessor>();
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();
            _webServer = ServiceContainer.Resolve<IWebServer>();
        }

        public async Task StartCiWatcher()
        {
            try
            {
                var rulesEngine = ServiceContainer.Resolve<RulesEngine>();
                rulesEngine.SetLights += RulesEngineOnSetLights;
                rulesEngine.SetAudio += RulesEngineOnSetAudio;
                rulesEngine.RefreshStatus += RulesEngineOnRefreshStatus;
                rulesEngine.NewNewsItem += RulesEngineOnNewNewsItem;
                rulesEngine.NewUser += RulesEngineOnNewUser;
                rulesEngine.StatsChanged += RulesEngineOnStatsChanged;
                await rulesEngine.Start(true);
            }
            catch (Exception ex)
            {
                _log.Error("Error starting CI watcher", ex);
            }
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

        private async void RulesEngineOnStatsChanged(object sender, StatsChangedEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("StatsChanged", argsAsJson);
        }

        private async void RulesEngineOnNewUser(object sender, NewUserEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("NewUser", argsAsJson);
        }

        private async void RulesEngineOnNewNewsItem(object sender, NewNewsItemEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("NewNewsItem", argsAsJson);
        }

        private async void RulesEngineOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("RefreshStatus", argsAsJson);
        }

        private async void RulesEngineOnSetAudio(object sender, SetAudioEventArgs args)
        {
            if (_sirenDeviceService.IsConnected)
            {
                await _sirenDeviceService.PlayAudioPattern(args.AudioPattern, args.TimeSpan);
            }
        }

        private async void RulesEngineOnSetLights(object sender, SetLightsEventArgs args)
        {
            if (_sirenDeviceService.IsConnected)
            {
                await _sirenDeviceService.PlayLightPattern(args.LedPattern, args.TimeSpan);
            }
        }

        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register(() => new MessageRelayService());
            ServiceContainer.Register(() => new SirenDeviceService());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
            ServiceContainer.Register(() => new MessageCommandProcessor());
        }

        public override void Stop()
        {
            base.Stop();
            _messageRelayService.CloseConnection();
        }
    }
}
