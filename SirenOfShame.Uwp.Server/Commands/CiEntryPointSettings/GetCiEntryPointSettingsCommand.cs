using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class GetCiEntryPointSettingsCommand : CommandBase
    {
        public override string CommandName => CommandNames.GET_CI_ENTRY_POINT_SETTINGS;
        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var sirenOfShameSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
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
