using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace SirenOfShame.Uwp.Server.Services
{
    public class MessageRelayService
    {
        private AppServiceConnection _connection;

        private async Task<AppServiceConnection> CachedConnection()
        {
            return _connection ?? (_connection = await MakeConnection());
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
            return connection;
        }

        private void ConnectionOnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message.First();
            MessageReceived?.Invoke(message.Value as string);
        }

        public async Task Send(string message)
        {
            var connection = await CachedConnection();
            await connection.SendMessageAsync(
              new ValueSet {
                new KeyValuePair<string, object>("ToUi", message)
              });
        }

        public event Action<string> MessageReceived;
    }
}
