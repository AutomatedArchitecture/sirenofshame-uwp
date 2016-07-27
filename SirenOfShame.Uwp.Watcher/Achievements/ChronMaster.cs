using System;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class ChronMaster : AchievementBase
    {
        public ChronMaster(PersonSetting personSetting) : base(personSetting)
        {
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.ChronMaster; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            TimeSpan? myCumulativeBuildTime = PersonSetting.GetCumulativeBuildTime();
            return myCumulativeBuildTime != null && myCumulativeBuildTime.Value.TotalHours >= 48;
        }
    }
}
