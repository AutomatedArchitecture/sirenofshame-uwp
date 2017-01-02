using Windows.ApplicationModel.Background;
using SirenOfShame.Uwp.Background.Services;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Background
{
    // ReSharper disable once UnusedMember.Global
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private readonly ServerStartManager _startManager = new BackgroundStartManager();

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += TaskInstanceOnCanceled;
            await _startManager.Start();
        }

        private void TaskInstanceOnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _startManager.Stop();
            _backgroundTaskDeferral?.Complete();
            _backgroundTaskDeferral = null;
        }
    }
}
