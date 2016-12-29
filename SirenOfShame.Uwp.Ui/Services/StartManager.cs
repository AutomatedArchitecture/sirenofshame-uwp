using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Services
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
            ServiceContainer.Register(() => new MessageDistributorService());
        }
    }
}
