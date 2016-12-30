using System;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Settings
{
    public class AchievementLookup
    {
        public AchievementEnum Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public NewsItemEvent AsNewNewsItem(PersonSetting person)
        {
            return new NewsItemEvent
            {
                Person = person,
                EventDate = DateTime.Now,
                NewsItemType = NewsItemTypeEnum.NewAchievement,
                BuildDefinitionId = null,
                ReputationChange = null,
                Title = "Achieved " + Name,
            };
        }
    }
}