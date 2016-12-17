using System;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Ui.Services.MessageParsers;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MessageAggregatorService
    {
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();

        public event EventHandler<NewNewsItemEventArgs> NewNewsItem;

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
            var newNewsMessageParser = new NewNewsMessageParser();
            var value = keyValuePair.Value as string;
            if (keyValuePair.Key == newNewsMessageParser.Key)
            {
                var result = newNewsMessageParser.Parse(value);
                NewNewsItem?.Invoke(this, result);
            }
        }
    }
}
