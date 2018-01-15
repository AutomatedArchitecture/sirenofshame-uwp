using System.Collections.Generic;

namespace SirenOfShame.Uwp.Core.Models
{
    public class PersonSettingBase : PersonBase
    {
        public override string RawName { get; set; }
        public override string DisplayName { get; set; }
        public int TotalBuilds { get; set; }
        public int FailedBuilds { get; set; }
        public bool Hidden { get; set; }
        public List<AchievementSetting> Achievements { get; set; }
        public long? CumulativeBuildTime { get; set; }
        public int? AvatarId { get; set; }
        public int NumberOfTimesFixedSomeoneElsesBuild { get; set; }
        public int NumberOfTimesPerformedBackToBackBuilds { get; set; }
        public int MaxBuildsInOneDay { get; set; }
        public double CurrentBuildRatio { get; set; }
        public double? LowestBuildRatioAfter50Builds { get; set; }
        public int CurrentSuccessInARow { get; set; }
        public string Email { get; set; }
        public string AvatarImageName { get; set; }
        public bool AvatarImageUploaded { get; set; }

        public static int GetReputation(int totalBuilds, int failedBuilds)
        {
            return totalBuilds - (failedBuilds * 5);
        }
        
        public int GetReputation()
        {
            return GetReputation(TotalBuilds, FailedBuilds);
        }

    }
}
