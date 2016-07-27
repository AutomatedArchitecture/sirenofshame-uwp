using System;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Dto
{
    public class OfflineUserAchievementDto
    {
        public OfflineUserAchievementDto(AchievementSetting achievementSetting)
        {
            AchievementId = achievementSetting.AchievementId;
            DateAchieved = achievementSetting.DateAchieved;
        }

        public int AchievementId { get; set; }
        
        public DateTime DateAchieved { get; set; }
    }
}