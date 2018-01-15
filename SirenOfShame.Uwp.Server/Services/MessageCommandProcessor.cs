using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Server.Commands;
using SirenOfShame.Uwp.Watcher;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Server.Services
{
    /// <summary>
    /// Parses incomming requests from the UI (via MesageRelay) then finds and invokes 
    /// the appropriate command.
    /// </summary>
    public class MessageCommandProcessor
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
                var messageBody = keyValuePair.Value as string;

                foreach (var command in CommandBase.Commands)
                {
                    if (command.CommandName == keyValuePair.Key)
                    {
                        var result = await command.Invoke(messageBody);
                        if (result.ResponseCode != 200)
                        {
                            _log.Error("Error for " + command.CommandName + " code: " + result.ResponseCode);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error when receiving " + keyValuePair.Key + ": " + keyValuePair.Value, ex);
            }
        }
    }
}