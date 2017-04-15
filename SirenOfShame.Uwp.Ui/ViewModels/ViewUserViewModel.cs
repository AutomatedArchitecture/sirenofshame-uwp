using System.Collections.Generic;
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
            Achievements = AchievementSetting.AchievementLookups;
        }

        public List<AchievementLookup> Achievements { get; set; }

        public string DisplayName => _person.DisplayName;

        public int? AvatarId => _person.AvatarId;
    }
}
