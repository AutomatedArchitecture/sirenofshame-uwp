using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class DeleteBuildDefinitionCommand : CommandBase<Request<int>>
    {
        public override string CommandName => "delete-server";
        protected override async Task<SocketResult> Invoke(Request<int> frame)
        {
            // todo delete
            await Task.Yield();
            return new OkSocketResult();
        }
    }
}