using NUnit.Framework;
using SirenOfShame.Test.Unit.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Tests
{
    public abstract class TestBase
    {
        protected SirenOfShameSettingsFake Settings { get; set; }

        [SetUp]
        public void Setup()
        {
            Settings = new SirenOfShameSettingsFake();
            ServiceContainer.Register<SirenOfShameSettings>(() => Settings);
        }
    }
}
