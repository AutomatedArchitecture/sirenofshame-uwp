using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Server.Services
{
    /// <summary>
    /// Watches the rules engine service and routes messages to UI (MessageDistributorService)
    /// or to other services such as siren of shame device
    /// </summary>
    public class RulesEngineWatcher
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(RulesEngineWatcher));
        private readonly ServerMessageRelayService _messageRelayService = ServiceContainer.Resolve<ServerMessageRelayService>();
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
            await _messageRelayService.Send(StatsChangedEventArgs.COMMAND_NAME, argsAsJson);
        }

        private async void RulesEngineOnNewUser(object sender, NewUserEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send(NewUserEventArgs.COMMAND_NAME, argsAsJson);
        }

        private async void RulesEngineOnNewNewsItem(object sender, NewNewsItemEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send(NewNewsItemEventArgs.COMMAND_NAME, argsAsJson);
        }

        private async void RulesEngineOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send(RefreshStatusEventArgs.COMMAND_NAME, argsAsJson);
        }

        private async void SetTrayIcon(object sender, SetTrayIconEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send(SetTrayIconEventArgs.COMMAND_NAME, argsAsJson);
        }

        private async void UpdateStatusBar(object sender, UpdateStatusBarEventArgs args)
        {
            var argsAsJson = JsonConvert.SerializeObject(args);
            await _messageRelayService.Send(UpdateStatusBarEventArgs.COMMAND_NAME, argsAsJson);
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
