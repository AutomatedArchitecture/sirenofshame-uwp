using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Background
{
    // ReSharper disable once UnusedMember.Global
    public sealed class StartupTask : IBackgroundTask
    {
        // ReSharper disable once NotAccessedField.Local
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private HttpServer _httpServer;
        private RulesEngine _rulesEngine;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            StartWebServer();
            await StartCiWatcher();
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
            SirenService.Instance.StartWatching();
        }
    }
}
