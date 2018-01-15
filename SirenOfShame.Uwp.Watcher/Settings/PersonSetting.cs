using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Watcher.Achievements;
using SirenOfShame.Uwp.Watcher.StatCalculators;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Settings
{
    //[Serializable]
    public class PersonSetting : PersonSettingBase
    {
        private readonly SosDb _sosDb = new SosDb();

        // this either needs to stay private or find the attribute to not persist
        private TimeSpan? MyCumulativeBuildTime
        {
            get { return CumulativeBuildTime == null ? (TimeSpan?)null : new TimeSpan(CumulativeBuildTime.Value); }
            set { CumulativeBuildTime = value == null ? (long?)null : value.Value.Ticks; }
        }

        public TimeSpan? GetCumulativeBuildTime()
        {
            return MyCumulativeBuildTime;
        }

        public PersonSetting()
        {
            Achievements = new List<AchievementSetting>();
        }

        //public override int GetAvatarId(ImageList avatarImageList)
        //{
        //    if (AvatarId.HasValue) return AvatarId.Value;
        //    var gravatarService = new GravatarService();
        //    if (!string.IsNullOrEmpty(AvatarImageName))
        //        return gravatarService.LoadAvatarFromFile(AvatarImageName, avatarImageList);
        //    return gravatarService.DownloadGravatarFromEmailAndAddToImageList(Email, avatarImageList);
        //}

        public async Task<IEnumerable<AchievementLookup>> CalculateNewAchievements(SirenOfShameSettings settings, BuildStatus build)
        {
            var allBuildDefinitions = await _sosDb
                .ReadAll(settings.GetAllActiveBuildDefinitions());
            List<BuildStatus> allActiveBuildDefinitionsOrderedChronoligically = allBuildDefinitions
                .OrderBy(i => i.StartedTime)
                .ToList();

            return CalculateNewAchievements(settings, build, allActiveBuildDefinitionsOrderedChronoligically);
        }

        public IEnumerable<AchievementLookup> CalculateNewAchievements(SirenOfShameSettings settings, BuildStatus build, List<BuildStatus> allActiveBuildDefinitionsOrderedChronoligically)
        {
            return from achievementEnum in CalculateNewAchievementEnums(settings, build, allActiveBuildDefinitionsOrderedChronoligically)
                   join achievement in AchievementSetting.AchievementLookups on achievementEnum equals achievement.Id
                   select achievement;
        }

        private IEnumerable<AchievementEnum> CalculateNewAchievementEnums(SirenOfShameSettings settings, BuildStatus build, List<BuildStatus> allActiveBuildDefinitionsOrderedChronoligically)
        {
            int reputation = GetReputation();

            List<BuildStatus> currentBuildDefinitionOrderedChronoligically = allActiveBuildDefinitionsOrderedChronoligically
                .Where(i => i.BuildDefinitionId == build.BuildDefinitionId)
                .ToList();

            if (build.FinishedTime != null && build.StartedTime != null)
            {
                TimeSpan? buildDuration = build.FinishedTime.Value - build.StartedTime.Value;
                MyCumulativeBuildTime = MyCumulativeBuildTime == null ? buildDuration : MyCumulativeBuildTime + buildDuration;
            }

            CalculateStats(allActiveBuildDefinitionsOrderedChronoligically);

            List<AchievementBase> possibleAchievements = new List<AchievementBase>
            {
                new Apprentice(this, reputation),
                new Neophyte(this, reputation),
                new Master(this, reputation),
                new GrandMaster(this, reputation),
                new Legend(this, reputation),
                new JonSkeet(this, reputation),
                new TimeWarrior(this),
                new ChronMaster(this),
                new ChronGrandMaster(this),
                new CiNinja(this),
                new Assassin(this),
                new LikeLightning(this, currentBuildDefinitionOrderedChronoligically),
                new ReputationRebound(this, allActiveBuildDefinitionsOrderedChronoligically),
                new ArribaArribaAndaleAndale(this),
                new SpeedDaemon(this),
                new InTheZone(this),
                new Terminator(this),
                new AndGotAwayWithIt(this, currentBuildDefinitionOrderedChronoligically),
                new Critical(this),
                new Perfectionist(this),
                new Macgyver(this, currentBuildDefinitionOrderedChronoligically),
                new Napoleon(this, settings.People),
                new ShamePusher(this, settings)
            };

            return possibleAchievements
                .Where(i => i.HasJustAchieved())
                .Select(i => i.AchievementEnum);
        }

        public void CalculateStats(List<BuildStatus> allActiveBuildDefinitionsOrderedChronoligically) {
            List<StatCalculatorBase> statCalculators = new List<StatCalculatorBase>
            {
                new FixedSomeoneElsesBuild(),
                new BackToBackBuilds(),
                new MaxBuildsInOneDay(),
                new BuildRatio(),
                new SuccessInARow(),
            };

            foreach (var statCalculator in statCalculators)
            {
                statCalculator.SetStats(this, allActiveBuildDefinitionsOrderedChronoligically);
            }
        }

        public bool HasAchieved(AchievementEnum achievement)
        {
            return Achievements.Any(i => i.AchievementId == (int)achievement);
        }

        public void AddAchievements(IEnumerable<AchievementLookup> newAchievements)
        {
            foreach (var achievementLookup in newAchievements)
            {
                Achievements.Add(new AchievementSetting { AchievementId = (int)achievementLookup.Id, DateAchieved = DateTime.Now });
            }
        }

        public override string ToString()
        {
            return RawName;
        }

        public string GetBothDisplayAndRawNames()
        {
            return HasDisplayName() ? string.Format("{0} ({1})", DisplayName, RawName) : RawName;
        }

        private bool HasDisplayName()
        {
            return !string.IsNullOrWhiteSpace(DisplayName) && DisplayName != RawName;
        }
    }
}