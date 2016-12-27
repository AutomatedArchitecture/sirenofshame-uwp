using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Commands
{

    public class GetProjectsRequest : RequestBase
    {
        public CiServer CiServer { get; set; }
    }

    public class CiServer
    {
        public string Url { get; set; }
        public string ServerType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    internal class GetBuildDefinitionsCommand : CommandBase
    {
        public override string CommandName => "getBuildDefinitions";

        public override async Task<SocketResult> Invoke(string frame)
        {
            var getProjectsRequest = JsonConvert.DeserializeObject<GetProjectsRequest>(frame);

            var ciServer = getProjectsRequest.CiServer;
            var ciEntryPoint = SirenOfShameSettings.CiEntryPoints.First(i => i.Name == ciServer.ServerType);

            var getProjectsArgs = new GetProjectsArgs
            {
                Password = ciServer.Password,
                Url = ciServer.Url,
                UserName = ciServer.UserName,
            };
            MyBuildDefinition[] builddefinitions = await ciEntryPoint.GetProjects(getProjectsArgs);
            var getProjectsResult = new GetProjectsResult(builddefinitions);
            return getProjectsResult;
        }
    }
}
