using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.TestServer.Services
{
    class TestServerStartManager : ServerStartManager
    {
        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register<IWebServer>(() => new WebServer());
        }
    }
}
