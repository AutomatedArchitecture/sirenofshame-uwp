using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class NewAchievementEventArgs
    {
        public List<AchievementLookup> Achievements;
        public PersonSetting Person { get; set; }
    }
}