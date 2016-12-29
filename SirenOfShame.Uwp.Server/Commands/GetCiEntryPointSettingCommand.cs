using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class GetCiEntryPointSettingCommand : CommandBase
    {
        public override string CommandName => "getCiEntryPointSetting";
        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
            // todo: Support multiple ci entry points!!!
            var ciEntryPointSetting = appSettings.CiEntryPointSettings.FirstOrDefault();
            return new GetCiEntryPointSettingResult(ciEntryPointSetting);
        }
    }

    internal class GetCiEntryPointSettingResult : SocketResult
    {
        public GetCiEntryPointSettingResult(CiEntryPointSetting ciEntryPointSetting)
        {
            ResponseCode = 200;
            Result = ciEntryPointSetting;
        }
    }
}
