using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Exceptions;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Watcher;

namespace SirenOfShame.Uwp.Watcher.HudsonServices
{
    public class HudsonWatcher : WatcherBase
    {
        private readonly HudsonCIEntryPoint _hudsonCiEntryPoint;
        private readonly HudsonService _service = new HudsonService();

        public HudsonWatcher(SirenOfShameSettings settings, HudsonCIEntryPoint hudsonCiEntryPoint)
            : base(settings)
        {
            _hudsonCiEntryPoint = hudsonCiEntryPoint;
        }

        protected override IList<BuildStatus> GetBuildStatus()
        {
            var watchedBuildDefinitions = GetAllWatchedBuildDefinitions().ToArray();

            if (string.IsNullOrEmpty(CiEntryPointSetting.Url))
                throw new SosException("Jenkins URL is null or empty");

            try
            {
                var buildsStatusTasks = _service.GetBuildsStatuses(CiEntryPointSetting, watchedBuildDefinitions);
                var buildsStatuses = Task.WhenAll(buildsStatusTasks).Result;
                return buildsStatuses
                    .Cast<BuildStatus>().ToList();
            }
            catch (AggregateException ex)
            {
                var serverUnavailable = ex.InnerExceptions.FirstOrDefault(i => i is ServerUnavailableException);
                if (serverUnavailable != null) throw serverUnavailable;
                throw;
            }
            catch (WebException ex)
            {
                if (
                    ex.Message.StartsWith("The remote name could not be resolved:") ||
                    ex.Message.Contains("Unable to connect to the remote server")
                    )
                {
                    throw new ServerUnavailableException();
                }
                throw;
            }
        }

        private IEnumerable<BuildDefinitionSetting> GetAllWatchedBuildDefinitions()
        {
            var activeBuildDefinitionSettings = CiEntryPointSetting.BuildDefinitionSettings.Where(bd => bd.Active && bd.BuildServer == _hudsonCiEntryPoint.Name);
            return activeBuildDefinitionSettings;
        }

        public override void StopWatching()
        {

        }

        public override void Dispose()
        {

        }
    }
}
