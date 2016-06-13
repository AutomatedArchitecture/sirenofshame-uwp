﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Device;
using SirenOfShame.Uwp.Background.Services;

namespace SirenOfShame.Uwp.Background.Controllers
{
    internal class LedPatternsController : ApiController
    {
        public override async Task<object> Get(HttpContext context)
        {
            if (SirenService.Instance.Device.IsConnected)
            {
                return SirenService.Instance.Device.LedPatterns;
            }
            return new [] {"No Device Connected"};
        }

        public override async Task Post(HttpContext context)
        {
            if (SirenService.Instance.Device.IsConnected)
            {
                var id = context.GetQuerystringParam("id");
                var durationStr = context.GetQuerystringParam("duration");
                var ledPattern = ToLedPattern(id);
                var duration = ToDuration(durationStr);
                await SirenService.Instance.Device.PlayLightPattern(ledPattern, duration);
            }
        }

        private TimeSpan? ToDuration(string durationStr)
        {
            if (string.IsNullOrEmpty(durationStr)) return null;
            var seconds = int.Parse(durationStr);
            return new TimeSpan(0, 0, seconds);
        }

        private LedPattern ToLedPattern(string id)
        {
            if (string.IsNullOrEmpty(id)) return new LedPattern();
            return new LedPattern {Id = int.Parse(id)};
        }
    }
}
