using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Dto;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Test.Unit.Services
{
    public class SosOnlineServiceDummy : SosOnlineService
    {
        public override void VerifyCredentialsAsync(SirenOfShameSettings settings, Action onSuccess, Action<string, Exception> onFail)
        {
        }

        public override void Synchronize(SirenOfShameSettings settings, string exportedBuilds, string exportedAchievements, Action<DateTime> onSuccess, Action<string, Exception> onFail)
        {
        }

        public override void TryToGetAndSendNewSosOnlineAlerts(SirenOfShameSettings settings, DateTime now, Action<NewAlertEventArgs> invokeNewAlert)
        {
        }

        public override Task<DesktopAppConnectionResult> StartRealtimeConnection(SirenOfShameSettings settings)
        {
            var taskCompletionSource = new TaskCompletionSource<DesktopAppConnectionResult>();
            taskCompletionSource.SetResult(new DesktopAppConnectionResult { Success = true });
            return taskCompletionSource.Task;
        }

        public override void SendMessage(SirenOfShameSettings settings, string message)
        {
        }

        public override void BuildStatusChanged(SirenOfShameSettings settings, IList<BuildStatus> changedBuildStatuses, List<InstanceUserDto> requestedByPeople)
        {
        }
    }
}
