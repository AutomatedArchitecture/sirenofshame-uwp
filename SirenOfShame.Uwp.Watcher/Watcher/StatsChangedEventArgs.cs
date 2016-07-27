using System.Collections.Generic;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class StatsChangedEventArgs
    {
        public IList<BuildStatus> ChangedBuildStatuses { get; set; }
    }
}