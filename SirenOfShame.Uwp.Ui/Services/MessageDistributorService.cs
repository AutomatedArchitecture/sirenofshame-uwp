using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MessageDistributorService
    {
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageDistributorService));

        public event EventHandler<NewNewsItemEventArgs> NewNewsItem;
        public event EventHandler<NewUserEventArgs> NewPerson;
        public event EventHandler<RefreshStatusEventArgs> RefreshStatus;
        public event EventHandler<StatsChangedEventArgs> StatsChanged;

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
                "NewNewsItem",
                new MessageOutlet<NewNewsItemEventArgs>((arg, mds) => mds.NewNewsItem?.Invoke(mds, arg))
            },
            {
                "NewUser",
                new MessageOutlet<NewUserEventArgs>((arg, mds) => mds.NewPerson?.Invoke(mds, arg))
            },
            {
                "RefreshStatus",
                new MessageOutlet<RefreshStatusEventArgs>((arg, mds) => mds.RefreshStatus?.Invoke(mds, arg))
            },
            {
                "StatsChanged",
                new MessageOutlet<StatsChangedEventArgs>((arg, mds) => mds.StatsChanged?.Invoke(mds, arg))
            }
        };

        private void Aggregate(KeyValuePair<string, object> keyValuePair)
        {
            try
            {
                var messageBody = keyValuePair.Value as string;

                foreach (var messageOutlet in _messageOutlets)
                {
                    if (messageOutlet.Key == keyValuePair.Key)
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
            await _messageRelayService.SendMessageAsync("SendLatest", null);
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
