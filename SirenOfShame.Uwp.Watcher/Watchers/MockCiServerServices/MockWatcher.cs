using System;
using System.Collections.Generic;
using System.Linq;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Watchers.MockCiServerServices
{
    public class MockWatcher : WatcherBase
    {
        private static readonly ILog _log = MyLogManager.GetLog(typeof(MockWatcher));

        public MockWatcher(SirenOfShameSettings settings) : base(settings)
        {
        }

        protected override IList<BuildStatus> GetBuildStatus()
        {
            return Builds;
        }

        public static void UpdateBuild(BuildStatus buildStatus)
        {
            var build = Builds.FirstOrDefault(i => i.BuildDefinitionId == buildStatus.BuildDefinitionId);
            if (build == null)
            {
                _log.Error("Unable to find build " + buildStatus.BuildId);
                return;
            }
            var index = Builds.IndexOf(build);
            Builds[index] = buildStatus;
        }

        private static IList<BuildStatus> Builds { get; set; } = new List<BuildStatus>
            {
                GetBuildStatus(1),
                GetBuildStatus(2),
                GetBuildStatus(3)
            };

        private static BuildStatus GetBuildStatus(int id)
        {
            DateTime? startedTime = new DateTime(2016, 12, 12, 9, 9 ,9);
            return new BuildStatus
            {
                Name = "Mock " + id,
                BuildDefinitionId = "Mock" + id,
                BuildStatusEnum = BuildStatusEnum.Working,
                Comment = "Comment #" + id,
                FinishedTime = new DateTime(2016, 12, 12, 10, 10, 10),
                StartedTime = startedTime,
                RequestedBy = "Bob Smith",
                BuildId = startedTime.HasValue ? startedTime.Value.Ticks.ToString() : null,
                Url = "http://www.google.com"
            };
        }

        public override void StopWatching()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
