using System.Linq;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Services
{
    public class CiEntryPointSettingService
    {
        private readonly SirenOfShameSettings _appSettings;

        public CiEntryPointSettingService()
        {
            _appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
        }

        public CiEntryPointSetting GetById(int id)
        {
            var existingRecord = _appSettings.CiEntryPointSettings.First(i => i.Id == id);
            return existingRecord;
        }

        private int GetNextId()
        {
            var maxId = _appSettings.CiEntryPointSettings.Max(i => (int?)i.Id) ?? 0;
            var newId = maxId + 1;
            return newId;
        }

        public void Add(CiEntryPointSetting ciEntryPointSetting)
        {
            var newId = GetNextId();
            ciEntryPointSetting.Id = newId;
            _appSettings.CiEntryPointSettings.Add(ciEntryPointSetting);
        }
    }
}
