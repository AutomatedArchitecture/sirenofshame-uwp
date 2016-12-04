﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace SirenOfShame.Uwp.MessageRelay
{
    public sealed class StartupTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private Guid _thisConnectionGuid;
        private static readonly Dictionary<Guid, AppServiceConnection> Connections = new Dictionary<Guid, AppServiceConnection>();

        /// <summary>
        /// When an AppServiceConnection of type 'SirenOfShameMessageRelay' (as
        /// defined in Package.appxmanifest) is instantiated and OpenAsync() is called 
        /// on it, then one of these StartupTask's in instantiated and Run() is called.
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                // get a service deferral so the service isn't terminated upon completion of Run()
                _backgroundTaskDeferral = taskInstance.GetDeferral();
                // save a unique identifier for each connection
                _thisConnectionGuid = Guid.NewGuid();
                var triggerDetails = (AppServiceTriggerDetails) taskInstance.TriggerDetails;
                var connection = triggerDetails.AppServiceConnection;
                // save the guid and connection in a *static* list of all connections
                Connections.Add(_thisConnectionGuid, connection);
                await Debug("Connection opened: " + _thisConnectionGuid);
                taskInstance.Canceled += OnTaskCancelled;
                // listen for incoming app service requests
                connection.RequestReceived += ConnectionRequestReceived;
                connection.ServiceClosed += ConnectionOnServiceClosed;
            }
            catch (Exception ex)
            {
                await Debug("Error in startup " + ex);
            }
        }

        private async void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            await Debug("MessageRelay was cancelled");
            if (_backgroundTaskDeferral != null)
            {
                _backgroundTaskDeferral.Complete();
                _backgroundTaskDeferral = null;
            }
        }

        private async void ConnectionOnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            await Debug("Connection closed: " + _thisConnectionGuid);
            RemoveConnection(_thisConnectionGuid);
        }

        private async Task Debug(string message)
        {
            await Task.Yield();

            //// e.g. '\User Folders\LocalAppData\SirenOfShame.Uwp.MessageRelay-uwp_1.0.0.0_arm__n7wdzm614gaee\LocalState\MessageRelayLogs'
            //var logFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("MessageRelayLogs", CreationCollisionOption.OpenIfExists);
            //var messageRelayLogsPath = Path.Combine(logFolder.Path, "MessageRelayLogs.txt");
            //var contents = $"{DateTime.Now} - {message}{Environment.NewLine}";
            //File.AppendAllText(messageRelayLogsPath, contents);
        }

        private async void ConnectionRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            // take out a deferral since we use await
            var appServiceDeferral = args.GetDeferral();
            try
            {
                await Debug("Request initiated by " + _thisConnectionGuid);

                // .ToList() required since connections may get removed during SendMessage()
                var otherConnections = Connections
                    .Where(i => i.Key != _thisConnectionGuid)
                    .ToList();
                foreach (var connection in otherConnections)
                {
                    await SendMessage(connection, args.Request.Message);
                }
            }
            finally
            {
                appServiceDeferral.Complete();
            }
        }

        private async Task SendMessage(KeyValuePair<Guid, AppServiceConnection> connection, ValueSet valueSet)
        {
            try
            {
                var result = await connection.Value.SendMessageAsync(valueSet);
                if (result.Status == AppServiceResponseStatus.Success)
                {
                    await Debug("Successfully sent message to " + connection.Key + ". Result = " + result.Message);
                    return;
                }
                if (result.Status == AppServiceResponseStatus.Failure)
                {
                    // When an app with an open connection is terminated and it fails
                    //      to dispose of its connection, the connection object remains
                    //      in Connections.  When someone tries to send to it, it gets
                    //      an AppServiceResponseStatus.Failure response
                    await Debug("Error sending to " + connection.Key + ".  Removing it from the list of active connections.");
                    RemoveConnection(connection.Key);
                    return;
                }
                await Debug("Error sending to " + connection.Key + " - " + result.Status);
            }
            catch (Exception ex)
            {
                await Debug("Error SendMessage to " + connection.Key + " " + ex);
            }
        }

        private void RemoveConnection(Guid key)
        {
            var connection = Connections[key];
            connection.Dispose();
            Connections.Remove(key);
        }
    }
}