using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Services
{
    public class RulesEngineService
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(RulesEngineService));
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private readonly SirenDeviceService _sirenDeviceService = ServiceContainer.Resolve<SirenDeviceService>();

        public async Task StartCiWatcher()
        {
            try
            {
                var rulesEngine = ServiceContainer.Resolve<RulesEngine>();
                rulesEngine.SetLights += RulesEngineOnSetLights;
                rulesEngine.SetAudio += RulesEngineOnSetAudio;
                rulesEngine.RefreshStatus += RulesEngineOnRefreshStatus;
                rulesEngine.NewNewsItem += RulesEngineOnNewNewsItem;
                rulesEngine.NewUser += RulesEngineOnNewUser;
                rulesEngine.StatsChanged += RulesEngineOnStatsChanged;
                rulesEngine.UpdateStatusBar += UpdateStatusBar;
                rulesEngine.SetTrayIcon += SetTrayIcon;
                await rulesEngine.Start(true);
            }
            catch (Exception ex)
            {
                _log.Error("Error starting CI watcher", ex);
            }
        }

        private async void RulesEngineOnStatsChanged(object sender, StatsChangedEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("StatsChanged", argsAsJson);
        }

        private async void RulesEngineOnNewUser(object sender, NewUserEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("NewUser", argsAsJson);
        }

        private async void RulesEngineOnNewNewsItem(object sender, NewNewsItemEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send("NewNewsItem", argsAsJson);
        }

        private async void RulesEngineOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            // aka goto MessageDistributorService.RefreshStatus
            await _messageRelayService.Send("RefreshStatus", argsAsJson);
        }

        private void SetTrayIcon(object sender, SetTrayIconEventArgs args)
        {

        }

        private void UpdateStatusBar(object sender, UpdateStatusBarEventArgs args)
        {

        }

        private async void RulesEngineOnSetAudio(object sender, SetAudioEventArgs args)
        {
            if (_sirenDeviceService.IsConnected)
            {
                await _sirenDeviceService.PlayAudioPattern(args.AudioPattern, args.TimeSpan);
            }
        }

        private async void RulesEngineOnSetLights(object sender, SetLightsEventArgs args)
        {
            if (_sirenDeviceService.IsConnected)
            {
                await _sirenDeviceService.PlayLightPattern(args.LedPattern, args.TimeSpan);
            }
        }
    }
}
