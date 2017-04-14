using System.Reflection;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Background.Services
{
    internal class WebServer : IWebServer
    {
        private HttpServer _httpServer;

        public void Start()
        {
            _httpServer = new HttpServer(80);
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
