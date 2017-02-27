using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class DeleteCiEntryPointSettingCommand : CommandBase<Request<int>>
    {
        private readonly RulesEngine _rulesEngine;

        public DeleteCiEntryPointSettingCommand()
        {
            _rulesEngine = ServiceContainer.Resolve<RulesEngine>();
        }

        public override string CommandName => "delete-server";
        protected override async Task<SocketResult> Invoke(Request<int> frame)
        {
            var appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
            // todo delete
            await Task.Yield();
            return new OkSocketResult();
        }
    }
}