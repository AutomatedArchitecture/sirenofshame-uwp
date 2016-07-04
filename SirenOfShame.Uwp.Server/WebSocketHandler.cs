using System;
using System.Linq;
using System.Threading.Tasks;
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
            SendConnectionChanged(SirenService.Instance.IsConnected);
        }

        private void SocketOnConnectionClosed(WebSocket socket)
        {
            _socket = null;
        }

        private void DeviceOnDisconnected(object sender, EventArgs e)
        {
            SendConnectionChanged(false);
        }

        private void DeviceOnConnected(object sender, EventArgs eventArgs)
        {
            SendConnectionChanged(true);
        }

        private void SendConnectionChanged(bool isConnected)
        {
            if (_socket == null) return;
            SendObject(_socket, new DeviceConnectionChangedResult(isConnected));
        }

        private static readonly CommandBase[] Commands = {
            new EchoCommand(),
            new SirenInfoCommand(),
            new PlayLedPatternCommand(),
            new PlayAudioPatternCommand(), 
            new GetProjectsCommand(), 
        };

        private WebSocket _socket;

        async void OnDataReceived(WebSocket socket, string frame)
        {
            if (string.IsNullOrEmpty(frame)) return;
            var request = JsonConvert.DeserializeAnonymousType(frame, new {type = ""});
            var requestType = request.type;
            var socketResult = await GetResponse(requestType, frame);
            socketResult.Type = requestType;
            SendObject(socket, socketResult);
        }

        private async Task<SocketResult> GetResponse(string requestType, string frame)
        {
            try
            {
                var controller = Commands.FirstOrDefault(i => i.CommandName == requestType);
                if (controller == null)
                {
                    return new ErrorResult(404, "No controller associated with type: " + requestType);
                }
                var result = await controller.Invoke(frame);
                return result;
            }
            catch (Exception ex)
            {
                return new ErrorResult(500, ex.ToString());
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