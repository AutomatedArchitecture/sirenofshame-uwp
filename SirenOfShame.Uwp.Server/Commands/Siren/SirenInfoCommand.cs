using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Commands.Siren
{
    internal class SirenInfoCommand : CommandBase
    {
        public override string CommandName => "getSirenInfo";
        private readonly SirenDeviceService _sirenDeviceService;

        public SirenInfoCommand()
        {
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();
        }

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var sirenInfo = new SirenInfo
            {
                LedPatterns = _sirenDeviceService.LedPatterns,
                AudioPatterns = _sirenDeviceService.AudioPatterns
            };

            return new SirenInfoResult(sirenInfo);
        }
    }
}