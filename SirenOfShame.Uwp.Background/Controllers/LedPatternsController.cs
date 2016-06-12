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
                await SirenService.Instance.Device.PlayLightPattern(new LedPattern { Id = 1 }, new TimeSpan(0, 0, 10));
            }
        }
    }
}
