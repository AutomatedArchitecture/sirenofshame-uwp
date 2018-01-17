using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Core.Services;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class UiMessageRelayService : MessageRelayServiceBase
    {
        private AppServiceConnection _connection;
        public event Action<ValueSet> OnMessageReceived;
        private readonly ILog _log = MyLogManager.GetLog(typeof(UiMessageRelayService));

        public bool IsConnected => _connection != null;

        private async Task<AppServiceConnection> CachedConnection()
        {
            if (_connection != null) return _connection;
            await _log.Debug("Opening connection to MessageRelay");
            _connection = await MakeConnection();
            await _log.Debug("Successfully opened connection to MessageRelay");
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
            var packageName = await TryFindMessageRelayAppPackageFamilyNameWithRetry();
            if (packageName == null)
            {
                throw new Exception("Unable to find app service '" + APP_SERVICE_NAME + "'");
            }

            var connection = new AppServiceConnection
            {
                AppServiceName = APP_SERVICE_NAME,
                PackageFamilyName = packageName
            };

            var status = await connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                throw new Exception("Could not connect to MessageRelay, status: " + status);
            }

            return connection;
        }

        protected override async Task<string> TryFindMessageRelayAppPackageFamilyName()
        {
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync(APP_SERVICE_NAME);
            return listing.FirstOrDefault()?.PackageFamilyName;
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
            catch (Exception ex)
            {
                _log.Error("Error processing ConnectionRequestReceived " + ValueSetToString(args.Request.Message), ex);
                // continue, since throwing exceptions here is liable to crash the MessageRelay
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
            return valueKey == RefreshStatusEventArgs.COMMAND_NAME;
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
            if (result.Status != AppServiceResponseStatus.Success)
            {
                await _log.Error("Error sending message " + keyValuePair.Key + " because " + result.Status);
                throw new EndpointNotFoundException("Error sending " + result.Status);
            }
        }

        public async Task SendMessageAsync(string key, string value)
        {
            await SendMessageAsync(new KeyValuePair<string, object>(key, value));
        }
    }
}
