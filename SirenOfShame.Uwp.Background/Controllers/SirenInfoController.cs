using System.Threading.Tasks;
using SirenOfShame.Uwp.Background.Models;
using SirenOfShame.Uwp.Background.Services;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class SirenInfoController : ControllerBase
    {
        public override string CommandName => "getSirenInfo";

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var sirenInfo = new SirenInfo
            {
                LedPatterns = SirenService.Instance.LedPatterns,
                AudioPatterns = SirenService.Instance.AudioPatterns
            };

            return new SirenInfoResult(sirenInfo);
        }
    }
}