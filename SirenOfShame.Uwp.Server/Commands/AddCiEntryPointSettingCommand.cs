using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{
    public class AddCiEntryPointSettingRequest : RequestBase
    {
        public string Type { get; set; }
        public CiEntryPointSetting CiEntryPointSetting { get; set; }
    }

    internal class AddCiEntryPointSettingCommand : CommandBase
    {
        private readonly SirenOfShameSettingsService _sosService;

        public AddCiEntryPointSettingCommand()
        {
            _sosService = SirenOfShameSettingsService.Instance;
        }

        public override string CommandName => "addCiEntryPointSetting";
        public override async Task<SocketResult> Invoke(string frame)
        {
            var request = Deserialize<AddCiEntryPointSettingRequest>(frame);

            var appSettings = await _sosService.GetAppSettings();

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
            await _sosService.Save(appSettings);
            return new OkSocketResult
            {
                Result = request.CiEntryPointSetting.Id
            };
        }
    }
}
