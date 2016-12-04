using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace SirenOfShame.Uwp.Ui.Services
{
    public class MessageRelayService
    {
        private AppServiceConnection _connection;
        public string Status { get; set; }
        public event Action<ValueSet> OnMessageReceived;
        bool _keepConnectionOpen = true;

        private async Task<AppServiceConnection> CachedConnection()
        {
            return _connection ?? (_connection = await MakeConnection());
        }

        public async Task Open()
        {
            try
            {
                await CachedConnection();
            }
            catch (Exception ex)
            {
                Status = "AppService Error: " + ex.Message;
            }
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
                Status = "Could not connect: " + status;
                return null;
            }

            Status = "Connected: " + status;
            connection.RequestReceived += ConnectionOnRequestReceived;
            connection.ServiceClosed += ConnectionOnServiceClosed;
            return connection;
        }

        private async void ConnectionOnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            DisposeConnection();
            // If the connection closed, it's probably because the Server was shut down.
            //      However, we want our connection to stay up so we can receive new
            //      notifications if the server re-connects.
            if (_keepConnectionOpen)
            {
                await CachedConnection();
            }
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
                OnMessageReceived?.Invoke(valueSet);
            }
            finally
            {
                appServiceDeferral.Complete();
            }
        }

        public void CloseConnection()
        {
            _keepConnectionOpen = false;
            DisposeConnection();
        }

        public async Task SendMessageAsync(KeyValuePair<string, object> keyValuePair)
        {
            var connection = await CachedConnection();
            var result = await connection.SendMessageAsync(new ValueSet {keyValuePair});
            if (result.Status == AppServiceResponseStatus.Success)
            {
                return;
            }
            throw new Exception("Error sending " + result.Status);
        }

        public async Task SendMessageAsync(string key, string value)
        {
            await SendMessageAsync(new KeyValuePair<string, object>(key, value));
        }
    }
}
