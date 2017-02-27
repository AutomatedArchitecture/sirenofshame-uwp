﻿using NUnit.Framework;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Tests.Services
{
    [TestFixture]
    public class CiEntryPointSettingServiceTest : TestBase
    {
        [Test]
        public void GivenCiEntryPointSetting_WhenGetById_ThenSettingIsFound()
        {
            Settings.CiEntryPointSettings.Add(new CiEntryPointSetting { Id = 5 });
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var foundCiEntryPointSetting = ciEntryPointSettingService.GetById(5);
            Assert.IsNotNull(foundCiEntryPointSetting);
            Assert.AreEqual(5, foundCiEntryPointSetting.Id);
        }

        [Test]
        public void GivenZeroCiEntryPointSettings_WhenAdd_ThenSettingIdIs1()
        {
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new CiEntryPointSetting();
            ciEntryPointSettingService.Add(ciEntryPointSetting);
            Assert.AreEqual(1, ciEntryPointSetting.Id);
        }

        [Test]
        public void GivenOneCiEntryPointSetting_WhenAdd_ThenSettingIdIs2()
        {
            Settings.CiEntryPointSettings.Add(new CiEntryPointSetting { Id = 1 });
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new CiEntryPointSetting();
            ciEntryPointSettingService.Add(ciEntryPointSetting);
            Assert.AreEqual(2, ciEntryPointSetting.Id);
        }
    }
}
