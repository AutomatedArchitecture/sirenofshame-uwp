using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Device;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;
using SirenOfShame.Uwp.Shared.Commands;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Commands.Siren
{
    internal class PlayLedPatternCommand : CommandBase
    {
        public override string CommandName => CommandNames.PLAY_LED_PATTERN;
        private readonly SirenDeviceService _sirenDeviceService;

        public PlayLedPatternCommand()
        {
            _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();
        }

        public override async Task<SocketResult> Invoke(string frame)
        {
            var playLedRequest = JsonConvert.DeserializeObject<PlayLedRequest>(frame);

            if (_sirenDeviceService.IsConnected)
            {
                var id = playLedRequest.Id;
                var durationStr = playLedRequest.Duration;
                var ledPattern = ToLedPattern(id);
                var duration = ToDuration(durationStr);
                await _sirenDeviceService.PlayLightPattern(ledPattern, duration);
            }

            return new OkSocketResult();
        }

        private TimeSpan? ToDuration(int? duration)
        {
            if (duration == null) return null;
            return new TimeSpan(0, 0, duration.Value);
        }

        private LedPattern ToLedPattern(int? id)
        {
            if (id == null) return new LedPattern();
            return new LedPattern { Id = id.Value };
        }
    }
}
