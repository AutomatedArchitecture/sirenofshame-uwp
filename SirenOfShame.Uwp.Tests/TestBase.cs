using NUnit.Framework;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Test.Unit.Watcher;
using SirenOfShame.Uwp.Tests.Services;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Tests
{
    public abstract class TestBase
    {
        protected SirenOfShameSettingsFake Settings { get; set; }

        [SetUp]
        public void Setup()
        {
            Settings = new SirenOfShameSettingsFake();
            ServiceContainer.Register<IFileAdapter>(() => new FakeFileAdapter());
            ServiceContainer.Register<SirenOfShameSettings>(() => Settings);
            ServiceContainer.Register<RulesEngine>(() => new FakeRulesEngine());
            ServiceContainer.Register<SettingsIoService>(() => new FakeSettingsIoService());
            ServiceContainer.Register<SosDb>(() => new FakeSosDb());
        }
    }
}
