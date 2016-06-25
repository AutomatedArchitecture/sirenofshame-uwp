using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using SirenOfShame.Device;

namespace SirenOfShame.Uwp.Server.Services
{
    public class SirenService
    {
        public static readonly SirenService Instance = new SirenService();
        private readonly SirenOfShameDevice _device;

        private SirenService()
        {
            _device = new SirenOfShameDevice();
            _device.Connected += DeviceOnConnected;
            _device.Disconnected += DeviceOnDisconnected;
        }

        private void DeviceOnDisconnected(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("Device Disconnected");
        }

        private void DeviceOnConnected(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("Device Connected");
        }

        [Deprecated("Violoation of law of demeter", DeprecationType.Deprecate, UInt32.MaxValue)]
        public SirenOfShameDevice Device => _device;

        public bool IsConnected => _device.IsConnected;
        public List<AudioPattern> AudioPatterns => _device.AudioPatterns;
        public List<LedPattern> LedPatterns => _device.LedPatterns;

        public void StartWatching() => _device.StartWatching();

        public async Task PlayLightPattern(LedPattern ledPattern, TimeSpan? duration)
        {
            await _device.PlayLightPattern(ledPattern, duration);
        }

        public async Task PlayAudioPattern(AudioPattern audioPattern, TimeSpan? duration)
        {
            await _device.PlayAudioPattern(audioPattern, duration);
        }
    }
}