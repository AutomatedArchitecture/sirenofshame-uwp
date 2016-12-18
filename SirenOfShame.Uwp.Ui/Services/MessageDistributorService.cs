using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation.Collections;
using Newtonsoft.Json;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MessageDistributorService
    {
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageDistributorService));

        public event EventHandler<NewNewsItemEventArgs> NewNewsItem;
        public event EventHandler<PersonSetting> NewPerson;
        public event EventHandler<RefreshStatusEventArgs> RefreshStatus;
        public event EventHandler<PersonSetting[]> StatsChanged;

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
                var messageBody = keyValuePair.Value as string;
                if (keyValuePair.Key == "NewNewsItem")
                {
                    var result = JsonConvert.DeserializeObject<NewNewsItemEventArgs>(messageBody);
                    NewNewsItem?.Invoke(this, result);
                    return;
                }
                if (keyValuePair.Key == "NewUser")
                {
                    var result = JsonConvert.DeserializeObject<PersonSetting>(messageBody);
                    NewPerson?.Invoke(this, result);
                    return;
                }
                if (keyValuePair.Key == "RefreshStatus")
                {
                    var result = JsonConvert.DeserializeObject<RefreshStatusEventArgs>(messageBody);
                    RefreshStatus?.Invoke(this, result);
                    return;
                }
                if (keyValuePair.Key == "StatsChanged")
                {
                    var result = JsonConvert.DeserializeObject<PersonSetting[]>(messageBody);
                    StatsChanged?.Invoke(this, result);
                    return;
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error when receiving " + keyValuePair.Key + ": " + keyValuePair.Value, ex);
            }
        }
    }
}
