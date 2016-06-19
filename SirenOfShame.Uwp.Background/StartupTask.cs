using System;
using System.Reflection;
using Windows.ApplicationModel.Background;
using IotWeb.Common.Http;
using SirenOfShame.Uwp.Background.Services;
using IotWeb.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            try
            {
                if (string.IsNullOrEmpty(frame)) return;
                var request = JsonConvert.DeserializeAnonymousType(frame, new {type = ""});
                if (request.type == "echo")
                {
                    var echoRequest = JsonConvert.DeserializeAnonymousType(frame, new {type = "", message = ""});
                    var echoResult = new
                    {
                        responseCode = 200,
                        response = "OK",
                        type = "echoResult",
                        result = echoRequest.message
                    };
                    SendObject(socket, echoResult);
                }
                else
                {
                    var sirenInfo = new
                    {
                        responseCode = 200,
                        response = "OK",
                        type = "getSirenInfoResult",
                        result = new
                        {
                            ledPatterns = SirenService.Instance.LedPatterns,
                            audioPatterns = SirenService.Instance.AudioPatterns
                        }
                    };
                    SendObject(socket, sirenInfo);
                }
            }
            catch (Exception ex)
            {
                var errorInfo = new
                {
                    responseCode = 500,
                    response = ex.Message,
                    type = "error"
                };
                var result = JsonConvert.SerializeObject(errorInfo);
                socket.Send(result);
            }
        }

        private static void SendObject(WebSocket socket, object echoResult)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var result = JsonConvert.SerializeObject(echoResult, jsonSerializerSettings);
            socket.Send(result);
        }
    }

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
