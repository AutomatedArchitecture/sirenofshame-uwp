using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace SirenOfShame.Uwp.MessageRelay
{
    public sealed class StartupTask : IBackgroundTask
    {
        // ReSharper disable once NotAccessedField.Local
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private AppServiceConnection _connection;
        private static List<AppServiceConnection> _receivedConnections = new List<AppServiceConnection>();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            _backgroundTaskDeferral = taskInstance.GetDeferral();
            ListenForMessages(taskInstance);
        }

        private void ListenForMessages(IBackgroundTaskInstance taskInstance)
        {
            var triggerDetails = (AppServiceTriggerDetails)taskInstance.TriggerDetails;
            _connection = triggerDetails.AppServiceConnection;
            _connection.RequestReceived += ConnectionRequestReceived;
        }

        private async void ConnectionRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            _receivedConnections.Add(sender);
            var value = (string)args.Request.Message.First().Value;
            System.Diagnostics.Debug.WriteLine("Received message: " + value);
            ValueSet message = new ValueSet
            {
                new KeyValuePair<string, object>("Message", "Echo " + value)
            };
            foreach (var connection in _receivedConnections)
            {
                await connection.SendMessageAsync(message);
            }
        }
    }
}
