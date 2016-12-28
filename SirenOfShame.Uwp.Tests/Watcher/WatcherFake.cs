using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Exceptions;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Test.Unit.Watcher
{
    public class WatcherFake : WatcherBase
    {
        public WatcherFake(SirenOfShameSettings settings)
            : base(settings)
        {
        }

        protected override IList<BuildStatus> GetBuildStatus()
        {
            return new List<BuildStatus>();
        }

        public override void Dispose()
        {
            // do nothing
        }

        public override async Task StartWatching(CancellationToken token)
        {
            await Task.Yield();
            // do nothing
        }

        public override void StopWatching()
        {
            // do nothing
        }

        public new void InvokeServerUnavailable(ServerUnavailableException ex)
        {
            base.InvokeServerUnavailable(ex);
        }

        public new void InvokeStatusChecked(IList<BuildStatus> args) {
            base.InvokeStatusChecked(args);
        }

        public void InvokeStoppedWatching() {
            OnStoppedWatching();
        }
    }
}
