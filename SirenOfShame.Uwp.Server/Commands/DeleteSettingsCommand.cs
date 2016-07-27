using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class DeleteSettingsCommand : CommandBase
    {
        public override string CommandName => "delete-settings";
        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            return new OkSocketResult();
        }
    }
}
