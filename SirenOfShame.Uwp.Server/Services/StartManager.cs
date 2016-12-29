using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Services
{
    public class StartManager : StartManagerBase
    {
        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register(() => new MessageRelayService());
            ServiceContainer.Register(() => new SirenDeviceService());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
        }
    }
}
