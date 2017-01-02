using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Server;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.TestServer.Services
{
    class WebServer : IWebServer
    {
        private HttpServer _httpServer;

        public void Start()
        {
            _httpServer = new HttpServer(8001);
            _httpServer.AddHttpRequestHandler(
                "/",
                new HttpResourceHandler(typeof(MainTestPage).GetTypeInfo().Assembly,
                    "wwwroot", "index.html"));
            _httpServer.AddWebSocketRequestHandler(
                "/sockets/",
                new WebSocketHandler()
                );
            _httpServer.Start();
        }
    }
}
