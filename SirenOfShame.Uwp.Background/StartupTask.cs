using System;
using System.Reflection;
using Windows.ApplicationModel.Background;
using IotWeb.Common.Http;
using IotWeb.Common.Util;
using SirenOfShame.Uwp.Background.Services;
using IotWeb.Server;

namespace SirenOfShame.Uwp.Background
{
    public sealed class StartupTask : IBackgroundTask
    {
        // ReSharper disable once NotAccessedField.Local
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private HttpServer _httpServer;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            _httpServer = new HttpServer(8001);
            _httpServer.AddHttpRequestHandler(
                "/",
                new HttpResourceHandler(typeof(StartupTask).GetTypeInfo().Assembly,
                    "wwwroot", "index.html"));
            _httpServer.Start();
            SirenService.Instance.StartWatching();
        }
    }
}
