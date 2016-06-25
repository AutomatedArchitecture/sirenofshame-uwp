using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Background.Models;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class EchoController : ControllerBase
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