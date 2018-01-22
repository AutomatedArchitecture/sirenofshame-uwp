using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class EchoCommand : CommandBase
    {
        public override string CommandName => CommandNames.ECHO;
        private readonly ServerMessageRelayService _messageRelayService;

        public EchoCommand()
        {
            _messageRelayService = ServiceContainer.Resolve<ServerMessageRelayService>();
        }

        public override async Task<SocketResult> Invoke(string frame)
        {
            var echoRequest = JsonConvert.DeserializeAnonymousType(frame, new { type = "", message = "" });
            await _messageRelayService.SendMessageAsync(MessageDestination.WebUi, "ToUi", echoRequest.message);
            return new EchoResult(echoRequest.message);
        }
    }
}