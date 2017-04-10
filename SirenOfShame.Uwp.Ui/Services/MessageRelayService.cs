using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MessageRelayService
    {
        private AppServiceConnection _connection;
        public event Action<ValueSet> OnMessageReceived;
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageRelayService));

        public bool IsConnected => _connection != null;

        private async Task<AppServiceConnection> CachedConnection()
        {
            if (_connection != null) return _connection;
            _log.Debug("Opening connection to MessageRelay");
            _connection = await MakeConnection();
            _connection.RequestReceived += ConnectionOnRequestReceived;
            _connection.ServiceClosed += ConnectionOnServiceClosed;
            return _connection;
        }

        public async Task Open()
        {
            await CachedConnection();
        }

        private async Task<AppServiceConnection> MakeConnection()
        {
            var appServiceName = "SirenOfShameMessageRelay";
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync(appServiceName);

            if (listing.Count == 0)
            {
                throw new Exception("Unable to find app service '" + appServiceName + "'");
            }
            var packageName = listing[0].PackageFamilyName;

            var connection = new AppServiceConnection
            {
                AppServiceName = appServiceName,
                PackageFamilyName = packageName
            };

            var status = await connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                throw new Exception("Could not connect to MessageRelay, status: " + status);
            }

            return connection;
        }

        private void ConnectionOnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            DisposeConnection();
        }

        private void DisposeConnection()
        {
            if (_connection == null) return;

            _connection.RequestReceived -= ConnectionOnRequestReceived;
            _connection.ServiceClosed -= ConnectionOnServiceClosed;
            _connection.Dispose();
            _connection = null;
        }

        private void ConnectionOnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var appServiceDeferral = args.GetDeferral();
            try
            {
                ValueSet valueSet = args.Request.Message;
                _log.Debug("Received message from MessageRelay: " + ValueSetToString(valueSet));
                OnMessageReceived?.Invoke(valueSet);
            }
            finally
            {
                appServiceDeferral.Complete();
            }
        }

        private string ValueSetToString(ValueSet valueSet)
        {
            if (valueSet.Count > 1)
                return "Multiple ValueSets: " + string.Join(", ", valueSet.Select(i => i.Key));
            var value = valueSet.First();
            if (IsChatty(value.Key)) return value.Key;
            return value.Key + " - " + value.Value;
        }

        private bool IsChatty(string valueKey)
        {
            return valueKey == MessageDistributorService.REFRESHSTATUS;
        }

        public void CloseConnection()
        {
            _log.Debug("Closing connection");
            DisposeConnection();
        }

        private async Task SendMessageAsync(KeyValuePair<string, object> keyValuePair)
        {
            var connection = await CachedConnection();
            var result = await connection.SendMessageAsync(new ValueSet {keyValuePair});
            if (result.Status == AppServiceResponseStatus.Success)
            {
                return;
            }
            throw new EndpointNotFoundException("Error sending " + result.Status);
        }

        public async Task SendMessageAsync(string key, string value)
        {
            await SendMessageAsync(new KeyValuePair<string, object>(key, value));
        }
    }
}
