﻿using System.Collections.Generic;
using System.Linq;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Achievements
{
    public class LikeLightning : AchievementBase
    {
        private readonly List<BuildStatus> _currentBuildDefinitionOrderedChronoligically;

        public LikeLightning(PersonSetting personSetting, List<BuildStatus> currentBuildDefinitionOrderedChronoligically) : base(personSetting)
        {
            _currentBuildDefinitionOrderedChronoligically = currentBuildDefinitionOrderedChronoligically;
        }

        public override AchievementEnum AchievementEnum
        {
            get { return AchievementEnum.LikeLightning; }
        }

        protected override bool MeetsAchievementCriteria()
        {
            if (_currentBuildDefinitionOrderedChronoligically.Count < 3) return false;
            var lastThree = _currentBuildDefinitionOrderedChronoligically.Skip(_currentBuildDefinitionOrderedChronoligically.Count - 3).ToList();
            if (!lastThree.All(i => i.RequestedBy == PersonSetting.RawName)) return false;
            if (!lastThree.All(i => i.StartedTime.HasValue && i.FinishedTime.HasValue)) return false;
            if (!lastThree.All(i => i.BuildStatusEnum == BuildStatusEnum.Working)) return false;
            BuildStatus oldestBuild = lastThree[0];
            BuildStatus middleBuild = lastThree[1];
            BuildStatus mostRecentBuild = lastThree[2];
            var wereLastBuildsBackToBack = oldestBuild.IsBackToBackWithNextBuild(middleBuild);
            var wereMostRecentBuidlsBackToBack = middleBuild.IsBackToBackWithNextBuild(mostRecentBuild);
            return wereLastBuildsBackToBack && wereMostRecentBuidlsBackToBack;
        }
    }
}
