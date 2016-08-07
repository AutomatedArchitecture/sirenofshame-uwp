using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class SirenInfoCommand : CommandBase
    {
        public override string CommandName => "getSirenInfo";

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var sirenInfo = new SirenInfo
            {
                LedPatterns = SirenDeviceService.Instance.LedPatterns,
                AudioPatterns = SirenDeviceService.Instance.AudioPatterns
            };

            return new SirenInfoResult(sirenInfo);
        }
    }
}