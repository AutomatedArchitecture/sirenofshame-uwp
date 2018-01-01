using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using SirenOfShame.Uwp.Maintenance.Log;

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
                await Task.Delay(new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: 20));

                _log.Info("Checking for software updates");
                // todo: check for software updates
            }
        }
    }
}
