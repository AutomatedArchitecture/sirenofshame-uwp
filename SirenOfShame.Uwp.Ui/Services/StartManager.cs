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
        }
    }
}
