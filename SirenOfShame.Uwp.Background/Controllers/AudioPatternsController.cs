using System.Threading.Tasks;
using SirenOfShame.Uwp.Background.Services;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal sealed class AudioPatternsController : ApiController
    {
        public override async Task<object> Get(HttpContext context)
        {
            await Task.Yield();
            if (SirenService.Instance.IsConnected)
            {
                return SirenService.Instance.AudioPatterns;
            }
            return new[] { "No Device Connected" };
        }
    }
}
