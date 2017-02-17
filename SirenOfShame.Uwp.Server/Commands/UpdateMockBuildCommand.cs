using System;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Watcher;
using SirenOfShame.Uwp.Watcher.Watchers.MockCiServerServices;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class UpdateMockBuildCommand : CommandBase
    {
        public override string CommandName => "updateMockBuild";

        public override async Task<SocketResult> Invoke(string frame)
        {
            await Task.Yield();
            var request = Deserialize<Request<BuildStatus>>(frame);
            // normally setting LocalStartTime happens in the ctor, but b/c of json deserialization we need to re-set it
            request.Message.LocalStartTime = DateTime.Now;
            MockWatcher.UpdateBuild(request.Message);
            return new OkSocketResult();
        }
    }
}