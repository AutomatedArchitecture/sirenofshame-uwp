using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Services;
using SirenOfShame.Uwp.Watcher.Services;

namespace SirenOfShame.Uwp.Ui.Services
{
    /// <summary>
    /// Receives all messages sent from RulesEngineWatcher, breaks them into message types, then
    /// distributes them as events that any interested parties can subscribe to (usually MainUiPage).
    /// </summary>
    public class MessageDistributorService : MessageCommandProcessorBase
    {
        private readonly UiMessageRelayService _messageRelayService = ServiceContainer.Resolve<UiMessageRelayService>();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageDistributorService));

        public event EventHandler<NewNewsItemEventArgs> NewNewsItem;
        public event EventHandler<NewUserEventArgs> NewPerson;
        public event EventHandler<RefreshStatusEventArgs> RefreshStatus;
        public event EventHandler<StatsChangedEventArgs> StatsChanged;
        public event EventHandler<SetTrayIconEventArgs> SetTrayIcon;
        public event EventHandler<UpdateStatusBarEventArgs> UpdateStatusBar;

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

        private readonly Dictionary<string, MessageOutletBase> _messageOutlets = new Dictionary<string, MessageOutletBase>
        {
            {
                NewNewsItemEventArgs.COMMAND_NAME,
                new MessageOutlet<NewNewsItemEventArgs>((arg, mds) => mds.NewNewsItem?.Invoke(mds, arg))
            },
            {
                NewUserEventArgs.COMMAND_NAME,
                new MessageOutlet<NewUserEventArgs>((arg, mds) => mds.NewPerson?.Invoke(mds, arg))
            },
            {
                RefreshStatusEventArgs.COMMAND_NAME,
                new MessageOutlet<RefreshStatusEventArgs>((arg, mds) => mds.RefreshStatus?.Invoke(mds, arg))
            },
            {
                StatsChangedEventArgs.COMMAND_NAME,
                new MessageOutlet<StatsChangedEventArgs>((arg, mds) => mds.StatsChanged?.Invoke(mds, arg))
            },
            {
                SetTrayIconEventArgs.COMMAND_NAME,
                new MessageOutlet<SetTrayIconEventArgs>((arg, mds) => mds.SetTrayIcon?.Invoke(mds, arg))
            },
            {
                UpdateStatusBarEventArgs.COMMAND_NAME,
                new MessageOutlet<UpdateStatusBarEventArgs>((arg, mds) => mds.UpdateStatusBar?.Invoke(mds, arg))
            },
        };

        private void Aggregate(KeyValuePair<string, object> keyValuePair)
        {
            try
            {
                ParseMessage(keyValuePair.Key, keyValuePair.Value, out var messageDestination, out var key, out var messageBody);
                if (messageDestination != MessageDestination.AppUi && messageDestination != MessageDestination.All) return;
               
                foreach (var messageOutlet in _messageOutlets)
                {
                    if (messageOutlet.Key == key)
                    {
                        messageOutlet.Value.Invoke(messageBody, this);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error when receiving " + keyValuePair.Key + ": " + keyValuePair.Value, ex);
            }
        }

        public async Task SendLatest()
        {
            await _messageRelayService.SendMessageAsync(MessageDestination.Server, "SendLatest", null);
        }

        abstract class MessageOutletBase
        {
            public abstract void Invoke(string messageBody, MessageDistributorService mds);
        }

        class MessageOutlet<T> : MessageOutletBase
        {
            private Action<T, MessageDistributorService> Action { get; }

            public MessageOutlet(Action<T, MessageDistributorService> action)
            {
                Action = action;
            }

            public override void Invoke(string messageBody, MessageDistributorService mds)
            {
                var result = JsonConvert.DeserializeObject<T>(messageBody);
                Action(result, mds);
            }
        }
    }
}
