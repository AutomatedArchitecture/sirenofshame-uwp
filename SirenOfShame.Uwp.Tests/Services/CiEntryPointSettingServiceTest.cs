using System.Threading.Tasks;
using NUnit.Framework;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Tests.Services
{
    [TestFixture]
    public class CiEntryPointSettingServiceTest : TestBase
    {
        private string passwordEncrypted = "wwwfBrRRCDxe3qSYCrri3w==";

        [Test]
        public void GivenCiEntryPointSettingWithPassword_WhenGetByIdForUnencryptedCommunication_ThenPasswordIsEmpty()
        {
            Settings.CiEntryPointSettings.Add(new CiEntryPointSetting
            {
                Id = 5,
                EncryptedPassword = "encryptedPassword"
            });
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var foundCiEntryPointSetting = ciEntryPointSettingService.GetByIdForUnencryptedCommunication(5);
            Assert.IsNotNull(foundCiEntryPointSetting);
            Assert.AreEqual(5, foundCiEntryPointSetting.Id);
            Assert.IsNull(foundCiEntryPointSetting.EncryptedPassword);
        }

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
        public void GivenZeroCiEntryPointSettings_WhenAdd_ThenPasswordSavedEncrypted()
        {
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new InMemoryCiEntryPointSetting
            {
                Password = "password"
            };
            ciEntryPointSettingService.Add(ciEntryPointSetting);
            Assert.AreEqual(passwordEncrypted, Settings.CiEntryPointSettings[0].EncryptedPassword);
        }

        [Test]
        public void GivenZeroCiEntryPointSettings_WhenAdd_ThenSettingIdIs1()
        {
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new InMemoryCiEntryPointSetting();
            ciEntryPointSettingService.Add(ciEntryPointSetting);
            Assert.AreEqual(1, ciEntryPointSetting.Id);
        }

        [Test]
        public void GivenOneCiEntryPointSetting_WhenAdd_ThenSettingIdIs2()
        {
            Settings.CiEntryPointSettings.Add(new CiEntryPointSetting { Id = 1 });
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new InMemoryCiEntryPointSetting();
            ciEntryPointSettingService.Add(ciEntryPointSetting);
            Assert.AreEqual(2, ciEntryPointSetting.Id);
        }

        [Test]
        public void GivenCiEntryPointSetting_WhenUpdatePassword_ThenPaswordIsSavedEncrypted()
        {
            var existingCiEntryPointSetting = new CiEntryPointSetting { Id = 1 };
            Settings.CiEntryPointSettings.Add(existingCiEntryPointSetting);
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new InMemoryCiEntryPointSetting
            {
                Id = 1,
                Password = "password"
            };
            ciEntryPointSettingService.Update(ciEntryPointSetting);
            Assert.AreEqual(passwordEncrypted, existingCiEntryPointSetting.EncryptedPassword);
        }

        [Test]
        public void GivenOneCiEntryPointSetting_WhenUpdate_ThenUrlUpdates()
        {
            var existingCiEntryPointSetting = new CiEntryPointSetting { Id = 1, Url = "Old" };
            Settings.CiEntryPointSettings.Add(existingCiEntryPointSetting);
            var ciEntryPointSettingService = new CiEntryPointSettingService();

            var ciEntryPointSetting = new InMemoryCiEntryPointSetting
            {
                Id = 1,
                Url = "New"
            };
            ciEntryPointSettingService.Update(ciEntryPointSetting);
            Assert.AreEqual("New", existingCiEntryPointSetting.Url);
        }

        [Test]
        public async Task GivenMatchingCiEntryPointSetting_WhenDelete_ItIsDeleted()
        {
            var existingCiEntryPointSetting = new CiEntryPointSetting { Id = 2 };
            Settings.CiEntryPointSettings.Add(existingCiEntryPointSetting);
            var ciEntryPointSettingService = new CiEntryPointSettingService();
            await ciEntryPointSettingService.Delete(2);
            Assert.AreEqual(0, Settings.CiEntryPointSettings.Count);
        }

        [Test]
        public async Task GivenNonmatchingCiEntryPointSetting_WhenDelete_ItFailsQuietly()
        {
            var ciEntryPointSettingService = new CiEntryPointSettingService();
            await ciEntryPointSettingService.Delete(2);
            Assert.AreEqual(0, Settings.CiEntryPointSettings.Count);
        }
    }
}
