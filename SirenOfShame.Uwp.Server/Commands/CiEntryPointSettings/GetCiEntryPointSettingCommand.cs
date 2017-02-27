using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class GetCiEntryPointSettingCommand : CommandBase<GetCiEntryPointSettingRequest>
    {
        public override string CommandName => "getCiEntryPointSetting";

        protected override async Task<SocketResult> Invoke(GetCiEntryPointSettingRequest request)
        {
            await Task.Yield();
            var appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
            var ciEntryPointSetting = appSettings.CiEntryPointSettings.FirstOrDefault(i => i.Id == request.Id);
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
