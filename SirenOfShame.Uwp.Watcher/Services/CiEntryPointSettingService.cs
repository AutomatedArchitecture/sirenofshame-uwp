using System.Linq;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.Services
{
    public class CiEntryPointSettingService
    {
        private readonly SirenOfShameSettings _appSettings;
        private readonly RulesEngine _rulesEngine;
        private readonly ILog _log = MyLogManager.GetLog(typeof(CiEntryPointSettingService));

        public CiEntryPointSettingService()
        {
            _appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
            _rulesEngine = ServiceContainer.Resolve<RulesEngine>();
        }

        public CiEntryPointSetting GetById(int id)
        {
            var existingRecord = _appSettings.CiEntryPointSettings.FirstOrDefault(i => i.Id == id);
            return existingRecord;
        }

        private int GetNextId()
        {
            var maxId = _appSettings.CiEntryPointSettings.Max(i => (int?)i.Id) ?? 0;
            var newId = maxId + 1;
            return newId;
        }

        public void Add(InMemoryCiEntryPointSetting ciEntryPointSetting)
        {
            EncryptPassword(ciEntryPointSetting);
            var newId = GetNextId();
            ciEntryPointSetting.Id = newId;
            _appSettings.CiEntryPointSettings.Add(ciEntryPointSetting);
        }

        public void Update(InMemoryCiEntryPointSetting requestCiEntryPointSetting)
        {
            EncryptPassword(requestCiEntryPointSetting);

            var existingRecord = GetById(requestCiEntryPointSetting.Id);
            existingRecord.Url = requestCiEntryPointSetting.Url;
            existingRecord.UserName = requestCiEntryPointSetting.UserName;
            existingRecord.EncryptedPassword = requestCiEntryPointSetting.EncryptedPassword;
            existingRecord.BuildDefinitionSettings = requestCiEntryPointSetting.BuildDefinitionSettings;
        }

        private static void EncryptPassword(InMemoryCiEntryPointSetting requestCiEntryPointSetting)
        {
            requestCiEntryPointSetting.SetPassword(requestCiEntryPointSetting.Password);
            requestCiEntryPointSetting.Password = null;
        }

        public async Task AddUpdate(InMemoryCiEntryPointSetting requestCiEntryPointSetting)
        {
            foreach (var buildDefinitionSetting in requestCiEntryPointSetting.BuildDefinitionSettings)
            {
                buildDefinitionSetting.Active = true;
                buildDefinitionSetting.BuildServer = requestCiEntryPointSetting.Name;
            }

            var incommingId = requestCiEntryPointSetting.Id;
            if (incommingId == 0)
            {
                Add(requestCiEntryPointSetting);
            }
            else
            {
                Update(requestCiEntryPointSetting);
            }
            await SaveSettingsRefreshWatcher();
        }

        private async Task SaveSettingsRefreshWatcher()
        {
            var settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
            await settingsIoService.Save();
            await _rulesEngine.RefreshAll();
        }

        public async Task Delete(int id)
        {
            _log.Debug("Attempting to delete CiEntryPointSetting #" + id);
            var ciEntryPointSetting = GetById(id);
            if (ciEntryPointSetting == null)
            {
                _log.Warn("Tried to delete CiEntryPointSetting #" + id + " but it didn't exist");
                return;
            }
            _appSettings.CiEntryPointSettings.Remove(ciEntryPointSetting);
            await SaveSettingsRefreshWatcher();
        }

        public CiEntryPointSetting GetByIdForUnencryptedCommunication(int? id)
        {
            var ciEntryPointSetting = _appSettings.CiEntryPointSettings.FirstOrDefault(i => i.Id == id);
            if (ciEntryPointSetting == null) return null;
            var result = new CiEntryPointSetting(ciEntryPointSetting);
            result.EncryptedPassword = null;
            return result;
        }
    }
}
