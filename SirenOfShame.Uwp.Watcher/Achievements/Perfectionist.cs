using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class Perfectionist : AchievementBase
    {
        private readonly double? _lowestBuildRatio;

        public Perfectionist(PersonSetting personSetting) : base(personSetting)
        {
            _lowestBuildRatio = personSetting.LowestBuildRatioAfter50Builds;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.Perfectionist; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _lowestBuildRatio < .05;
        }
    }
}
