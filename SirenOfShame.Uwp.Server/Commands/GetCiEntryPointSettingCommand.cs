using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class GetCiEntryPointSettingCommand : CommandBase
    {
        private SirenOfShameSettingsService _sosService;

        public GetCiEntryPointSettingCommand()
        {
            _sosService = SirenOfShameSettingsService.Instance;
        }

        public override string CommandName => "getCiEntryPointSetting";
        public override async Task<SocketResult> Invoke(string frame)
        {
            var appSettings = await _sosService.GetAppSettings();
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
