using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Commands.Settings
{
    internal class DeleteSettingsCommand : CommandBase
    {
        private readonly SettingsIoService _settingsService;
        private readonly RulesEngine _rulesEngine;

        public DeleteSettingsCommand()
        {
            _settingsService = ServiceContainer.Resolve<SettingsIoService>();
            _rulesEngine = ServiceContainer.Resolve<RulesEngine>();
        }

        public override string CommandName => "delete-settings";
        public override async Task<SocketResult> Invoke(string frame)
        {
            await _settingsService.DeleteSettings();
            await _rulesEngine.RefreshAll();
            return new OkSocketResult();
        }
    }
}
