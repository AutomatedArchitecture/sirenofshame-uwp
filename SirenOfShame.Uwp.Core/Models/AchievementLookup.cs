using System;

namespace SirenOfShame.Uwp.Core.Models
{
    public class AchievementLookup
    {
        public AchievementEnum Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public NewsItemEvent AsNewNewsItem(PersonSettingBase person)
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