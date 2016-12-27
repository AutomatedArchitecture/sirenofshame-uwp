using System.Collections.Generic;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Watchers.MockCiServerServices
{
    public class MockWatcher : WatcherBase
    {
        private readonly List<BuildStatus> _buildStatuses = new List<BuildStatus>();

        public MockWatcher(SirenOfShameSettings settings) : base(settings)
        {
        }

        protected override IList<BuildStatus> GetBuildStatus()
        {
            return _buildStatuses;
        }

        public override void StopWatching()
        {
            
        }

        public override void Dispose()
        {
            
        }
    }
}
