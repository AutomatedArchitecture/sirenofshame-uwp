using System;
using System.Threading.Tasks;
using Polly;
using SirenOfShame.Uwp.Core.Interfaces;

namespace SirenOfShame.Uwp.Core.Services
{
    public abstract class MessageRelayServiceBase
    {
        private readonly ILog _log = MyLogManager.GetLog(typeof(MessageRelayServiceBase));
        protected const string APP_SERVICE_NAME = "SirenOfShameMessageRelay";

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
