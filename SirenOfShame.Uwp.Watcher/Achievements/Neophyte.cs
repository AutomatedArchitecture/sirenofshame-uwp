﻿using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class Neophyte : AchievementBase
    {
        private readonly int _reputation;

        public Neophyte(PersonSetting personSetting, int reputation)
            : base(personSetting)
        {
            _reputation = reputation;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.Neophyte; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            return _reputation >= 100;
        }
    }
}