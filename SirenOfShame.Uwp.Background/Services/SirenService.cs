using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using SirenOfShame.Device;

namespace SirenOfShame.Uwp.Background.Services
{
    internal class SirenService
    {
        public static readonly SirenService Instance = new SirenService();
        private readonly SirenOfShameDevice _device;

        private SirenService()
        {
            _device = new SirenOfShameDevice();
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
    }
}
