using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace SirenOfShame.Uwp.Maintenance.Services
{
    internal class MaintenanceMessageRelayService
    {
        private AppServiceConnection _connection;

        public bool IsConnected => _connection != null;

        private async Task<AppServiceConnection> CachedConnection()
        {
            if (_connection != null) return _connection;
            _connection = await MakeConnection();
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

            _connection.ServiceClosed -= ConnectionOnServiceClosed;
            _connection.Dispose();
            _connection = null;
        }

        public void CloseConnection()
        {
            DisposeConnection();
        }

        private async Task SendMessageAsync(KeyValuePair<string, object> keyValuePair)
        {
            var connection = await CachedConnection();
            var result = await connection.SendMessageAsync(new ValueSet { keyValuePair });
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
