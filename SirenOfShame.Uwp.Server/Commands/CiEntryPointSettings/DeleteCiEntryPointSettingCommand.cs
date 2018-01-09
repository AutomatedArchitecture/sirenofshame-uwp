using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class DeleteCiEntryPointSettingCommand : CommandBase<Request<int>>
    {
        private readonly CiEntryPointSettingService _ciEntryPointSettingService;

        public DeleteCiEntryPointSettingCommand()
        {
            _ciEntryPointSettingService = ServiceContainer.Resolve<CiEntryPointSettingService>();
        }

        public override string CommandName => CommandNames.DELETE_CI_ENTRY_POINT_SETTING;
        protected override async Task<SocketResult> Invoke(Request<int> frame)
        {
            await _ciEntryPointSettingService.Delete(frame.Message);
            return new OkSocketResult();
        }
    }
}