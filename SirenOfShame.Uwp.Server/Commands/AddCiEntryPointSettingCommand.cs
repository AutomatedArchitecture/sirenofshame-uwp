using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
        private SirenOfShameSettingsService _sosService;

        public AddCiEntryPointSettingCommand()
        {
            _sosService = SirenOfShameSettingsService.Instance;
        }

        public override string CommandName => "addCiEntryPointSetting";
        public override async Task<SocketResult> Invoke(string frame)
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var request = JsonConvert.DeserializeObject<AddCiEntryPointSettingRequest>(frame, jsonSerializerSettings);

            var appSettings = await _sosService.GetAppSettings();

            request.CiEntryPointSetting.Id = appSettings.CiEntryPointSettings.Max(i => (int?)i.Id) ?? 0 + 1;

            appSettings.CiEntryPointSettings.Add(request.CiEntryPointSetting);
            await _sosService.Save(appSettings);
            return new OkSocketResult();
        }
    }
}
