using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Watcher.Settings
{
    public class SettingsIoService
    {
        public const string SIRENOFSHAME_CONFIG = @"SirenOfShame.config";

        private static readonly List<Rule> _defaultRules = new List<Rule>{
            new Rule { TriggerType = TriggerType.BuildTriggered, AlertType = AlertType.TrayAlert, BuildDefinitionId = null, TriggerPerson = null, InheritAudioSettings = true, InheritLedSettings = true, WindowsAudioLocation = SoundService.NEW_RESOURCE_PREFIX + "Plunk.wav" },
            new Rule { TriggerType = TriggerType.InitialFailedBuild, AlertType = AlertType.ModalDialog, BuildDefinitionId = null, TriggerPerson = null, InheritAudioSettings = true, InheritLedSettings = true, WindowsAudioLocation  = SoundService.NEW_RESOURCE_PREFIX + "Sad-Trombone.wav" },
            new Rule { TriggerType = TriggerType.SubsequentFailedBuild, AlertType = AlertType.TrayAlert, BuildDefinitionId = null, TriggerPerson = null, InheritAudioSettings = true, InheritLedSettings = true, WindowsAudioLocation = SoundService.NEW_RESOURCE_PREFIX + "Boo-Hiss.wav" },
            new Rule { TriggerType = TriggerType.SuccessfulBuild, AlertType = AlertType.TrayAlert, BuildDefinitionId = null, TriggerPerson = null, InheritAudioSettings = true, InheritLedSettings = true, WindowsAudioLocation = null },
        };

        public async Task<SirenOfShameSettings> GetFromDiskOrDefault()
        {
            IFileAdapter fileAdapter = ServiceContainer.Resolve<IFileAdapter>();
            var configFileExists = await fileAdapter.Exists(SIRENOFSHAME_CONFIG);
            if (!configFileExists)
            {
                return GetDefaultSettings();
            }
            var fileContents = await fileAdapter.ReadTextAsync(SIRENOFSHAME_CONFIG);
            return JsonConvert.DeserializeObject<SirenOfShameSettings>(fileContents);
        }

        public async Task DeleteSettings()
        {
            IFileAdapter fileAdapter = ServiceContainer.Resolve<IFileAdapter>();
            var configFileExists = await fileAdapter.Exists(SIRENOFSHAME_CONFIG);
            if (configFileExists)
            {
                await fileAdapter.DeleteAsync(SIRENOFSHAME_CONFIG);
            }
        }

        public async Task Save()
        {
            IFileAdapter fileAdapter = ServiceContainer.Resolve<IFileAdapter>();
            SirenOfShameSettings settings = ServiceContainer.Resolve<SirenOfShameSettings>();
            var contents = JsonConvert.SerializeObject(settings);
            await fileAdapter.WriteTextAsync(SIRENOFSHAME_CONFIG, contents);
        }

        public SirenOfShameSettings GetDefaultSettings()
        {
            return new SirenOfShameSettings
            {
                Rules = _defaultRules,
                PollInterval = 5
            };
        }

    }
}
