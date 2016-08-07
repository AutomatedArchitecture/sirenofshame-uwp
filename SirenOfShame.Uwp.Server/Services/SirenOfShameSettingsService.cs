using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Services
{
    public class SirenOfShameSettingsService
    {
        public static SirenOfShameSettingsService Instance = new SirenOfShameSettingsService();
        private SirenOfShameSettings _settingsCache;
        readonly Mutex _retrievingSettings = new Mutex(false);

        public async Task<StorageFolder> GetSosAppDataFolder()
        {
            var aa = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Automated Architecture", CreationCollisionOption.OpenIfExists);
            var sos = await aa.CreateFolderAsync("SirenOfShame", CreationCollisionOption.OpenIfExists);
            return sos;
        }

        public async Task<SirenOfShameSettings> GetAppSettings()
        {
            if (_settingsCache != null) return _settingsCache;
            // don't allow more than one thread into the following block
            _retrievingSettings.WaitOne();
            try
            {
                // if multiple threads were waiting _settingsCache is probably now not-null
                if (_settingsCache != null) return _settingsCache;
                _settingsCache = await GetAppSettingsFromDiskOrDefault();
                return _settingsCache;
            }
            finally
            {
                _retrievingSettings.ReleaseMutex();
            }
        }

        private async Task<SirenOfShameSettings> GetAppSettingsFromDiskOrDefault()
        {
            var sosFolder = await GetSosAppDataFolder();
            var configFile = await sosFolder.TryGetItemAsync(SirenOfShameSettings.SIRENOFSHAME_CONFIG);
            if (configFile != null)
            {
                using (var fileStream = new FileStream(configFile.Path, FileMode.Open))
                using (var streamReader = new StreamReader(fileStream))
                {
                    var contents = await streamReader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<SirenOfShameSettings>(contents);
                }
            }

            return SirenOfShameSettings.GetDefaultSettings();
        }

        public async Task Save(SirenOfShameSettings settings)
        {
            var sosFolder = await GetSosAppDataFolder();
            using (var fileStream = new FileStream(Path.Combine(sosFolder.Path, SirenOfShameSettings.SIRENOFSHAME_CONFIG), FileMode.OpenOrCreate))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                var contents = JsonConvert.SerializeObject(settings);
                await streamWriter.WriteAsync(contents);
            }
        }

        public async Task DeleteSettings()
        {
            var sosFolder = await GetSosAppDataFolder();
            var configFile = await sosFolder.GetFileAsync(SirenOfShameSettings.SIRENOFSHAME_CONFIG);
            await configFile.DeleteAsync();
        }
    }
}
