﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Watcher;

namespace SirenOfShame.Uwp.Server.Services
{
    public class ServerMessageRelayService
    {
        private AppServiceConnection _connection;
        public event Action<ValueSet> OnMessageReceived;
        private readonly ILog _log = MyLogManager.GetLog(typeof(ServerMessageRelayService));

        private async Task<AppServiceConnection> CachedConnection()
        {
            if (_connection != null) return _connection;
            await _log.Debug("Opening connection to MessageRelay");
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
                throw new Exception("Could not connect to app service, error: " + status);
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

        public void CloseConnection()
        {
            DisposeConnection();
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

        public async Task Send(string key, string message)
        {
            try
            {
                var connection = await CachedConnection();
                var keyValuePair = new KeyValuePair<string, object>(key, message);
                var result = await connection.SendMessageAsync(new ValueSet { keyValuePair });
                if (result.Status != AppServiceResponseStatus.Success)
                {
                    await _log.Error("Error sending message " + message + " because " + result.Status);
                }
            }
            catch (Exception ex)
            {
                await _log.Error("Error sending message " + message, ex);
            }
        }
    }
}