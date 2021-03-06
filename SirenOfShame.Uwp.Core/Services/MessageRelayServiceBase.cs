﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using SirenOfShame.Uwp.Core.Interfaces;

namespace SirenOfShame.Uwp.Core.Services
{
    public enum MessageDestination
    {
        All = 0,
        AppUi = 1,
        Server = 2,
        Maintenance = 3,
        WebUi = 4
    }
    
    public abstract class MessageRelayServiceBase
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageRelayServiceBase));
        protected const string APP_SERVICE_NAME = "SirenOfShameMessageRelay";

        protected abstract void DisposeConnection();

        protected abstract Task SendMessageAsync(KeyValuePair<string, object> keyValuePair);

        public abstract Task Open();

        private async Task OnSendTimeout()
        {
            await _log.Warn("Timeout occurred, closing connection");
            DisposeConnection();
            await _log.Debug("Reopening connection");
            await Open();
        }

        /// <summary>
        /// On send do pecimistic timeouts and if they happen close the connection and reopen it and retry
        /// </summary>
        protected async Task TrySendWithTimeout(KeyValuePair<string, object> keyValuePair, int seconds = 20)
        {
            var timeoutPolicy = Policy.TimeoutAsync(seconds, 
                TimeoutStrategy.Pessimistic);

            var retryPolicy = Policy.Handle<Exception>()
                .RetryAsync((exception, retryCount, context) => OnSendTimeout());

            await retryPolicy.WrapAsync(timeoutPolicy)
                .ExecuteAsync(() => SendMessageAsync(keyValuePair));
        }

        protected async Task<string> TryFindMessageRelayAppPackageFamilyNameWithRetry()
        {
            var policyResult = await Policy.HandleResult<string>(i => i == null)
                .WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(15),
                        TimeSpan.FromSeconds(20),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(30),
                    },
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _log.Warn($"Can't find MessageRelay app.  Attempt #{retryCount}. Trying again in {timeSpan.TotalSeconds} seconds.");
                    })
                .ExecuteAndCaptureAsync(TryFindMessageRelayAppPackageFamilyName);
            return policyResult.Result;
        }

        protected abstract Task<string> TryFindMessageRelayAppPackageFamilyName();

        public async Task SendMessageAsync(MessageDestination destination, string key, string value)
        {
            var dkey = destination + "," + key;
            var keyValuePair = new KeyValuePair<string, object>(dkey, value);
            await TrySendWithTimeout(keyValuePair);
        }
    }
}
