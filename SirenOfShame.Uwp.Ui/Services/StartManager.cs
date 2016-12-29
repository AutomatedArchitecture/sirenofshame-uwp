using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class StartManager : StartManagerBase
    {
        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register(() => new MessageRelayService());
            ServiceContainer.Register(() => new MessageDistributorService());
            ServiceContainer.Register<IFileAdapter>(() => new FileAdapter());
        }
    }
}
