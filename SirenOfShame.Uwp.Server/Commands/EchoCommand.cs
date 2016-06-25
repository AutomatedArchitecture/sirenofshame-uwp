using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class EchoCommand : CommandBase
    {
        public override string CommandName => "echo";

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var echoRequest = JsonConvert.DeserializeAnonymousType(frame, new { type = "", message = "" });
            return new EchoResult(echoRequest.message);
        }
    }
}