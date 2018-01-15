using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Shared.Dtos;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    internal class GetLogsCommand : CommandBase
    {
        public override string CommandName => CommandNames.GET_LOGS;

        public override async Task<SocketResult> Invoke(string frame)
        {
            var readLogEntriesResult = await WatcherLogManager.ReadLogEntriesAsync(true);
            var result = readLogEntriesResult.Events
                .Select(LogToString)
                .ToList();

            return new OkSocketResult<IList<string>>(result);
        }

        private static string LogToString(ILogEntry log)
        {
            return log.DateTimeUtc.ToLocalTime() + " | " + log.Level + " | " + log.Message;
        }
    }
}