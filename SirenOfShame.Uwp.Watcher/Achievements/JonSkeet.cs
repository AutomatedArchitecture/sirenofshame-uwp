using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class JonSkeet : AchievementBase
    {
        private readonly int _reputation;

        public JonSkeet(PersonSetting personSetting, int reputation)
            : base(personSetting)
        {
            _reputation = reputation;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.JonSkeet; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _reputation >= 2500;
        }
    }
}
