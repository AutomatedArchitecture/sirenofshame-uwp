using System;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MessageAggregatorService
    {
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageAggregatorService));

        public event EventHandler<NewNewsItemEventArgs> NewNewsItem;
        public event EventHandler<PersonSetting> NewPerson;

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
                var value = keyValuePair.Value as string;
                if (keyValuePair.Key == "NewNewsItem")
                {
                    var result = JsonConvert.DeserializeObject<NewNewsItemEventArgs>(value);
                    NewNewsItem?.Invoke(this, result);
                }
                if (keyValuePair.Key == "NewUser")
                {
                    var result = JsonConvert.DeserializeObject<PersonSetting>(value);
                    NewPerson?.Invoke(this, result);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error when receiving " + keyValuePair.Key + ": " + keyValuePair.Value, ex);
            }
        }
    }
}
