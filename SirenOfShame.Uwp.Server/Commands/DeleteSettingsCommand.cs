using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class DeleteSettingsCommand : CommandBase
    {
        private readonly SirenOfShameSettingsService _sirenOfShameSettingsService;

        public DeleteSettingsCommand()
        {
            _sirenOfShameSettingsService = SirenOfShameSettingsService.Instance;
        }

        public override string CommandName => "delete-settings";
        public override async Task<SocketResult> Invoke(string frame)
        {
            await _sirenOfShameSettingsService.DeleteSettings();
            return new OkSocketResult();
        }
    }
}
