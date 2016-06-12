using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal sealed class AudioPatternsController : ApiController
    {
        public override async Task<string> Get(HttpContext context)
        {
            await Task.Yield();
            string[] audioPatterns = new[]
            {
                "Audio Pattern 1",
                "Audio Pattern 2",
                "Audio Pattern 3",
            };
            return JsonConvert.SerializeObject(audioPatterns);
        }
    }
}
