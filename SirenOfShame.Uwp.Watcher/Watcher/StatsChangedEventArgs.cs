using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class StatsChangedEventArgs
    {
        public const string COMMAND_NAME = "StatsChanged";
        public IList<BuildStatus> ChangedBuildStatuses { get; set; }
        public IList<PersonSetting> ChangedPeople { get; set; }
    }
}