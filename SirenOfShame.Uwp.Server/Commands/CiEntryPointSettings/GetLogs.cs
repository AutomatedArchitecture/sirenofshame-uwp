using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroLog.Targets;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    class GetLogs : CommandBase
    {
        public override string CommandName => "get-logs";

        public override async Task<SocketResult> Invoke(string frame)
        {
            var readLogEntriesResult = await MetroLogger.ReadLogEntriesAsync(true);
            var result = readLogEntriesResult.Events
                .Select(LogToString)
                .ToList();

            return new OkSocketResult<IList<string>>(result);
        }

        private static string LogToString(LogEventInfoItem log)
        {
            return log.DateTimeUtc.ToLocalTime() + " | " + log.Level + " | " + log.Message;
        }
    }
}