using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Background
{
    // ReSharper disable once UnusedMember.Global
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private HttpServer _httpServer;
        private RulesEngine _rulesEngine;
        private readonly StartManager _startManager = new StartManager();
        private MessageRelayService _messageRelayService;
        private readonly ILog _log = MyLogManager.GetLog(typeof(StartupTask));
        private SirenDeviceService _sirenDeviceService;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += TaskInstanceOnCanceled;
            _startManager.Configure();

            _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();

            StartWebServer();
            _sirenDeviceService.StartWatching();
            await StartMessageRelayService();
            await StartCiWatcher();
        }

        private async Task StartMessageRelayService()
        {
            try
            {
                await _messageRelayService.Open();
            }
            catch (Exception ex)
            {
                _log.Error("Unable to start message rleay service", ex);
            }
        }

        private void TaskInstanceOnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _messageRelayService.CloseConnection();
            _backgroundTaskDeferral?.Complete();
            _backgroundTaskDeferral = null;
        }

        private async Task StartCiWatcher()
        {
            var sosSettings = await SirenOfShameSettingsService.Instance.GetAppSettings();
            _rulesEngine = new RulesEngine(sosSettings);
            _rulesEngine.Start(true);
        }

        private void StartWebServer()
        {
            _httpServer = new HttpServer(8001);
            _httpServer.AddWebSocketRequestHandler(
                "/sockets/",
                new WebSocketHandler()
                );
            _httpServer.AddHttpRequestHandler(
                "/",
                new HttpResourceHandler(typeof(StartupTask).GetTypeInfo().Assembly,
                    "wwwroot", "index.html"));
            _httpServer.Start();
        }
    }
}
