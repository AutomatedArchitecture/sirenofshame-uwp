using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class RefreshStatusEventArgs {
        public const string COMMAND_NAME = "RefreshStatus";
        public IList<BuildStatusDto> BuildStatusDtos { get; set; }

        public void RefreshDisplayNames(SirenOfShameSettings settings)
        {
            foreach (var buildStatusDto in BuildStatusDtos)
            {
                buildStatusDto.SetDisplayName(settings);
            }
        }
    }
}