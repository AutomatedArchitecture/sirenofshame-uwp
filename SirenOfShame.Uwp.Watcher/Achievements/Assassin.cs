using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class Assassin : AchievementBase
    {
        private readonly int _howManyTimesHasFixedSomeoneElsesBuild;

        public Assassin(PersonSetting personSetting)
            : base(personSetting)
        {
            _howManyTimesHasFixedSomeoneElsesBuild = personSetting.NumberOfTimesFixedSomeoneElsesBuild;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.Assassin; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _howManyTimesHasFixedSomeoneElsesBuild >= 10;
        }
    }
}
