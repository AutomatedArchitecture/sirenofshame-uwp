using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class SosDb
    {
        private static readonly ILog _log = MyLogManager.GetLog(typeof(SosDb));
        private readonly IFileAdapter _fileAdapter = ServiceContainer.Resolve<IFileAdapter>();

        protected async Task Write(string location, string contents)
        {
            try
            {
                await _fileAdapter.AppendAllText(location, contents);
            }
            catch (IOException ex)
            {
                _log.Error("Unable to write: " + contents + " to " + location, ex);
            }
        }

        public async Task Write(BuildStatus buildStatus, SirenOfShameSettings settings, bool disableWritingToSosDb)
        {
            if (!disableWritingToSosDb)
            {
                await AppendToFile(buildStatus);
            }
            UpdateStatsInSettings(buildStatus, settings);
        }

        private static void UpdateStatsInSettings(BuildStatus buildStatus, SirenOfShameSettings settings)
        {
            if (string.IsNullOrEmpty(buildStatus.RequestedBy)) return;
            var personSetting = settings.FindAddPerson(buildStatus.RequestedBy);
            if (buildStatus.BuildStatusEnum == BuildStatusEnum.Broken)
            {
                personSetting.FailedBuilds++;
            }
            personSetting.TotalBuilds++;
            settings.Dirty();
        }

        private async Task AppendToFile(BuildStatus buildStatus)
        {
            string[] items = new[]
            {
                buildStatus.StartedTime == null ? "" : buildStatus.StartedTime.Value.Ticks.ToString(CultureInfo.InvariantCulture),
                buildStatus.FinishedTime == null ? "" : buildStatus.FinishedTime.Value.Ticks.ToString(CultureInfo.InvariantCulture),
                ((int) buildStatus.BuildStatusEnum).ToString(CultureInfo.InvariantCulture),
                buildStatus.RequestedBy
            };
            string contents = string.Join(",", items) + "\r\n";
            string location = GetBuildLocation(buildStatus);
            await Write(location, contents);
        }

        private string GetBuildLocation(BuildStatus buildStatus)
        {
            return GetBuildLocation(buildStatus.BuildDefinitionId);
        }

        private static string RemoveIllegalCharacters(string s)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars()) + "\\.";
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(s, "");
        }

        private string GetBuildLocation(BuildDefinitionSetting buildDefinition)
        {
            return GetBuildLocation(buildDefinition.Id);
        }

        private string GetEventsLocation()
        {
            return "SirenOfShameEvents.txt";
        }

        private string GetBuildLocation(string buildDefinitionId)
        {
            return RemoveIllegalCharacters(buildDefinitionId) + ".txt";
        }

        public async Task<IList<BuildStatus>> ReadAll()
        {
            var allTxtFiles = await _fileAdapter.GetFiles("*.txt");
            var allBuildFiles = allTxtFiles
                .Where(fileName => fileName != ".txt" && fileName != "SirenOfShameEvents.txt");
            var tasks = allBuildFiles.Select(ReadAllFromLocation).ToList();
            await Task.WhenAll(tasks);
            var enumerable = tasks.SelectMany(i => i.Result).ToList();
            return enumerable;
        }

        public async Task<IList<BuildStatus>> ReadAll(IEnumerable<BuildDefinitionSetting> buildDefinitionSettings)
        {
            var readTasks = buildDefinitionSettings
                .Select(ReadAllInternal)
                .ToList();
            await Task.WhenAll(readTasks);
            var buildStatuses = readTasks
                .SelectMany(t => t.Result)
                .ToList();
            return buildStatuses;
        }

        private async Task<IEnumerable<BuildStatus>> ReadAllInternal(BuildDefinitionSetting buildDefinitionSetting)
        {
            string location = GetBuildLocation(buildDefinitionSetting);
            var exists = await _fileAdapter.Exists(location);
            if (!exists) return new List<BuildStatus>();
            var results = await ReadAllFromLocation(location);
            return results;
        }

        public async Task<IList<BuildStatus>> ReadAll(string buildId)
        {
            var location = GetBuildLocation(buildId);
            return await ReadAllFromLocation(location);
        }

        private async Task<IList<BuildStatus>> ReadAllFromLocation(string location)
        {
            var exists = await _fileAdapter.Exists(location);
            if (!exists) return new List<BuildStatus>();
            var lines = await _fileAdapter.ReadAllLines(location);
            var buildDefinitionId = Path.GetFileNameWithoutExtension(location);
            var statuses = lines.Select(l => l.Split(','))
                .Where(l => l.Length == 4) // just in case there are partially written records
                .Select(lineFromSosDb => BuildStatus.Parse(lineFromSosDb, buildDefinitionId))
                .Where(i => i != null) // ignore parse errors
                .ToList();
            return statuses;
        }

        public async Task<IList<BuildStatus>> ReadAll(BuildDefinitionSetting buildDefinitionSetting)
        {
            string location = GetBuildLocation(buildDefinitionSetting);
            return await ReadAllFromLocation(location);
        }

        public async Task<string> ExportNewBuilds(SirenOfShameSettings settings)
        {
            if (string.IsNullOrEmpty(settings.MyRawName)) return null;
            DateTime? highWaterMark = settings.GetHighWaterMark();
            var initialExport = highWaterMark == null;
            var allBuildDefinitions = await ReadAll(settings.GetAllActiveBuildDefinitions());
            var currentUsersBuilds = allBuildDefinitions
                .Where(i => i.RequestedBy == settings.MyRawName)
                .Where(i => i.StartedTime != null);
            var buildsAfterHighWaterMark = initialExport ? currentUsersBuilds : currentUsersBuilds.Where(i => i.StartedTime > highWaterMark);
            var buildsAsExport = buildsAfterHighWaterMark.Select(i => i.AsSosOnlineExport());
            var result = string.Join("\r\n", buildsAsExport);
            return string.IsNullOrEmpty(result) ? null : result;
        }

        public async Task ExportNewNewsItem(NewNewsItemEventArgs args)
        {
            foreach (var argsNewsItemEvent in args.NewsItemEvents)
            {
                await ExportNewNewsItem(argsNewsItemEvent);
            }
        }

        public async Task ExportNewNewsItem(NewsItemEvent args)
        {
            var location = GetEventsLocation();
            string asCommaSeparated = args.AsCommaSeparated();
            if (!string.IsNullOrEmpty(asCommaSeparated))
            {
                string contents = asCommaSeparated + "\r\n";
                try
                {
                    await _fileAdapter.AppendAllText(location, contents);
                }
                catch (IOException ex)
                {
                    _log.Error("Unable to export news item: " + contents, ex);
                }
            }
        }

        public async Task<IList<NewsItemEvent>> GetAllNewsItems(SirenOfShameSettings settings)
        {
            try
            {
                var location = GetEventsLocation();
                var exists = await _fileAdapter.Exists(location);
                if (!exists) return new List<NewsItemEvent>();

                var allLines = await _fileAdapter.ReadAllLines(location);
                var result = allLines
                    .Select(i => NewsItemEvent.FromCommaSeparated(i, settings))
                    .Where(i => i != null)
                    .Reverse()
                    .ToList();
                return result;
            } catch (Exception exception) { 
                if (!(exception is FileNotFoundException))
                    _log.Error("Error getting most recent news items", exception);
                return new List<NewsItemEvent>();
            }

        }
    }
}