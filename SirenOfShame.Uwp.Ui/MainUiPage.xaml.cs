using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using SirenOfShame.Uwp.Ui.Models;
using SirenOfShame.Uwp.Ui.Services;
using SirenOfShame.Uwp.Watcher.Services;
using SirenOfShame.Uwp.Watcher.Util;
using SirenOfShame.Uwp.Watcher.Watcher;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SirenOfShame.Uwp.Ui
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainUiPage
    {
        private readonly MessageDistributorService _messageDistributorService = ServiceContainer.Resolve<MessageDistributorService>();
        private readonly MessageRelayService _messageRelayService = ServiceContainer.Resolve<MessageRelayService>();
        private RootViewModel ViewModel { get; set; }
        private readonly ILog _log = MyLogManager.GetLog(typeof(MainUiPage));
        private List<BuildStatusDto> _lastBuildStatusDtos;
        private Timer _prettyDateTimer;

        public MainUiPage()
        {
            InitializeComponent();
            _messageDistributorService.NewNewsItem += MessageDistributorServiceOnNewNewsItem;
            _messageDistributorService.NewPerson += MessageDistributorServiceOnNewPerson;
            _messageDistributorService.RefreshStatus += MessageDistributorServiceOnRefreshStatus;
            _messageDistributorService.StatsChanged += MessageDistributorServiceOnStatsChanged;
            LoadInitialData();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                // if we start UI and Server at the same time, give the server time to start up
                SetStatus($"Connecting to server...");
                await Task.Delay(2000);
                await _messageDistributorService.SendLatest();
                _prettyDateTimer = new Timer(PrettyDateOnTick, null, 0, 10000);
                SetStatus(null);
            }
            catch (EndpointNotFoundException)
            {
                await EnsureConnected();
                await _messageDistributorService.SendLatest();
            }
        }

        private async void PrettyDateOnTick(object state)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ViewModel.BuildDefinitions.ToList().ForEach(i => i.CalculatePrettyDate());
                ViewModel.News.ToList().ForEach(i => i.CalculatePrettyDate());
            });
        }

        private async Task EnsureConnected()
        {
            var retryDelay = 10000;
            await Task.Delay(retryDelay);
            while (!_messageRelayService.IsConnected)
            {
                try
                {
                    await _messageRelayService.Open();
                }
                catch (Exception ex)
                {
                    _log.Error("Unable to connect, retrying", ex);
                    SetStatus($"Unable to connect to siren of shame engine. Retrying in {(retryDelay / 1000)} seconds...");
                    await Task.Delay(retryDelay);
                }
            }
        }

        private void SetStatus(string statusText)
        {
            var hide = string.IsNullOrEmpty(statusText);
            Status.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            if (statusText != null)
            {
                Status.Text = statusText;
            }
        }

        private async void MessageDistributorServiceOnStatsChanged(object sender, StatsChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var leaderPairs = from oldLeader in ViewModel.Leaders
                    join newLeader in args.ChangedPeople on oldLeader.RawName equals newLeader.RawName
                    select new {oldLeader, newLeader};
                foreach (var leaderPair in leaderPairs)
                {
                    leaderPair.oldLeader.Update(leaderPair.newLeader);
                }
                ViewModel.Leaders.SortDescending(i => i.Reputation);
            });
        }

        private async void MessageDistributorServiceOnRefreshStatus(object sender, RefreshStatusEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                _lastBuildStatusDtos = args.BuildStatusDtos
                    .OrderByDescending(i => i.LocalStartTime)
                    .Take(50)
                    .ToList();
                RefreshBuildStatuses(_lastBuildStatusDtos);
            });
        }

        private void RefreshBuildStatuses(List<BuildStatusDto> lastBuildStatusDtos)
        {
            RemoveAllChildControlsIfBuildCountOrBuildNamesChanged(lastBuildStatusDtos);
            if (ViewModel.BuildDefinitions.Count == 0)
            {
                ViewModel.BuildDefinitions = new ObservableCollection<BuildStatusDto>(_lastBuildStatusDtos);
            }
            else
            {
                UpdateExistingControls(_lastBuildStatusDtos);
            }
        }

        private void UpdateExistingControls(List<BuildStatusDto> lastBuildStatusDtos)
        {
            UpdateDetailsInExistingControls(lastBuildStatusDtos);
            SortExistingControls();
        }

        private void UpdateDetailsInExistingControls(List<BuildStatusDto> lastBuildStatusDtos)
        {
            var buildsJoined = from existingBd in ViewModel.BuildDefinitions
                join newBd in lastBuildStatusDtos on existingBd.BuildDefinitionId equals newBd.BuildDefinitionId
                select new {existingBd, newBd};

            foreach (var builds in buildsJoined)
            {
                builds.existingBd.Update(builds.newBd);
            }
        }

        private void SortExistingControls()
        {
            ViewModel.BuildDefinitions.Sort();
        }

        private void RemoveAllChildControlsIfBuildCountOrBuildNamesChanged(ICollection<BuildStatusDto> buildStatusDtos)
        {
            bool numberOfBuildsChanged = ViewModel.BuildDefinitions.Count != buildStatusDtos.Count();
            bool anyNewBuildDefIds = buildStatusDtos
                .Any(newBd => ViewModel.BuildDefinitions.All(oldBd => newBd.BuildDefinitionId != oldBd.BuildDefinitionId));
            if (numberOfBuildsChanged || anyNewBuildDefIds)
            {
                _log.Debug("Removing child controls because: numberOfBuildsChanged: " + numberOfBuildsChanged);
                ViewModel.BuildDefinitions.Clear();
            }
        }

        private async void MessageDistributorServiceOnNewPerson(object sender, NewUserEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                foreach (var personSetting in args.NewPeople)
                {
                    var personDto = new PersonDto(personSetting);
                    ViewModel.Leaders.Add(personDto);
                }
            });
        }

        private async void MessageDistributorServiceOnNewNewsItem(object sender, NewNewsItemEventArgs newNewsItemEventArgs)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                foreach (var newsItemEvent in newNewsItemEventArgs.NewsItemEvents)
                {
                    if (newsItemEvent.ShouldUpdateOldInProgressNewsItem)
                        TryToFindAndUpdateOldInProgressNewsItem(newsItemEvent);
                    else
                        AddNewsItemToPanel(newsItemEvent);
                }
            });
        }

        private void AddNewsItemToPanel(NewsItemEvent newsItemEvent)
        {
            NewsItemDto newsItemDto = new NewsItemDto(newsItemEvent);
            ViewModel.News.Insert(0, newsItemDto);
        }

        private void TryToFindAndUpdateOldInProgressNewsItem(NewsItemEvent newsItemEvent)
        {
            var oldBuild = ViewModel.News.FirstOrDefault(i => i.BuildId == newsItemEvent.BuildId);
            if (oldBuild == null)
            {
                AddNewsItemToPanel(newsItemEvent);
            }
            else
            {
                oldBuild.UpdateState(newsItemEvent);
            }
        }

        private void LoadInitialData()
        {
            ViewModel = new RootViewModel
            {
                Leaders = new ObservableCollection<PersonDto>(),
                News = new ObservableCollection<NewsItemDto>()
            };
            DataContext = ViewModel;
        }
    }
}
