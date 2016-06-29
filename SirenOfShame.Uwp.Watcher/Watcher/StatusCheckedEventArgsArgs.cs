using System.Collections.Generic;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class StatusCheckedEventArgsArgs
    {
        public IList<BuildStatus> BuildStatuses { get; set; }
    }
}