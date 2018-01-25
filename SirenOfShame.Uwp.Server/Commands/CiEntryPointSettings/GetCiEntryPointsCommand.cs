using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    class GetCiEntryPointsCommand : CommandBase
    {
        public override string CommandName { get; } = CommandNames.GET_CI_ENTRY_POINTS;
        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var ciEntryPoints = SirenOfShameSettings.CiEntryPoints;
            return new OkSocketResult<IEnumerable<ICiEntryPoint>>(ciEntryPoints);
        }
    }
}
