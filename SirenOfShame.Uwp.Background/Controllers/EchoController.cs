using Newtonsoft.Json;
using SirenOfShame.Uwp.Background.Models;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class EchoController : ControllerBase
    {
        public override string CommandName => "echo";

        public override SocketResult Invoke(string frame)
        {
            var echoRequest = JsonConvert.DeserializeAnonymousType(frame, new { type = "", message = "" });
            return new EchoResult(echoRequest.message);
        }
    }
}