using System.Collections.Generic;

namespace SirenOfShame.Uwp.Server.Models
{
    internal class GetProjectsResult : SocketResult
    {
        public GetProjectsResult(IEnumerable<Project> projects)
        {
            Type = "getProjectsResult";
            ResponseCode = 200;
            Result = projects;
        }
    }

    internal class Project
    {
    }
}