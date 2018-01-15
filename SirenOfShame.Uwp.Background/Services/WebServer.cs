using System.Reflection;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Background.Services
{
    internal class WebServer : IWebServer
    {
        private HttpServer _httpServer;
        private readonly ILog _log = MyLogManager.GetLog(typeof(WebSocketHandler));

        public void Start()
        {
            _httpServer = new HttpServer(80);
            _httpServer.AddWebSocketRequestHandler(
                "/sockets/",
                new WebSocketHandler()
                );
            var backgroundAssembly = typeof(StartupTask).GetTypeInfo().Assembly;
            var indexHtmlResourceHandler = new HttpResourceHandler(backgroundAssembly, "wwwroot", "index.html");
            _httpServer.AddHttpRequestHandler("/", indexHtmlResourceHandler);
            _httpServer.AddHttpRequestHandler("/settings", indexHtmlResourceHandler);
            _httpServer.AddHttpRequestHandler("/home", indexHtmlResourceHandler);
            _httpServer.AddHttpRequestHandler("/mockServer", indexHtmlResourceHandler);

            _log.Info("Starting web server");
            _httpServer.Start();
        }
    }
}
