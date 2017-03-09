using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Commands.CiEntryPointSettings
{
    class GetLogs : CommandBase
    {
        public override string CommandName => "get-logs";

        public override async Task<SocketResult> Invoke(string frame)
        {
            var rulesEngine = ServiceContainer.Resolve<RulesEngine>();

            var metroLogs = await ApplicationData.Current.LocalFolder.GetFolderAsync("MetroLogs");
            var allLogFiles = await metroLogs.GetFilesAsync();
            var lastLogFile = allLogFiles
                .OrderBy(i => i.Name)
                .LastOrDefault();

            List<string> result = new List<string>();

            await rulesEngine.Pause(async () =>
            {
                MetroLogger.CloseAllOpenFiles();
                var allLines = await FileIO.ReadLinesAsync(lastLogFile);
                result = allLines.ToList();
            });

            return new OkSocketResult<IList<string>>(result);
        }
    }
}