using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class Master : AchievementBase
    {
        private readonly int _reputation;

        public Master(PersonSetting personSetting, int reputation) : base(personSetting)
        {
            _reputation = reputation;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.Master; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _reputation >= 250;
        }
    }
}
