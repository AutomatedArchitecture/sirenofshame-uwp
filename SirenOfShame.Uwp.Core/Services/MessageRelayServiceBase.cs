using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;
using SirenOfShame.Uwp.Core.Interfaces;

namespace SirenOfShame.Uwp.Core.Services
{
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

        protected async Task TrySendWithTimeout(KeyValuePair<string, object> keyValuePair, int seconds = 20)
        {
            await Policy.Handle<Exception>()
                .RetryAsync((exception, count, context) => OnSendTimeout())
                .ExecuteAsync(() => SendMessageAsync(keyValuePair));

            //await Policy.TimeoutAsync(seconds, TimeoutStrategy.Pessimistic, (context, span, arg3) => OnSendTimeout())
            //    .ExecuteAndCaptureAsync(() => SendMessageAsync(keyValuePair));
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
    }
}
