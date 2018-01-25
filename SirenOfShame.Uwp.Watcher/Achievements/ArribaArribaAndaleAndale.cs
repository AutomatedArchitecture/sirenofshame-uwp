using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class ArribaArribaAndaleAndale : AchievementBase
    {
        private readonly int _howManyTimesHasPerformedBackToBackBuilds;

        public ArribaArribaAndaleAndale(PersonSetting personSetting)
            : base(personSetting)
        {
            _howManyTimesHasPerformedBackToBackBuilds = personSetting.NumberOfTimesPerformedBackToBackBuilds;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.ArribaArribaAndaleAndale; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _howManyTimesHasPerformedBackToBackBuilds >= 5;
        }
    }
}
