using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Device;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class LedPatternsController : ApiController
    {
        public override async Task<string> Get(HttpContext context)
        {
            SirenOfShameDevice device = new SirenOfShameDevice();
            await device.TryConnect();
            if (device.IsConnected)
            {
                var names = device.LedPatterns.Select(i => i.Name);
                return JsonConvert.SerializeObject(names);
            }
            return JsonConvert.SerializeObject(new [] {"No Device Connected"});
        }
    }
}
