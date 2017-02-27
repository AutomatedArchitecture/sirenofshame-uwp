using System;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Watcher;
using SirenOfShame.Uwp.Watcher.Watchers.MockCiServerServices;

namespace SirenOfShame.Uwp.Server.Commands.BuildDefinitions
{
    internal class UpdateMockBuildCommand : CommandBase<Request<BuildStatus>>
    {
        public override string CommandName => "updateMockBuild";

        protected override async Task<SocketResult> Invoke(Request<BuildStatus> request)
        {
            await Task.Yield();
            // normally setting LocalStartTime happens in the ctor, but b/c of json deserialization we need to re-set it
            request.Message.LocalStartTime = DateTime.Now;
            MockWatcher.UpdateBuild(request.Message);
            return new OkSocketResult();
        }
    }
}