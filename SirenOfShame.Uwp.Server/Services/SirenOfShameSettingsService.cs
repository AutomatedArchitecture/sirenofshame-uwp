using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Services
{
    public class SirenOfShameSettingsService
    {
        public static SirenOfShameSettingsService Instance = new SirenOfShameSettingsService();

        public async Task<StorageFolder> GetSosAppDataFolder()
        {
            var aa = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Automated Architecture", CreationCollisionOption.OpenIfExists);
            var sos = await aa.CreateFolderAsync("SirenOfShame", CreationCollisionOption.OpenIfExists);
            return sos;
        }

        public async Task<SirenOfShameSettings> GetAppSettings()
        {
            var sosFolder = await GetSosAppDataFolder();
            var configFile = await sosFolder.TryGetItemAsync(SirenOfShameSettings.SIRENOFSHAME_CONFIG);
            if (configFile != null)
            {
                var fileStream = new FileStream(configFile.Path, FileMode.Open);
                var streamReader = new StreamReader(fileStream);
                var contents = await streamReader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<SirenOfShameSettings>(contents);
            }

            return SirenOfShameSettings.GetDefaultSettings();
        }

        public async Task Save(CiEntryPointSetting ciEntryPointSetting)
        {
            // todo: implement save
            await Task.Yield();
        }
    }
}
