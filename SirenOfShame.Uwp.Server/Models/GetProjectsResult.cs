using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.HudsonServices;

namespace SirenOfShame.Uwp.Server.Models
{
    internal class GetProjectsResult : SocketResult
    {
        public GetProjectsResult(IEnumerable<HudsonBuildDefinition> projects)
        {
            Type = "getProjectsResult";
            ResponseCode = 200;
            Result = projects;
        }
    }
}