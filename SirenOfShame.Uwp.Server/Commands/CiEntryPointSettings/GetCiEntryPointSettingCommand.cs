using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class GetCiEntryPointSettingCommand : CommandBase<GetCiEntryPointSettingRequest>
    {
        public override string CommandName => CommandNames.GET_CI_ENTRY_POINT_SETTING;

        protected override async Task<SocketResult> Invoke(GetCiEntryPointSettingRequest request)
        {
            await Task.Yield();
            var ciEntryPointSettingService = ServiceContainer.Resolve<CiEntryPointSettingService>();
            var result = ciEntryPointSettingService.GetByIdForUnencryptedCommunication(request.Id);
            return new GetCiEntryPointSettingResult(result);
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
