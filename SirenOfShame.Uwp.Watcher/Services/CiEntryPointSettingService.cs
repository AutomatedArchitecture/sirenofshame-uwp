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

        public CiEntryPointSettingService()
        {
            _appSettings = ServiceContainer.Resolve<SirenOfShameSettings>();
            _rulesEngine = ServiceContainer.Resolve<RulesEngine>();
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

        public void Update(CiEntryPointSetting requestCiEntryPointSetting)
        {
            var existingRecord = GetById(requestCiEntryPointSetting.Id);
            existingRecord.Url = requestCiEntryPointSetting.Url;
            existingRecord.BuildDefinitionSettings = requestCiEntryPointSetting.BuildDefinitionSettings;
        }

        public async Task AddUpdate(CiEntryPointSetting requestCiEntryPointSetting)
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
            var settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
            await settingsIoService.Save();
            await _rulesEngine.RefreshAll();
        }
    }
}
