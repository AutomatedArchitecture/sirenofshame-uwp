using System.Collections.Generic;
using System.Linq;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Ui.ViewModels
{
    public class ViewUserViewModel
    {
        private readonly PersonDto _person;

        public ViewUserViewModel(PersonDto person)
        {
            _person = person;
            Achievements = AchievementSetting.AchievementLookups
                .Select(i => new AchievementViewModel(i, _person.AchievementIdsList))
                .ToList();
        }

        public List<AchievementViewModel> Achievements { get; set; }

        public string DisplayName => _person.DisplayName;

        public int? AvatarId => _person.AvatarId;
    }

    public class AchievementViewModel
    {
        private readonly AchievementLookup _achievementLookup;

        public AchievementViewModel(AchievementLookup achievementLookup, List<int> personAchievementIdsList)
        {
            _achievementLookup = achievementLookup;
            IsAchieved = personAchievementIdsList.Contains((int)achievementLookup.Id);
        }

        public string Name => _achievementLookup.Name;
        public bool IsAchieved { get; private set; }
    }
}
