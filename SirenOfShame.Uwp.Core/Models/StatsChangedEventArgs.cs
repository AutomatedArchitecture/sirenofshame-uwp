using System.Collections.Generic;
using SirenOfShame.Uwp.Core.Services;

namespace SirenOfShame.Uwp.Core.Models
{
    public class StatsChangedEventArgs
    {
        public const string COMMAND_NAME = "StatsChanged";
        public IList<BuildStatusBase> ChangedBuildStatuses { get; set; }
        public IList<PersonSettingBase> ChangedPeople { get; set; }
    }
}