using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SirenOfShame.Uwp.Watcher.Device;
using SirenOfShame.Uwp.Watcher.Helpers;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Settings;
using SirenOfShame.Uwp.Watcher.Util;

namespace SirenOfShame.Uwp.Watcher.Watcher
{
    public class RulesEngine
    {
        private struct ChangedBuildStatusesAndTheirPreviousState
        {
            public BuildStatus ChangedBuildStatus { get; set; }
            public BuildStatusEnum? PreviousWorkingOrBrokenBuildStatus { get; set; }
            public BuildStatusEnum? PreviousBuildStatus { get; set; }
        }

        private const int NEWS_ITEMS_TO_GET_ON_STARTUP = 10;
        private static readonly ILog _log = MyLogManager.GetLog(typeof(RulesEngine));
        private IDictionary<string, BuildStatus> PreviousWorkingOrBrokenBuildStatus { get; set; }
        private IDictionary<string, BuildStatus> PreviousBuildStatus { get; set; }

        private CancellationTokenSource _watcherCancellationToken;
        readonly Timer _buildsInProgressTimer;
        private readonly SettingsIoService _settingsIoService = ServiceContainer.Resolve<SettingsIoService>();
        private readonly SosDb _sosDb = ServiceContainer.Resolve<SosDb>();

        private readonly SirenOfShameSettings _settings;
        private readonly IList<WatcherBase> _watchers = new List<WatcherBase>();
        private bool DisableSosOnline { get; }
        private bool DisableWritingToSosDb { get; }

        public event UpdateStatusBarEvent UpdateStatusBar;
        public event StatusChangedEvent RefreshStatus;
        public event StatsChangedEvent StatsChanged;
        public event TrayNotifyEvent TrayNotify;
        public event ModalDialogEvent ModalDialog;
        public event SetAudioEvent SetAudio;
        public event SetLightsEvent SetLights;
        public event SetTrayIconEvent SetTrayIcon;
        public event NewAlertEvent NewAlert;
        public event PlayWindowsAudioEvent PlayWindowsAudio;
        public event NewAchievementEvent NewAchievement;
        public event NewNewsItemEvent NewNewsItem;
        public event NewUserEvent NewUser;

        private void InvokeNewUser(string requestedBy)
        {
            var firstPerson = _settings.People.FirstOrDefault(i => i.RawName == requestedBy);
            if (firstPerson == null)
            {
                _log.Error("Unable to find person " + requestedBy + " this should never happen.");
            }
            InvokeNewUser(new List<PersonSetting>() { firstPerson });
        }

        private void InvokeNewUser(List<PersonSetting> newPeople)
        {
            NewUser?.Invoke(this, new NewUserEventArgs
            {
                NewPeople = newPeople
            });
        }

        private async Task InvokeNewNewsItem(NewsItemEvent args, bool newsIsBothLocalAndNew)
        {
            await InvokeNewNewsItem(new NewNewsItemEventArgs
            {
                NewsItemEvents = new List<NewsItemEvent> {args}
            }, newsIsBothLocalAndNew);
        }

        private async Task InvokeNewNewsItem(NewNewsItemEventArgs args, bool newsIsBothLocalAndNew)
        {
            if (newsIsBothLocalAndNew)
                await _sosDb.ExportNewNewsItem(args);
            NewNewsItem?.Invoke(this, args);
        }

        private async Task InvokeNewAchievement(PersonSetting person, List<AchievementLookup> achievements)
        {
            NewAchievement?.Invoke(this, new NewAchievementEventArgs { Person = person, Achievements = achievements });
            foreach (var achievement in achievements)
            {
                await InvokeNewNewsItem(achievement.AsNewNewsItem(person), newsIsBothLocalAndNew: true);
            }
        }

        public void InvokePlayWindowsAudio(string location)
        {
            if (_settings.Mute) return;

            PlayWindowsAudio?.Invoke(this, new PlayWindowsAudioEventArgs { Location = location });
        }

        private void InvokeSetTrayIcon(TrayIcon trayIcon)
        {
            SetTrayIcon?.Invoke(this, new SetTrayIconEventArgs { TrayIcon = trayIcon });
        }

        private void InvokeNewAlert(NewAlertEventArgs args)
        {
            NewAlert?.Invoke(this, args);
        }

        public void InvokeTrayNotify(SosToolTipIcon tipIcon, string title, string tipText)
        {
            TrayNotify?.Invoke(this, new TrayNotifyEventArgs { TipIcon = tipIcon, TipText = tipText, Title = title });
        }

        public RulesEngine()
        {
            DisableSosOnline = false;
            DisableWritingToSosDb = false;
            ResetPreviousWorkingOrBrokenStatuses();
            _settings = ServiceContainer.Resolve<SirenOfShameSettings>();
            _buildsInProgressTimer = new Timer(BuildsInProgressTimerTick, null, 0, 1000);
        }

        private bool _serverPreviouslyUnavailable;

        private void BuildWatcherServerUnavailable(object sender, ServerUnavailableEventArgs args)
        {
            BuildWatcherServerUnavailableAsync(args).Wait();
        }

        private async Task BuildWatcherServerUnavailableAsync(ServerUnavailableEventArgs args)
        {
            InvokeUpdateStatusBar("Build server unavailable, attempting to reconnect", args.Exception);
            await SetStatusUnknown();
            // only notify that it was unavailable once
            if (_serverPreviouslyUnavailable)
            {
                return;
            }

            InvokeTrayNotify(SosToolTipIcon.Error, "Build Server Unavailable", "The connection will be restored when possible.");
            ResetPreviousWorkingOrBrokenStatuses();
            _serverPreviouslyUnavailable = true;
        }

        private void ResetPreviousWorkingOrBrokenStatuses()
        {
            PreviousWorkingOrBrokenBuildStatus = new Dictionary<string, BuildStatus>();
            PreviousBuildStatus = new Dictionary<string, BuildStatus>();
            _previousBuildStatuses = new BuildStatus[] { };
        }

        private BuildStatus[] _previousBuildStatuses = { };
        private bool _restarting = false;

        private void InvokeUpdateStatusBar(string statusText, Exception exception = null)
        {
            string datedStatusText = null;
            if (!string.IsNullOrEmpty(statusText))
            {
                datedStatusText = $"{DateTime.Now:G} - {statusText}";
            }
            UpdateStatusBar?.Invoke(this, new UpdateStatusBarEventArgs { StatusText = datedStatusText, Exception = exception });
        }

        private void BuildWatcherStatusChecked(object sender, StatusCheckedEventArgsArgs args)
        {
            // .Wait() rather than async void so that we can ensure it runs to completion
            //      before the polling does a 2nd check, possibly resuling in calling 
            //      ExecuteNewBuilds simultaneously
            ExecuteNewBuilds(args.BuildStatuses).Wait();
        }

        private void StoppedWatching(object sender, StoppedWatchingEventArgs args)
        {
            _restarting = true;
        }

        private async Task ExecuteNewBuilds(IList<BuildStatus> newBuildStatuses)
        {
            try
            {
                ApplyUserMappings(newBuildStatuses);
                SendCiServerConnectedEvents();
                TryToGetAndSendNewSosOnlineAlerts();
                var allBuildStatuses = BuildStatusUtil.Merge(_previousBuildStatuses, newBuildStatuses);
                var changedBuildStatuses = GetChangedBuildStatuses(allBuildStatuses);
                if (!changedBuildStatuses.Any())
                {
                    if (_restarting)
                        InvokeRefreshStatus(allBuildStatuses);
                    return;
                }
                InvokeSetTrayIcon(changedBuildStatuses);
                InvokeRefreshStatusIfAnythingChanged(allBuildStatuses, changedBuildStatuses);
                AddAnyNewPeopleToSettings(changedBuildStatuses);
                UpdateBuildNamesInSettingsIfAnyChanged(changedBuildStatuses);
                var changedBuildStatusesAndTheirPreviousState =
                    GetChangedBuildStatusesAndTheirPreviousState(changedBuildStatuses);
                FireApplicableRulesEngineEvents(changedBuildStatusesAndTheirPreviousState);
                await WriteNewBuildsToSosDb(changedBuildStatusesAndTheirPreviousState);
                await NotifyIfNewAchievements(changedBuildStatuses);
                InvokeStatsChanged(changedBuildStatuses);
                await SyncNewBuildsToSos(changedBuildStatuses);
                await InvokeNewNewsItemIfAny(changedBuildStatusesAndTheirPreviousState);
                CacheBuildStatuses(changedBuildStatuses);
                await SaveSettingsIfDirty();
            }
            catch (Exception ex)
            {
                // log and continue?
                _log.Error("Error in Watcher", ex);
            }
            finally
            {
                _restarting = false;
            }
        }

        private async Task SaveSettingsIfDirty()
        {
            await _settingsIoService.SaveIfDirty();
        }

        private void ApplyUserMappings(IList<BuildStatus> buildStatuses)
        {
            foreach (var buildStatus in buildStatuses)
            {
                string requestedBy = buildStatus.RequestedBy;
                var userMapping = _settings.UserMappings.FirstOrDefault(i => i.WhenISee == requestedBy);
                bool userMappingExistsForThisUser = userMapping != null;
                if (userMappingExistsForThisUser)
                {
                    buildStatus.RequestedBy = userMapping.PretendItsActually;
                }
            }
        }

        private async Task InvokeNewNewsItemIfAny(IEnumerable<ChangedBuildStatusesAndTheirPreviousState> changedBuildStatuses)
        {
            var newNewsItemEventArgses = changedBuildStatuses
                .Where(i => i.PreviousWorkingOrBrokenBuildStatus != null && !string.IsNullOrEmpty(i.ChangedBuildStatus.RequestedBy))
// ReSharper disable PossibleInvalidOperationException
                .Select(i => i.ChangedBuildStatus.AsNewsItemEventArgs(i.PreviousWorkingOrBrokenBuildStatus.Value, _settings))
// ReSharper restore PossibleInvalidOperationException
                .ToList();

            foreach (var newsItemEvent in newNewsItemEventArgses)
            {
                await InvokeNewNewsItem(newsItemEvent, newsIsBothLocalAndNew: true);
            }
        }

        private async Task WriteNewBuildsToSosDb(IEnumerable<ChangedBuildStatusesAndTheirPreviousState> changedBuildStatusesAndTheirPreviousState)
        {
            var previouslyWorkingOrBrokenBuilds = changedBuildStatusesAndTheirPreviousState
                .Where(i => i.ChangedBuildStatus.IsWorkingOrBroken() && i.PreviousWorkingOrBrokenBuildStatus != null)
                .ToList();
            foreach (var i in previouslyWorkingOrBrokenBuilds)
            {
                await _sosDb.Write(i.ChangedBuildStatus, _settings, DisableWritingToSosDb);
            }
        }

        private void FireApplicableRulesEngineEvents(IEnumerable<ChangedBuildStatusesAndTheirPreviousState> changedBuildStatusesAndTheirPreviousState)
        {
            foreach (var buildStatus in changedBuildStatusesAndTheirPreviousState)
            {
                buildStatus.ChangedBuildStatus.FireApplicableRulesEngineEvents(buildStatus.PreviousWorkingOrBrokenBuildStatus,
                                                                               buildStatus.PreviousBuildStatus,
                                                                               this,
                                                                               _settings.Rules);
            }
        }

        private List<ChangedBuildStatusesAndTheirPreviousState> GetChangedBuildStatusesAndTheirPreviousState(IEnumerable<BuildStatus> changedBuildStatuses)
        {
            var result = changedBuildStatuses
                .Select(changedBuildStatus => new ChangedBuildStatusesAndTheirPreviousState
                {
                    PreviousWorkingOrBrokenBuildStatus = TryGetBuildStatus(changedBuildStatus, PreviousWorkingOrBrokenBuildStatus),
                    PreviousBuildStatus = TryGetBuildStatus(changedBuildStatus, PreviousBuildStatus),
                    ChangedBuildStatus = changedBuildStatus,
                });
            return result.ToList();
        }

        /// <summary>
        /// We cache the build statuses primarily so we can tell the rules engine whether a build
        /// changed from Broken->InProgress->Working or Broken->InProgress, etc
        /// </summary>
        private void CacheBuildStatuses(IEnumerable<BuildStatus> changedBuildStatuses)
        {
            foreach (var changedBuildStatus in changedBuildStatuses)
            {
                SetValue(changedBuildStatus, PreviousBuildStatus);
                if (changedBuildStatus.IsWorkingOrBroken())
                {
                    SetValue(changedBuildStatus, PreviousWorkingOrBrokenBuildStatus);
                }
            }
        }

        private void UpdateBuildNamesInSettingsIfAnyChanged(IEnumerable<BuildStatus> changedBuildStatuses)
        {
            foreach (var build in changedBuildStatuses)
            {
                _settings.UpdateNameIfChanged(build);
            }
        }

        private void SendCiServerConnectedEvents()
        {
            if (_serverPreviouslyUnavailable)
            {
                InvokeTrayNotify(SosToolTipIcon.Info, "Reconnected", "Reconnected to server.");
            }
            _serverPreviouslyUnavailable = false;
            InvokeUpdateStatusBar("Connected");
        }

        // e.g. if a build exists in newStatus but doesn't exit in oldStatus, return it.  If a build exists in
        //  oldStatus and in newStatus and the BuildStatusEnum is different then return it.
        private IList<BuildStatus> GetChangedBuildStatuses(BuildStatus[] allBuildStatuses)
        {
            var oldBuildStatus = _previousBuildStatuses;
            _previousBuildStatuses = allBuildStatuses;
            var changedBuildStatuses = from newStatus in allBuildStatuses
                                       from oldStatus in oldBuildStatus.Where(s => s.BuildDefinitionId == newStatus.BuildDefinitionId).DefaultIfEmpty()
                                       where DidBuildStatusChange(oldStatus, newStatus)
                                       select newStatus;
            
            Debug.Assert(changedBuildStatuses != null, "changedBuildStatuses should not be null");
            Debug.Assert(PreviousWorkingOrBrokenBuildStatus != null, "PreviousWorkingOrBrokenBuildStatus should never be null");
            Debug.Assert(PreviousBuildStatus != null, "PreviousBuildStatus should never be null");

            return changedBuildStatuses.ToList();
        }

        private static bool DidBuildStatusChange(BuildStatus oldStatus, BuildStatus newStatus)
        {
            if (oldStatus == null) return true;

            bool startTimesUnequal = oldStatus.StartedTime != newStatus.StartedTime;
            bool buildStatusesUnequal = oldStatus.BuildStatusEnum != newStatus.BuildStatusEnum;
            bool buildChanged = 
                startTimesUnequal || buildStatusesUnequal;
            
            if (buildChanged)
            {
                string message = string.Format(
                    "Detected a build status change. BuildDefinitionId: {0}; OldStartTime: {1}; NewStartTime: {2}; OldStatus: {3}; NewStatus: {4}; BuildId: {5}; RequestedBy: {6}", 
                    newStatus.BuildDefinitionId, 
                    oldStatus.StartedTime, 
                    newStatus.StartedTime, 
                    oldStatus.BuildStatusEnum, 
                    newStatus.BuildStatusEnum,
                    newStatus.BuildId,
                    newStatus.RequestedBy
                    );
                _log.Debug(message);
            }

            return buildChanged;
        }

        private void TryToGetAndSendNewSosOnlineAlerts()
        {
            if (DisableSosOnline) return;
            SosOnlineService.TryToGetAndSendNewSosOnlineAlerts(_settings, Now, InvokeNewAlert);
        }

        protected virtual DateTime Now => DateTime.Now;

        private void InvokeStatsChanged(IList<BuildStatus> changedBuildStatuses)
        {
            if (!changedBuildStatuses.Any(i => i.IsWorkingOrBroken())) return;
            var statsChanged = StatsChanged;
            if (statsChanged == null) return;
            var changedPeople = from changedBuildStatus in changedBuildStatuses
                join person in _settings.VisiblePeople on changedBuildStatus.RequestedBy equals person.RawName
                select person;
            var uniqueChangedPeople = changedPeople.GroupBy(p => p.RawName).Select(i => i.First()).ToList();

            statsChanged(this, new StatsChangedEventArgs
            {
                ChangedBuildStatuses = changedBuildStatuses,
                ChangedPeople = uniqueChangedPeople
            });
        }

        private void InvokeRefreshStatus(IEnumerable<BuildStatus> buildStatuses)
        {
            IList<BuildStatusDto> buildStatusListViewItems = buildStatuses
                .Select(bs => bs.AsBuildStatusDto(DateTime.Now, PreviousWorkingOrBrokenBuildStatus, _settings))
                .ToList();

            RefreshStatus?.Invoke(this, new RefreshStatusEventArgs { BuildStatusDtos = buildStatusListViewItems });
        }

        private void BuildsInProgressTimerTick(object sender)
        {
            if (_previousBuildStatuses.Any(bs => bs.BuildStatusEnum == BuildStatusEnum.InProgress))
            {
                InvokeRefreshStatus(_previousBuildStatuses);
            }
        }

        private void InvokeRefreshStatusIfAnythingChanged(IEnumerable<BuildStatus> allBuildStatuses, IEnumerable<BuildStatus> changedBuildStatuses)
        {
            if (changedBuildStatuses.Any())
            {
                _log.Debug("InvokeRefreshStatus: Some build status changed");
                InvokeRefreshStatus(allBuildStatuses);
            }
        }

        protected virtual SosOnlineService SosOnlineService { get; } = new SosOnlineService();

        private async Task SyncNewBuildsToSos(IList<BuildStatus> changedBuildStatuses)
        {
            if (!_settings.SosOnlineAlwaysSync) return;
            var noUsername = string.IsNullOrEmpty(_settings.SosOnlineUsername);
            if (noUsername) return;

            TrySynchronizeBuildStatuses(changedBuildStatuses);
            await TrySynchronizeMyPointsAndAchievements(changedBuildStatuses);
        }

        private void TrySynchronizeBuildStatuses(IList<BuildStatus> changedBuildStatuses)
        {
            if (DisableSosOnline) return;
            if (_settings.SosOnlineWhatToSync != WhatToSyncEnum.BuildStatuses) return;
            var requestedByPeople = _settings.GetUsersContainedInBuildsAsDto(changedBuildStatuses);
            SosOnlineService.BuildStatusChanged(_settings, changedBuildStatuses, requestedByPeople);
        }

        private async Task TrySynchronizeMyPointsAndAchievements(IList<BuildStatus> changedBuildStatuses)
        {
            if (DisableSosOnline) return;
            if (!changedBuildStatuses.Any(i => i.IsWorkingOrBroken())) return;
            var anyBuildsAreMine = changedBuildStatuses.Any(i => i.RequestedBy == _settings.MyRawName && i.IsWorkingOrBroken());
            if (!anyBuildsAreMine) return;
            var exportedBuilds = await _sosDb.ExportNewBuilds(_settings);
            var noBuildsToExport = exportedBuilds == null;
            if (noBuildsToExport)
            {
                _log.Error("No builds were found to export from sosDb to sos online even though one was changed");
                return;
            }
            _log.Debug("Uploading the following builds to sos online: " + exportedBuilds);
            string exportedAchievements = _settings.ExportNewAchievements();
            SosOnlineService.Synchronize(_settings, exportedBuilds, exportedAchievements, OnAddBuildsSuccess, OnAddBuildsFail);
        }

        private void OnAddBuildsFail(string userTargedErrorMessage, Exception ex)
        {
            _log.Error("Failed to connect to SoS online", ex);
            InvokeUpdateStatusBar(userTargedErrorMessage, ex);
        }

        private void OnAddBuildsSuccess(DateTime newHighWaterMark)
        {
            _log.Debug("Successfully uploaded to sos online. New high water mark: " + newHighWaterMark);
            _settings.SosOnlineHighWaterMark = newHighWaterMark.Ticks;
        }

        private async Task NotifyIfNewAchievements(IList<BuildStatus> changedBuildStatuses)
        {
            if (!changedBuildStatuses.Any(i => i.IsWorkingOrBroken())) return;
            var visiblePeopleWithNewChanges = from changedBuildStatus in changedBuildStatuses
                                             join person in _settings.VisiblePeople on changedBuildStatus.RequestedBy equals person.RawName
                                             where changedBuildStatus.IsWorkingOrBroken()
                                             select new
                                             {
                                                 Person = person,
                                                 Build = changedBuildStatus
                                             };

            foreach (var personWithNewChange in visiblePeopleWithNewChanges)
            {
                var newAchievements = await personWithNewChange.Person.CalculateNewAchievements(_settings, personWithNewChange.Build);
                List<AchievementLookup> achievements = newAchievements.ToList();
                if (achievements.Any())
                {
                    personWithNewChange.Person.AddAchievements(achievements);
                    await InvokeNewAchievement(personWithNewChange.Person, achievements);
                }
                // this is required because achievements often write to settings e.g. cumulative build time
                _settings.Dirty();
            }
        }

        private static BuildStatusEnum? TryGetBuildStatus(BuildStatus changedBuildStatus, IDictionary<string, BuildStatus> dictionary)
        {
            BuildStatus previousWorkingOrBrokenBuildStatus;
            dictionary.TryGetValue(changedBuildStatus.BuildDefinitionId, out previousWorkingOrBrokenBuildStatus);

            return previousWorkingOrBrokenBuildStatus?.BuildStatusEnum;
        }

        private static void SetValue(BuildStatus changedBuildStatus, IDictionary<string, BuildStatus> dictionary)
        {
            try
            {
                if (!dictionary.ContainsKey(changedBuildStatus.BuildDefinitionId))
                    dictionary.Add(changedBuildStatus.BuildDefinitionId, changedBuildStatus);
                else
                    dictionary[changedBuildStatus.BuildDefinitionId] = changedBuildStatus;
            }
            catch (IndexOutOfRangeException)
            {
                // todo: migrage "var name = Thread.CurrentThread.Name;"
                var name = "???";

                _log.Error("Tried to update the cache from the thread '" + name + "' but failed because the cache was previously accessed from a different thread. This could cause errors in determining whether a build changed.");
            }
        }

        private void InvokeSetTrayIcon(IEnumerable<BuildStatus> buildStatuses)
        {
            var buildStatusesAndSettings = from buildStatus in buildStatuses
                                           join setting in _settings.CiEntryPointSettings.SelectMany(i => i.BuildDefinitionSettings) on buildStatus.BuildDefinitionId equals setting.Id
                                           select new { buildStatus, setting };
            bool anyBuildBroken = buildStatusesAndSettings
                .Any(bs => bs.setting.AffectsTrayIcon && (
                    bs.buildStatus.BuildStatusEnum == BuildStatusEnum.Broken));
            TrayIcon trayIcon = anyBuildBroken ? TrayIcon.Red : TrayIcon.Green;
            InvokeSetTrayIcon(trayIcon);
        }

        private void AddAnyNewPeopleToSettings(IEnumerable<BuildStatus> changedBuildStatuses)
        {
            var buildDefinitionSettings = _settings.CiEntryPointSettings
                .SelectMany(i => i.BuildDefinitionSettings)
                .ToList();

            var buildStatusesWithNewPeople = from buildStatus in changedBuildStatuses
                                             join setting in buildDefinitionSettings on buildStatus.BuildDefinitionId equals setting.Id
                                             where !setting.ContainsPerson(buildStatus)
                                                && !string.IsNullOrEmpty(buildStatus.RequestedBy)
                                             select new { buildStatus, setting };

            var buildStatusesWithNewPeopleList = buildStatusesWithNewPeople
                .GroupBy(i => i.buildStatus.RequestedBy)
                .Select(i => i.First())
                .ToList();
            if (!buildStatusesWithNewPeopleList.Any()) return;

            var allExistingPeople = buildDefinitionSettings
                .SelectMany(bds => bds.People)
                .Distinct()
                .ToList();
            var newPeople = buildStatusesWithNewPeopleList
                .Select(s => s.buildStatus.RequestedBy)
                .Where(p => allExistingPeople.All(p1 => p1 != p))
                .ToList();

            buildStatusesWithNewPeopleList
                .ForEach(bss => bss.setting.People.Add(bss.buildStatus.RequestedBy));
            _settings.Dirty();

            newPeople.ForEach(InvokeNewUser);
        }

        internal void InvokeModalDialog(string dialogText, string okText)
        {
            var modalDialog = ModalDialog;
            modalDialog?.Invoke(this, new ModalDialogEventArgs { DialogText = dialogText, OkText = okText });
        }

        public void InvokeSetAudio(AudioPattern audioPattern, int? duration)
        {
            if (_settings.Mute) return;
            SetAudio?.Invoke(this, new SetAudioEventArgs { AudioPattern = audioPattern, Duration = duration });
        }

        public void InvokeSetLights(LedPattern ledPattern, int? duration)
        {
            SetLights?.Invoke(this, new SetLightsEventArgs { LedPattern = ledPattern, Duration = duration });
        }

        public async Task Start(bool initialStart)
        {
            var ciEntryPointSettings = _settings.CiEntryPointSettings
                .Where(s => !string.IsNullOrEmpty(s.Url))
                .ToList();

            StartWatchers(ciEntryPointSettings);
            await Task.Yield();
        }

        private void StartWatchers(List<CiEntryPointSetting> ciEntryPointSettings)
        {
            _watchers.Clear();
            foreach (var ciEntryPointSetting in ciEntryPointSettings)
            {
                WatcherBase watcher = ciEntryPointSetting.GetWatcher(_settings);
                _watchers.Add(watcher);
                watcher.StatusChecked += BuildWatcherStatusChecked;
                watcher.StoppedWatching += StoppedWatching;
                watcher.ServerUnavailable += BuildWatcherServerUnavailable;
                watcher.BuildDefinitionNotFound += BuildDefinitionNotFound;
                watcher.Settings = _settings;
                watcher.CiEntryPointSetting = ciEntryPointSetting;
                // todo: It looks like we are overwriting preceding watcher threads with subsequent ones which will cause problems when we try to Stop() them
                _watcherCancellationToken = new CancellationTokenSource();
                var task = new Task(async () => await watcher.StartWatching(_watcherCancellationToken.Token),
                    _watcherCancellationToken.Token);
                task.Start();
            }
        }

        public async Task SendLatest()
        {
            if (_watchers.Any())
            {
                InvokeUpdateStatusBar("Attempting to connect to server");
                await SendRecentNews();
                AddExistingLeaders();
                _restarting = true;
            }
            else
            {
                InvokeUpdateStatusBar("");
                InvokeRefreshStatus(Enumerable.Empty<BuildStatus>());
            }
        }

        private void AddExistingLeaders()
        {
            var peopleByReputation = _settings.People
                .OrderByDescending(i => i.GetReputation())
                .ToList();
            if (peopleByReputation.Count > 0)
            {
                InvokeNewUser(peopleByReputation);
            }
        }

        private async Task SendRecentNews()
        {
            var allEvents = await _sosDb.GetAllNewsItems(_settings);
            var recentEvent = allEvents
                .Take(NEWS_ITEMS_TO_GET_ON_STARTUP)
                .ToList();
            if (recentEvent.Count == 0) return;
            await InvokeNewNewsItem(new NewNewsItemEventArgs
            {
                NewsItemEvents = recentEvent
            }, newsIsBothLocalAndNew: false);
        }

        private void BuildDefinitionNotFound(object sender, BuildDefinitionNotFoundArgs args)
        {
            if (args.BuildDefinitionSetting == null)
            {
                _log.Warn("BuildDefinitionNotFound, yet no BuildDefinition provided.");
                return;
            }
            args.BuildDefinitionSetting.Active = false;
            _settings.Dirty();
            InvokeTrayNotify(SosToolTipIcon.Error, "Can't Find " + args.BuildDefinitionSetting.Name, "This build will be removed from the list of watched builds.\nYou may add it back from the 'Configure CI Server' button.");
        }

        private async Task SetStatusUnknown()
        {
            InvokeSetTrayIcon(TrayIcon.Question);
            var tasks = _settings.CiEntryPointSettings
                .SelectMany(i => i.BuildDefinitionSettings)
                .Where(bd => bd.Active)
                .Select(bd => bd.AsUnknownBuildStatus(_sosDb))
                .ToList();
            var buildStatuses = await Task.WhenAll(tasks);
            InvokeRefreshStatus(buildStatuses);
        }

        public async Task RefreshAll()
        {
            Stop();
            await Start(initialStart: false);
        }

        private void Stop()
        {
            _buildsInProgressTimer.Change(-1, int.MaxValue);
            _watcherCancellationToken?.Cancel();
        }

        // todo: implement sos online
        //public void SyncAllBuildStatuses()
        //{
        //    if (DisableSosOnline) return;
        //    if (_settings.SosOnlineWhatToSync == WhatToSyncEnum.BuildStatuses)
        //    {
        //        _sosOnlineService.BuildStatusChanged(
        //            _settings,
        //            PreviousWorkingOrBrokenBuildStatus.Select(i => i.Value).ToList(),
        //            _settings.People.Select(i => new InstanceUserDto(i)).ToList()
        //            );
        //    }
        //}
    }
}
