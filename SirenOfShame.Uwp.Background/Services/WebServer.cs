using System;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Windows.Networking.Connectivity;
using IotWeb.Common.Http;
using IotWeb.Server;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
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
            NetworkInformation.NetworkStatusChanged += NetworkInformationOnNetworkStatusChanged;
            var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkAvailable)
            {
                StartWebServer();
            }
            else
            {
                _log.Info("Network unavailable.  Will start web server when connectivity returns.");
            }
        }

        readonly SemaphoreSlim _slim = new SemaphoreSlim(0);

        private void NetworkInformationOnNetworkStatusChanged(object sender)
        {
            _slim.Wait();
            try
            {
                var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
                if (isNetworkAvailable)
                {
                    _httpServer?.Stop();
                    _httpServer = null;
                    StartWebServer();
                }
            }
            finally
            {
                _slim.Release();
            }
        }

        private void StartWebServer()
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
