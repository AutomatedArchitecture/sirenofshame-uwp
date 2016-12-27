using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Models
{
    internal class GetProjectsResult : SocketResult
    {
        public GetProjectsResult(IEnumerable<MyBuildDefinition> projects)
        {
            Type = "getProjectsResult";
            ResponseCode = 200;
            Result = projects;
        }
    }
}