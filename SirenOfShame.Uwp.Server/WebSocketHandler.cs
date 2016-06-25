using System;
using System.Linq;
using IotWeb.Common.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SirenOfShame.Uwp.Server.Commands;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Server
{
    /// <summary>
    /// Simple 'echo' web socket server
    /// </summary>
    public class WebSocketHandler : IWebSocketRequestHandler
    {
        public WebSocketHandler()
        {
            SirenService.Instance.Device.Connected += DeviceOnConnected;
            SirenService.Instance.Device.Disconnected += DeviceOnDisconnected;
        }

        public bool WillAcceptRequest(string uri, string protocol)
        {
            return (uri.Length == 0) && (protocol == "echo");
        }

        public void Connected(WebSocket socket)
        {
            _socket = socket;
            socket.DataReceived += OnDataReceived;
            socket.ConnectionClosed += SocketOnConnectionClosed;
            SendType(SirenService.Instance.IsConnected ? "deviceConnected" : "deviceDisconnected");
        }

        private void SocketOnConnectionClosed(WebSocket socket)
        {
            _socket = null;
        }

        private void DeviceOnDisconnected(object sender, EventArgs e)
        {
            SendType("deviceDisconnected");
        }

        private void SendType(string type)
        {
            if (_socket == null) return;
            SendObject(_socket, new OkSocketResult { Type = type });
        }

        private void DeviceOnConnected(object sender, EventArgs eventArgs)
        {
            SendType("deviceConnected");
        }

        private static readonly CommandBase[] Commands = {
            new EchoCommand(),
            new SirenInfoCommand(),
            new PlayLedPatternCommand(),
            new PlayAudioPatternCommand(), 
        };

        private WebSocket _socket;

        async void OnDataReceived(WebSocket socket, string frame)
        {
            try
            {
                if (string.IsNullOrEmpty(frame)) return;
                var request = JsonConvert.DeserializeAnonymousType(frame, new {type = ""});
                var controller = Commands.FirstOrDefault(i => i.CommandName == request.type);
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