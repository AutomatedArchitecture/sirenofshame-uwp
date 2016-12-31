﻿using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class SendLatestCommand : CommandBase
    {
        public override string CommandName => "SendLatest";

        public override async Task<SocketResult> Invoke(string frame)
        {
            var rulesEngine = ServiceContainer.Resolve<RulesEngine>();
            await rulesEngine.SendLatest();
            return new OkSocketResult();
        }
    }
}
