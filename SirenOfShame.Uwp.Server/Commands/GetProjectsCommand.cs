using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.HudsonServices;

namespace SirenOfShame.Uwp.Server.Commands
{

    public class GetProjectsRequest
    {
        public string Type { get; set; }
        public CiServer CiServer { get; set; }
    }

    public class CiServer
    {
        public string Url { get; set; }
        public string ServerType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    internal class GetProjectsCommand : CommandBase
    {
        public override string CommandName => "getProjects";
        private TaskCompletionSource<SocketResult> _taskCompletionSource;

        public override async Task<SocketResult> Invoke(string frame)
        {
            if (_taskCompletionSource != null) await _taskCompletionSource.Task;
            _taskCompletionSource = new TaskCompletionSource<SocketResult>();

            var getProjectsRequest = JsonConvert.DeserializeObject<GetProjectsRequest>(frame);

            var service = new HudsonService();
            var ciServer = getProjectsRequest.CiServer;
            service.GetProjects(ciServer.Url, ciServer.UserName, ciServer.Password, GetProjectsComplete, GetProjectsError);
            var socketResult = await _taskCompletionSource.Task;
            return socketResult;
        }

        private void GetProjectsError(Exception obj)
        {
            _taskCompletionSource.TrySetException(obj);
            _taskCompletionSource = null;
        }

        private void GetProjectsComplete(HudsonBuildDefinition[] builddefinitions)
        {
            var getProjectsResult = new GetProjectsResult(builddefinitions);
            _taskCompletionSource.TrySetResult(getProjectsResult);
            _taskCompletionSource = null;
        }
    }
}
