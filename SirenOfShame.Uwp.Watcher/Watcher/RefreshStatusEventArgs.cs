using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class RefreshStatusEventArgs {
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