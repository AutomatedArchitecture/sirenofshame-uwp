﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Device;
using SirenOfShame.Uwp.Server.Models;
using SirenOfShame.Uwp.Server.Services;

namespace SirenOfShame.Uwp.Server.Commands
{
    internal class PlayAudioPatternCommand : CommandBase
    {
        public override string CommandName => "playAudioPattern";
        public override async Task<SocketResult> Invoke(string frame)
        {
            var playAudioRequest = JsonConvert.DeserializeObject<PlayAudioRequest>(frame);

            if (SirenService.Instance.IsConnected)
            {
                var id = playAudioRequest.Id;
                var durationStr = playAudioRequest.Duration;
                var audioPattern = ToAudioPattern(id);
                var duration = ToDuration(durationStr);
                await SirenService.Instance.PlayAudioPattern(audioPattern, duration);
            }

            return new OkSocketResult();
        }

        private TimeSpan? ToDuration(int? duration)
        {
            if (duration == null) return null;
            return new TimeSpan(0, 0, duration.Value);
        }

        private AudioPattern ToAudioPattern(int? id)
        {
            if (id == null) return new AudioPattern();
            return new AudioPattern { Id = id.Value };
        }
    }
}