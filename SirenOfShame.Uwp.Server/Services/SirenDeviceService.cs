using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using SirenOfShame.Device;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Server.Services
{
    public class SirenDeviceService
    {
        private readonly SirenOfShameDevice _device;
        private readonly ILog _log = MyLogManager.GetLog(typeof(SirenDeviceService));
        private SettingsIoService _settingsIoService = ServiceContainer.Resolve<SettingsIoService>();

        public SirenDeviceService()
        {
            _device = new SirenOfShameDevice();
            _device.Connected += DeviceOnConnected;
            _device.Disconnected += DeviceOnDisconnected;
        }

        private void DeviceOnDisconnected(object sender, EventArgs eventArgs)
        {
            _log.Debug("Device Disconnected");
        }

        private async void DeviceOnConnected(object sender, EventArgs eventArgs)
        {
            _log.Debug("Device Connected");

            var settings = ServiceContainer.Resolve<SirenOfShameSettings>();
            bool firstTimeSirenHasEverBeenConnected = !settings.SirenEverConnected;
            if (firstTimeSirenHasEverBeenConnected)
            {
                settings.SirenEverConnected = true;
                var firstLedPattern = ToDeviceLedPatten(_device.LedPatterns.First());
                var firstAudioPattern = ToDeviceAudioPatten(_device.AudioPatterns.First());
                settings.InitializeRulesForConnectedSiren(firstAudioPattern, firstLedPattern);
                await _settingsIoService.Save();
            }

            //EnableSirenMenuItem(true);
        }

        [Deprecated("Violoation of law of demeter", DeprecationType.Deprecate, UInt32.MaxValue)]
        public SirenOfShameDevice Device => _device;

        public bool IsConnected => _device.IsConnected;
        public List<AudioPattern> AudioPatterns => _device.AudioPatterns;
        public List<LedPattern> LedPatterns => _device.LedPatterns;
        public event EventHandler Connected
        {
            add { _device.Connected += value; }
            remove { _device.Connected -= value; }
        }

        public event EventHandler Disconnected
        {
            add { _device.Disconnected += value; }
            remove { _device.Disconnected -= value; }
        }

        public void StartWatching() => _device.StartWatching();

        public async Task PlayLightPattern(LedPattern ledPattern, TimeSpan? duration)
        {
            await _device.PlayLightPattern(ledPattern, duration);
        }

        public async Task PlayAudioPattern(Watcher.Device.AudioPattern deviceAudioPattern, TimeSpan? duration)
        {
            var audioPattern = ToRealAudioPatten(deviceAudioPattern);
            await _device.PlayAudioPattern(audioPattern, duration);
        }

        public async Task PlayAudioPattern(AudioPattern audioPattern, TimeSpan? duration)
        {
            await _device.PlayAudioPattern(audioPattern, duration);
        }

        public Task PlayLightPattern(Watcher.Device.LedPattern deviceLedPattern, TimeSpan? timeSpan)
        {
            var ledPattern = ToRealLedPatten(deviceLedPattern);
            return PlayLightPattern(ledPattern, timeSpan);
        }

        private static LedPattern ToRealLedPatten(Watcher.Device.LedPattern deviceLedPattern)
        {
            if (deviceLedPattern == null) return null;
            LedPattern ledPattern = new LedPattern
            {
                Id = deviceLedPattern.Id,
                Name = deviceLedPattern.Name
            };
            return ledPattern;
        }

        private static AudioPattern ToRealAudioPatten(Watcher.Device.AudioPattern deviceAudioPattern)
        {
            if (deviceAudioPattern == null) return null;
            AudioPattern audioPatten = new AudioPattern
            {
                Id = deviceAudioPattern.Id,
                Name = deviceAudioPattern.Name
            };
            return audioPatten;
        }

        private static Watcher.Device.LedPattern ToDeviceLedPatten(LedPattern deviceLedPattern)
        {
            var ledPattern = new Watcher.Device.LedPattern
            {
                Id = deviceLedPattern.Id,
                Name = deviceLedPattern.Name
            };
            return ledPattern;
        }

        private static Watcher.Device.AudioPattern ToDeviceAudioPatten(AudioPattern deviceAudioPattern)
        {
            var audioPatten = new Watcher.Device.AudioPattern
            {
                Id = deviceAudioPattern.Id,
                Name = deviceAudioPattern.Name
            };
            return audioPatten;
        }
    }
}