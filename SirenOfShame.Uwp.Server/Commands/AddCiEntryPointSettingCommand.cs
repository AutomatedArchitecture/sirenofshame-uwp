using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class AddCiEntryPointSettingCommand : CommandBase
    {
        public override string CommandName => "addCiEntryPointSetting";
        public override async Task<SocketResult> Invoke(string frame)
        {
            var sosService = SirenOfShameSettingsService.Instance;
            var ciEntryPointSetting = JsonConvert.DeserializeObject<CiEntryPointSetting>(frame);
            var appSettings = await sosService.GetAppSettings();
            appSettings.CiEntryPointSettings.Add(ciEntryPointSetting);
            await sosService.Save(ciEntryPointSetting);
            return new OkSocketResult();
        }
    }
}
