using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class GetCiEntryPointSettingsCommand : CommandBase
    {
        public override string CommandName => "getCiEntryPointSettings";
        public override async Task<SocketResult> Invoke(string frame)
        {
            var sosService = SirenOfShameSettingsService.Instance;
            var sirenOfShameSettings = await sosService.GetAppSettings();
            return new GetCiEntryPointSettingsResult(sirenOfShameSettings.CiEntryPointSettings);
        }
    }

    internal class GetCiEntryPointSettingsResult : SocketResult
    {
        public GetCiEntryPointSettingsResult(IEnumerable<CiEntryPointSetting> projects)
        {
            ResponseCode = 200;
            Result = projects;
        }
    }
}
