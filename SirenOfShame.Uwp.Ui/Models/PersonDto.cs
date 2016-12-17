using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Ui.Models
{
    public class PersonDto
    {
        public PersonDto(PersonSetting person)
        {
            DisplayName = person.DisplayName;
            Reputation = person.GetReputation();
            Achievements = person.Achievements.Count;
            FailPercent = string.Format("{0:p1}", person.CurrentBuildRatio).Replace(" ", "");
            SuccessfulBuildsInARow = string.Format("{0}", person.CurrentSuccessInARow);
            FixedSomeoneElsesBuild = string.Format("{0}", person.NumberOfTimesFixedSomeoneElsesBuild);
            TotalBuilds = string.Format("{0}", person.TotalBuilds);
        }

        public string TotalBuilds { get; set; }

        public string FixedSomeoneElsesBuild { get; set; }

        public string SuccessfulBuildsInARow { get; set; }

        public string FailPercent { get; set; }

        public int Achievements { get; set; }

        public int Reputation { get; set; }

        public string DisplayName { get; set; }
    }
}
