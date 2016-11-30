using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
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
        private AppServiceConnection _connection;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            ListenForMessagesFromUi(taskInstance);
            StartWebServer();
            await StartCiWatcher();
        }

        private void ListenForMessagesFromUi(IBackgroundTaskInstance taskInstance)
        {
            var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;
            _connection = triggerDetails.AppServiceConnection;
            _connection.RequestReceived += ConnectionRequestReceived;
        }

        private async void ConnectionRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var name = (string)args.Request.Message.First().Value;
            System.Diagnostics.Debug.WriteLine("Received message: " + name);
            ValueSet message = new ValueSet
            {
                new KeyValuePair<string, object>("Hello", "Hello From Server")
            };
            await _connection.SendMessageAsync(message);
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
            SirenDeviceService.Instance.StartWatching();
        }
    }
}
