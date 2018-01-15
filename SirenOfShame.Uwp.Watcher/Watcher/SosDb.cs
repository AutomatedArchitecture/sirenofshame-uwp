using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SirenOfShame.Lib.Watcher;
using SirenOfShame.Uwp.Core.Interfaces;
using SirenOfShame.Uwp.Core.Models;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class SosDb
    {
        private static readonly ILog _log = MyLogManager.GetLog(typeof(SosDb));
        private readonly IFileAdapter _fileAdapter = ServiceContainer.Resolve<IFileAdapter>();

        private async Task Write(string location, string contents)
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
            Regex r = new Regex($"[{Regex.Escape(regexSearch)}]");
            return r.Replace(s, "");
        }

        private string GetBuildLocation(BuildDefinitionSetting buildDefinition)
        {
            return GetBuildLocation(buildDefinition.Id);
        }

        private string GetEventsLocation()
        {
            return "SirenOfShameEvents.sosdb";
        }

        private string GetBuildLocation(string buildDefinitionId)
        {
            return RemoveIllegalCharacters(buildDefinitionId) + ".sosdb";
        }

        public async Task<IList<BuildStatus>> ReadAll()
        {
            var allTxtFiles = await _fileAdapter.GetFiles("*.sosdb");
            var allBuildFiles = allTxtFiles
                .Where(fileName => fileName != ".sosdb" && fileName != "SirenOfShameEvents.sosdb");
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

        public static string MakeCsvSafe(string s)
        {
            return string.IsNullOrEmpty(s) ? "" : RemoveNewlines(s.Replace(',', ' '));
        }

        private static string RemoveNewlines(string s)
        {
            return string.IsNullOrEmpty(s) ? "" : s.Replace("\r\n", " ").Replace("\n", " ");
        }

        public string AsCommaSeparated(NewsItemEvent newsItem)
        {
            if (newsItem == null) return null;
            try
            {
                return string.Format("{0},{1},{2},{3},{4},{5},{6}",
                    newsItem.EventDate.Ticks,
                    MakeCsvSafe(newsItem.Person?.RawName),
                    (int)newsItem.NewsItemType,
                    newsItem.ReputationChange,
                    MakeCsvSafe(newsItem.BuildDefinitionId),
                    MakeCsvSafe(newsItem.BuildId),
                    RemoveNewlines(newsItem.Title));
            }
            catch (Exception ex)
            {
                _log.Error("Failed to serialize news item", ex);
                return null;
            }
        }


        public async Task ExportNewNewsItem(NewsItemEvent args)
        {
            var location = GetEventsLocation();
            string asCommaSeparated = AsCommaSeparated(args);
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
                    .Select(i => FromCommaSeparated(i, settings))
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
        
        public static NewsItemEvent FromCommaSeparated(string commaSeparated, SirenOfShameSettings settings)
        {
            try
            {
                var elements = commaSeparated.Split(',');
                if (elements.Length < 6)
                {
                    _log.Error("Found a news item with fewer than three elements" + commaSeparated);
                    return null;
                }
                var eventDate = GetEventDate(elements[0]);
                var person = GetPerson(settings, elements[1]);
                if (person == null)
                {
                    _log.Warn("Unable to find user " + elements[1]);
                    return null;
                }
                var newsItemType = GetNewsItemType(elements[2]);
                var reputationChange = GetReputationChange(elements[3]);
                var buildDefinitionId = GetString(elements[4]);
                var buildId = GetString(elements[5]);
                var title = GetTitle(elements, 6);
                return new SirenOfShame.Uwp.Core.Models.NewsItemEvent
                {
                    EventDate = eventDate,
                    Person = person,
                    Title = title,
                    NewsItemType = newsItemType,
                    ReputationChange = reputationChange,
                    BuildId = buildId,
                    BuildDefinitionId = buildDefinitionId
                };
            }
            catch (Exception ex)
            {
                _log.Error("Error parsing news item: " + commaSeparated, ex);
                return null;
            }
        }
        
        public static PersonSetting GetPerson(SirenOfShameSettings settings, string element)
        {
            var rawName = element;
            var person = settings.FindPersonByRawName(rawName);
            if (person == null)
            {
                _log.Error("Unable to find person from news item: " + rawName);
                return null;
            }
            return person;
        }

        public static string GetString(string element)
        {
            if (string.IsNullOrEmpty(element)) return null;
            return element;
        }

        public static int? GetReputationChange(string element)
        {
            var reputationChangeRaw = element;
            if (string.IsNullOrEmpty(reputationChangeRaw)) return null;
            return int.Parse(reputationChangeRaw);
        }

        public static NewsItemTypeEnum GetNewsItemType(string element)
        {
            var newsItemTypeRaw = element;
            var newsItemTypeInt = int.Parse(newsItemTypeRaw);
            return (NewsItemTypeEnum)newsItemTypeInt;
        }

        public static string GetTitle(IEnumerable<string> elements, int commentStart)
        {
            var title = string.Join(",", elements.Skip(commentStart));
            return title;
        }

        public static DateTime GetEventDate(string element)
        {
            var eventDateTicks = long.Parse(element);
            var eventDate = new DateTime(eventDateTicks);
            return eventDate;
        }
    }
}