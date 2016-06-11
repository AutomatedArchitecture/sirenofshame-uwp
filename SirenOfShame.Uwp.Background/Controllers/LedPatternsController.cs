using Newtonsoft.Json;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class LedPatternsController : ApiController
    {
        public override string Get(HttpContext context)
        {
            string[] audioPatterns = new[]
            {
                "LED Pattern 1",
                "LED Pattern 2",
                "LED Pattern 3",
            };
            return JsonConvert.SerializeObject(audioPatterns);
        }
    }
}
