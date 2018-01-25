using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Maintenance.Log;
using SirenOfShame.Uwp.Maintenance.Services;

namespace SirenOfShame.Uwp.Maintenance
{
    public sealed class StartupTask : IBackgroundTask
    {
        private const string CERTIFICATE_PUBLIC_KEY = "MIIBCgKCAQEAwmHdD6jOo0UVxFSuo4g8m5p8EFP/fNPklEIeZRHu2MduByA7FZCsMVjo5P0opRg5dYWrmtcpXDkGIp0WJqxcDok6Uh3M1JMF6txSgDeVI9LTk2uWncVKZFFPfRecTGv9KGztjM9QhOyB6wcWvvUQllEenjBP5tXpGAcTX32g250RSPyLOJoPq96qWwi041ahE+GbF5q8izSf6OEoRicowBr9Hu8hgIPIRi8AwmaLsshKMZ4l4S+XW1dM1qTxrOGl4pyioOjiUkZGCvcJnupl5moHgmCuIwzBHE7dEFV35zr5tyZ9rUNQ0HrFNU48nU0cFLN23AgIc5oNFIUTEQ/Z9QIDAQAB";
        private const string CERTIFICATE_COMMON_NAME = "sirenofshame.com";
        private const string CERTIFICATE_PINNING_BASE_URL = "https://sirenofshame.com";

        private BackgroundTaskDeferral _backgroundTaskDeferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            ILog log;
            try
            {
                // continue forever, this is needed before any await calls
                _backgroundTaskDeferral = taskInstance.GetDeferral();

                var messageRelayService = new MaintenanceMessageRelayService();
                var maintenanceCommandProcessor = new MaintenanceCommandProcessor(messageRelayService);
                maintenanceCommandProcessor.StartWatching();
                await messageRelayService.Open();
                log = new MessageRelayLogger(messageRelayService);
                maintenanceCommandProcessor.TryUpgrade += async (sender, args) => await TryUpgrade(log);
                MyLogManager.GetLog = type => log;
                await log.Info("Starting Maintenance");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Log initializeation failed :( " + ex);
                return;
            }

            // wait a short amount of time from initial install 
            await Task.Delay(new TimeSpan(days: 0, hours: 0, minutes: 1, seconds: 0));

            while (true)
            {
                await TryUpgrade(log);
                await Task.Delay(new TimeSpan(days: 0, hours: 6, minutes: 0, seconds: 0));
            }
        }

        private static async Task TryUpgrade(ILog log)
        {
            try
            {
                await log.Debug("Checking for software updates");

                var httpClientFactory = new CertificatePinningHttpClientFactory(CERTIFICATE_PINNING_BASE_URL,
                    CERTIFICATE_COMMON_NAME, CERTIFICATE_PUBLIC_KEY);
                var bundleService = new BundleService(log, httpClientFactory);
                var updateManifestService = new UpdateManifestService();
                var manifest = await updateManifestService.GetManifest();
                await bundleService.TryUpdate(manifest, UpdateManifestService.SOS_BACKGROUND);
                await bundleService.TryUpdate(manifest, UpdateManifestService.SOS_UI);
                await bundleService.DeleteDownloads();
            }
            catch (Exception ex)
            {
                await log.Error("Unexpected error checking for updates", ex);
            }
        }
    }
}
