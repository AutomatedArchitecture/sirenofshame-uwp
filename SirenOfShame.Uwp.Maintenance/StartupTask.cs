using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using SirenOfShame.Uwp.Maintenance.Log;
using SirenOfShame.Uwp.Maintenance.Services;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SirenOfShame.Uwp.Maintenance
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private ILog _log = MyLogManager.GetLog(typeof(StartupTask));

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _log.Info("Starting Maintenance");

            // continue forever
            _backgroundTaskDeferral = taskInstance.GetDeferral();

            while (true)
            {
                await Task.Delay(new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0));

                _log.Info("Checking for software updates");

                var bundleService = new BundleService(_log);
                var manifest = await bundleService.GetManifest();
                await bundleService.TryUpdate(manifest, "c187913a-00d0-450c-ace6-c66581bdaf08");
            }
        }
    }
}
