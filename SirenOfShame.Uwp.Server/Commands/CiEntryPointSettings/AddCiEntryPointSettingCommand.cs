using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    public class AddCiEntryPointSettingRequest : RequestBase
    {
        public CiEntryPointSetting CiEntryPointSetting { get; set; }
    }

    internal class AddCiEntryPointSettingCommand : CommandBase<AddCiEntryPointSettingRequest>
    {
        public override string CommandName => "addCiEntryPointSetting";
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
