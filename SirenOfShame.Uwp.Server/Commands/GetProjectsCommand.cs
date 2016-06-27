using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class GetProjectsCommand : CommandBase
    {
        public override string CommandName => "getProjects";

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var projects = new []
            {
                new Project(),
                new Project()
            };
            return new GetProjectsResult(projects);
        }
    }
}
