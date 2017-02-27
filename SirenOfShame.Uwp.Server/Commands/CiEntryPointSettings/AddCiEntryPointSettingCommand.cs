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
        private readonly RulesEngine _rulesEngine;

        public AddCiEntryPointSettingCommand()
        {
            _rulesEngine = ServiceContainer.Resolve<RulesEngine>();
        }

        public override string CommandName => "addCiEntryPointSetting";
        protected override async Task<SocketResult> Invoke(AddCiEntryPointSettingRequest request)
        {
            var ciEntryPointSettingService = ServiceContainer.Resolve<CiEntryPointSettingService>();

            foreach (var buildDefinitionSetting in request.CiEntryPointSetting.BuildDefinitionSettings)
            {
                buildDefinitionSetting.Active = true;
                buildDefinitionSetting.BuildServer = request.CiEntryPointSetting.Name;
            }

            var incommingId = request.CiEntryPointSetting.Id;
            if (incommingId == 0)
            {
                ciEntryPointSettingService.Add(request.CiEntryPointSetting);
            }
            else
            {
                ciEntryPointSettingService.Update(request.CiEntryPointSetting);
            }
            var settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
            await settingsIoService.Save();
            await _rulesEngine.RefreshAll();
            return new OkSocketResult
            {
                Result = request.CiEntryPointSetting.Id
            };
        }
    }
}
