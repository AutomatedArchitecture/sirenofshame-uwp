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
        }
    }
}
