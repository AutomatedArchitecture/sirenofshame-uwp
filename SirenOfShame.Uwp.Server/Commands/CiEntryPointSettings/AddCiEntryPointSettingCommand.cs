using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    public class AddCiEntryPointSettingRequest : RequestBase
    {
        public InMemoryCiEntryPointSetting CiEntryPointSetting { get; set; }
    }

    internal class AddCiEntryPointSettingCommand : CommandBase<AddCiEntryPointSettingRequest>
    {
        public override string CommandName => CommandNames.ADD_CI_ENTRY_POINT_SETTING;
        protected override async Task<SocketResult> Invoke(AddCiEntryPointSettingRequest request)
        {
            var ciEntryPointSettingService = ServiceContainer.Resolve<CiEntryPointSettingService>();
            await ciEntryPointSettingService.AddUpdate(request.CiEntryPointSetting);
            return new OkSocketResult
            {
                Result = request.CiEntryPointSetting.Id
            };
        }
    }
}
