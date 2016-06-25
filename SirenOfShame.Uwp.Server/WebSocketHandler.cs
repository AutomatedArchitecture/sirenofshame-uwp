using System;
using System.Linq;
using IotWeb.Common.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SirenOfShame.Uwp.Background.Controllers;
using SirenOfShame.Uwp.Background.Models;

namespace SirenOfShame.Uwp.Background
{
    /// <summary>
    /// Simple 'echo' web socket server
    /// </summary>
    public class WebSocketHandler : IWebSocketRequestHandler
    {
        public bool WillAcceptRequest(string uri, string protocol)
        {
            return (uri.Length == 0) && (protocol == "echo");
        }

        public void Connected(WebSocket socket)
        {
            socket.DataReceived += OnDataReceived;
        }

        private static readonly ControllerBase[] Controllers = {
            new EchoController(),
            new SirenInfoController(),
            new PlayLedPatternController()
        };

        async void OnDataReceived(WebSocket socket, string frame)
        {
            try
            {
                if (string.IsNullOrEmpty(frame)) return;
                var request = JsonConvert.DeserializeAnonymousType(frame, new {type = ""});
                var controller = Controllers.FirstOrDefault(i => i.CommandName == request.type);
                if (controller == null)
                {
                    SendObject(socket, new ErrorResult(404, "No controller associated with type: " + request.type));
                    return;
                }
                var result = await controller.Invoke(frame);
                SendObject(socket, result);
            }
            catch (Exception ex)
            {
                SendObject(socket, new ErrorResult(500, ex.ToString()));
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
}