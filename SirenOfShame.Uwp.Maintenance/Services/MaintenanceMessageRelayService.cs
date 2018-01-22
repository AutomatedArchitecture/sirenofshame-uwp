using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Services;

namespace SirenOfShame.Uwp.Maintenance.Services
{
    internal class MaintenanceMessageRelayService : MessageRelayServiceBase
    {
        private AppServiceConnection _connection;
        public event Action<ValueSet> OnMessageReceived;
        private readonly ILog _log = MyLogManager.GetLog(typeof(MaintenanceMessageRelayService));

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

        public override async Task Open()
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

        protected override void DisposeConnection()
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

        protected override async Task SendMessageAsync(KeyValuePair<string, object> keyValuePair)
        {
            var connection = await CachedConnection();
            var result = await connection.SendMessageAsync(new ValueSet { keyValuePair });
            if (result.Status == AppServiceResponseStatus.Success)
            {
                return;
            }
            throw new EndpointNotFoundException("Error sending " + result.Status);
        }
    }
}
