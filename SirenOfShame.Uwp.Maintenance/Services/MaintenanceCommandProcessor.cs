using System;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Shared.Commands;

namespace SirenOfShame.Uwp.Maintenance.Services
{
    internal sealed class MaintenanceCommandProcessor : MessageCommandProcessorBase
    {
        private readonly MaintenanceMessageRelayService _messageRelayService;
        private readonly ILog _log = MyLogManager.GetLog(typeof(MaintenanceCommandProcessor));

        public MaintenanceCommandProcessor(MaintenanceMessageRelayService maintenanceMessageRelayService)
        {
            _messageRelayService = maintenanceMessageRelayService;
        }

        public event EventHandler TryUpgrade;

        public void StartWatching()
        {
            _messageRelayService.OnMessageReceived += MessageRelayServiceOnOnMessageReceived;
        }

        private void MessageRelayServiceOnOnMessageReceived(ValueSet valueSet)
        {
            foreach (var keyValuePair in valueSet)
            {
                Aggregate(keyValuePair);
            }
        }

        private void Aggregate(KeyValuePair<string, object> keyValuePair)
        {
            try
            {
                ParseMessage(keyValuePair.Key, keyValuePair.Value, out var messageDestination, out var key, out var messageBody);
                if (messageDestination != MessageDestination.Maintenance) return;

                if (key == CommandNames.TRY_UPGRADE)
                {
                    TryUpgrade?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error when receiving " + keyValuePair.Key + ": " + keyValuePair.Value, ex);
            }
        }
    }
}
