using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Server.Services
{
    public class MessageRelayService
    {
        private readonly ILog _log = MyLogManager.GetLogger(typeof(MessageRelayService));

        private AppServiceConnection _connection;
        private bool _keepConnectionOpen = true;

        private async Task<AppServiceConnection> CachedConnection()
        {
            return _connection ?? (_connection = await MakeConnection());
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
                throw new Exception("Could not connect to app service, error: " + status);
            }
            connection.RequestReceived += ConnectionOnRequestReceived;
            connection.ServiceClosed += ConnectionOnServiceClosed;
            return connection;
        }

        private async void ConnectionOnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            DisposeConnection();
            // If the connection closed, it's probably because the UI was shut down.
            //      However, we want our connection to stay up so we can receive new
            //      notifications if the client re-connects.
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

        public void CloseConnection()
        {
            _keepConnectionOpen = false;
            DisposeConnection();
        }

        private void ConnectionOnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var appServiceDeferral = args.GetDeferral();
            try
            {
                var message = args.Request.Message.First();
                MessageReceived?.Invoke(message.Value as string);
            }
            finally
            {
                appServiceDeferral.Complete();
            }
        }

        public async Task Send(string message)
        {
            try
            {
                var connection = await CachedConnection();
                var result = await connection.SendMessageAsync(
                    new ValueSet
                    {
                        new KeyValuePair<string, object>("ToUi", message)
                    });
                if (result.Status != AppServiceResponseStatus.Success)
                {
                    _log.Error("Error sending message " + message + " because " + result.Status);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error sending message " + message, ex);
            }
        }

        public event Action<string> MessageReceived;
    }
}
