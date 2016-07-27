using System;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class TimeWarrior : AchievementBase
    {
        public TimeWarrior(PersonSetting personSetting) : base(personSetting)
        {
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.TimeWarrior; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            TimeSpan? myCumulativeBuildTime = PersonSetting.GetCumulativeBuildTime();
            return myCumulativeBuildTime != null && myCumulativeBuildTime.Value.TotalHours >= 24;
        }
    }
}
