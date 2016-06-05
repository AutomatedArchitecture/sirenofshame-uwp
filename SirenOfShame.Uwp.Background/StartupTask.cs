using System;
using Windows.ApplicationModel.Background;

namespace SirenOfShame.Uwp.Background
{
    public sealed class StartupTask : IBackgroundTask
    {
        // ReSharper disable once NotAccessedField.Local
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private HttpServer _httpServer;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            _httpServer = new HttpServer(8001);
            await _httpServer.StartServerAction();
        }
    }
}
