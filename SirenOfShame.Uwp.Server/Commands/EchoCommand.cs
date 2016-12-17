using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class EchoCommand : CommandBase
    {
        public override string CommandName => "echo";
        private MessageRelayService _messageRelayService;

        public EchoCommand()
        {
            _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        }

        public override async Task<SocketResult> Invoke(string frame)
        {
            var echoRequest = JsonConvert.DeserializeAnonymousType(frame, new { type = "", message = "" });
            await _messageRelayService.Send("ToUi", echoRequest.message);
            return new EchoResult(echoRequest.message);
        }
    }
}