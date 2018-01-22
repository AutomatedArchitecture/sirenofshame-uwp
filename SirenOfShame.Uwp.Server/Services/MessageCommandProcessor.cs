using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Server.Commands;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Services
{
    /// <summary>
    /// Parses incomming requests from the UI (via MesageRelay) then finds and invokes 
    /// the appropriate command.
    /// </summary>
    public class MessageCommandProcessor : MessageCommandProcessorBase
    {
        private readonly ServerMessageRelayService _messageRelayService = ServiceContainer.Resolve<ServerMessageRelayService>();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageCommandProcessor));

        public void StartWatching()
        {
            _messageRelayService.OnMessageReceived += MessageRelayServiceOnOnMessageReceived;
        }

        private async void MessageRelayServiceOnOnMessageReceived(ValueSet valueSet)
        {
            foreach (var keyValuePair in valueSet)
            {
                await Aggregate(keyValuePair);
            }
        }

        private async Task Aggregate(KeyValuePair<string, object> keyValuePair)
        {
            try
            {
                ParseMessage(keyValuePair.Key, keyValuePair.Value, out var messageDestination, out var key, out var messageBody);
                
                if (messageDestination != MessageDestination.Server && messageDestination != MessageDestination.All) return;

                foreach (var command in CommandBase.Commands)
                {
                    if (command.CommandName == key)
                    {
                        await _log.Debug("Invoking command " + command.CommandName);
                        var result = await command.Invoke(messageBody);
                        if (result.ResponseCode != 200)
                        {
                            await _log.Error("Error for " + command.CommandName + " code: " + result.ResponseCode);
                        }

                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await _log.Error("Error when receiving " + keyValuePair.Key + ": " + keyValuePair.Value, ex);
            }
        }
    }
}