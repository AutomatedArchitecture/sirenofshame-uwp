using System.Reflection;
using Windows.ApplicationModel.Background;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Background
{
    // ReSharper disable once UnusedMember.Global
    public sealed class StartupTask : IBackgroundTask
    {
        // ReSharper disable once NotAccessedField.Local
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private HttpServer _httpServer;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
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
