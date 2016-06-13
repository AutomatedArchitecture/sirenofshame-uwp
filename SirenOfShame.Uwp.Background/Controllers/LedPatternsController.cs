using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Device;
using SirenOfShame.Uwp.Background.Services;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class LedPatternsController : ApiController
    {
        public override async Task<object> Get(HttpContext context)
        {
            if (SirenService.Instance.Device.IsConnected)
            {
                return SirenService.Instance.Device.LedPatterns;
            }
            return new [] {"No Device Connected"};
        }

        public override async Task Post(HttpContext context)
        {
            if (SirenService.Instance.Device.IsConnected)
            {
                var id = context.GetQuerystringParam("id");
                var ledPattern = ToLedPattern(id);
                await SirenService.Instance.Device.PlayLightPattern(ledPattern, null);
            }
        }

        private LedPattern ToLedPattern(string id)
        {
            if (string.IsNullOrEmpty(id)) return new LedPattern();
            return new LedPattern {Id = int.Parse(id)};
        }
    }
}
