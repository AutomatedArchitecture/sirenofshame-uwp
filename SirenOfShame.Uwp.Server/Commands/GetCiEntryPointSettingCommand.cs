using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class GetCiEntryPointSettingCommand : CommandBase
    {
        public override string CommandName => "getCiEntryPointSetting";
        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var ciEntryPointSetting = new CiEntryPointSetting {Id = 1, Name = "Jenkins", Url = "http://win7ci3:8081"};
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
