using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class Legend : AchievementBase
    {
        private readonly int _reputation;

        public Legend(PersonSetting personSetting, int reputation)
            : base(personSetting)
        {
            _reputation = reputation;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.Legend; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _reputation >= 1000;
        }
    }
}
