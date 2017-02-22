using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Commands
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
            var appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();

            foreach (var buildDefinitionSetting in request.CiEntryPointSetting.BuildDefinitionSettings)
            {
                buildDefinitionSetting.Active = true;
                buildDefinitionSetting.BuildServer = request.CiEntryPointSetting.Name;
            }

            var incommingId = request.CiEntryPointSetting.Id;
            if (incommingId == 0)
            {
                var maxId = appSettings.CiEntryPointSettings.Max(i => (int?)i.Id) ?? 0;
                var newId = maxId + 1;
                request.CiEntryPointSetting.Id = newId;
                appSettings.CiEntryPointSettings.Add(request.CiEntryPointSetting);
            }
            else
            {
                var existingRecord = appSettings.CiEntryPointSettings.First(i => i.Id == incommingId);
                existingRecord.Url = request.CiEntryPointSetting.Url;
                existingRecord.BuildDefinitionSettings = request.CiEntryPointSetting.BuildDefinitionSettings;
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
