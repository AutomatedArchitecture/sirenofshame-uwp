using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    class GetLogs : CommandBase
    {
        public override string CommandName => "get-logs";

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var result = new[]
            {
                "Line 1",
                "Line 2",
            };
            return new OkSocketResult<string[]>(result);
        }
    }
}