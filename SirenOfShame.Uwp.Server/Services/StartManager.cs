using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Services
{
    public class StartManager
    {
        public void Configure()
        {
            RegisterServices();
        }

        private void RegisterServices()
        {
            ServiceContainer.Register(() => new MessageRelayService());
            ServiceContainer.Register(() => new SirenDeviceService());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
        }
    }
}
