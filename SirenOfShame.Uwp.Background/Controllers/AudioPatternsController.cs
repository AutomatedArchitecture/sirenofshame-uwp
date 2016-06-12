using System.Threading.Tasks;
using SirenOfShame.Uwp.Background.Services;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal sealed class AudioPatternsController : ApiController
    {
        public override async Task<object> Get(HttpContext context)
        {
            if (SirenService.Instance.Device.IsConnected)
            {
                return SirenService.Instance.Device.AudioPatterns;
            }
            return new[] { "No Device Connected" };
        }
    }
}
