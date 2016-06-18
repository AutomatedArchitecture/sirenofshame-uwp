using System;
using System.Reflection;
using Windows.ApplicationModel.Background;
using Windows.Web.Http.Filters;
using IotWeb.Common.Http;
using IotWeb.Common.Util;
using SirenOfShame.Uwp.Background.Services;
using IotWeb.Server;

namespace SirenOfShame.Uwp.Background
{
    /// <summary>
    /// Simple 'echo' web socket server
    /// </summary>
    class WebSocketHandler : IWebSocketRequestHandler
    {

        public bool WillAcceptRequest(string uri, string protocol)
        {
            return (uri.Length == 0) && (protocol == "echo");
        }

        public void Connected(WebSocket socket)
        {
            socket.DataReceived += OnDataReceived;
        }

        void OnDataReceived(WebSocket socket, string frame)
        {
            socket.Send(frame);
        }
    }

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
            ;
            _httpServer.AddHttpRequestHandler(
                "/",
                new HttpResourceHandler(typeof(StartupTask).GetTypeInfo().Assembly,
                    "wwwroot", "index.html"));
            _httpServer.Start();
            SirenService.Instance.StartWatching();
        }
    }
}
