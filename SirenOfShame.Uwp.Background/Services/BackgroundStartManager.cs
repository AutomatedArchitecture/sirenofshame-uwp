using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Background.Services
{
    internal class BackgroundStartManager : ServerStartManager
    {
        protected override void RegisterServices()
        {
            base.RegisterServices();
            ServiceContainer.Register<IWebServer>(() => new WebServer());
        }
    }
}
